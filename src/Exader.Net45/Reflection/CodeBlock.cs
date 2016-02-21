using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Exader.Reflection
{
	/// <summary>
	/// TODO Переделать на основе примеров:
	/// http://blogs.msdn.com/b/zelmalki/archive/2008/12/11/msil-parser-quick-update.aspx
	/// http://evain.net/blog/articles/2009/04/30/reflection-based-cil-reader
	/// </summary>
	public class CodeBlock
	{
		public static CodeBlock Create(MethodInfo methodInfo)
		{
			MethodBody body = methodInfo.GetMethodBody();
			return null == body ? null : new CodeBlock(body, methodInfo);
		}

		/// <summary>
		/// .NET operands have varying lengths, so we need to figure out the length in each particular case
		/// </summary>
		private static int GetOperandLength(OpCode op, byte[] msil, int position)
		{
			switch (op.OperandType)
			{
				case OperandType.InlineSwitch:
					{
						uint numberOfCases = BitConverter.ToUInt32(msil, position);
						return (int)(4 * (1 + numberOfCases)); //add 1 to include the 32-bit case-count    
					}

				case OperandType.InlineNone:
					return 0;

				case OperandType.ShortInlineBrTarget:
				case OperandType.ShortInlineI:
				case OperandType.ShortInlineVar:
					return 1;

				case OperandType.InlineVar:
					return 2;

				case OperandType.InlineBrTarget:
				case OperandType.InlineTok:
				case OperandType.InlineType:
				case OperandType.InlineSig:
				case OperandType.InlineString:
				case OperandType.InlineField:
				case OperandType.InlineI:
				case OperandType.InlineMethod:
				case OperandType.ShortInlineR:
					return 4;

				case OperandType.InlineI8:
				case OperandType.InlineR:
					return 8;

				default:
					{
						throw new NotSupportedException("Operand type " + op.OperandType + " not supported.");
					}
			}
		}

		private readonly MethodBody body;

		private readonly List<Instruction> list;

		private readonly MethodInfo method;

		private CodeBlock(MethodBody body, MethodInfo method)
		{
			this.method = method;
			this.body = body;
			list = new List<Instruction>();

			byte[] msil = body.GetILAsByteArray();
			int position = 0;
			while (position < msil.Length)
			{
				//read 1 or 2 byte opcode.  Our Opcode lists contain Nullable<OpCode>, to ensure that if there is an error in indexing them, where we end up with something that's completely out of range (i.e. an un-used cell in the array) we will get an exception (since HasValue will be false in that case)
				OpCode code;
				byte byteFromMsil = msil[position++];
				if (byteFromMsil != 0xFE) // all double byte opcodes start with FE
				{
					code = OpCodeLists.SingleByte[byteFromMsil].Value;
				}
				else
				{
					code = OpCodeLists.DoubleByte[msil[position++]].Value;
				}

				// read operand bytes
				int operandLength = GetOperandLength(code, msil, position);
				var operandBytes = new byte[operandLength];
				Array.Copy(msil, position, operandBytes, 0, operandLength);

				// Create Instruction object
				new Instruction(this, code, operandBytes);

				position += operandLength;

			}
		}

		[Obsolete]
		public IEnumerable<FieldInfo> FieldReturns()
		{
			foreach (Instruction inst in list)
			{
				if (inst.IsReturn)
				{
					yield return inst.GetLoadedField();
				}
			}
		}

		[Obsolete]
		public FieldInfo FindBackingField()
		{
			return FieldReturns().Single();
		}

		[Obsolete]
		public IEnumerable<MemberInfo> MemberReturns()
		{
			foreach (Instruction inst in list)
			{
				if (inst.IsReturn)
				{
					yield return inst.GetLoadedMember();
				}
			}
		}

		public ParameterInfo ResolveArgument(int argIndex)
		{
			return method.GetParameters()[argIndex + 1];
		}

		public FieldInfo ResolveField(byte[] operandBytes)
		{
			int msilFieldToken = BitConverter.ToInt32(operandBytes, 0);

			//Re the way this code handles generics: see http://blogs.gotdotnet.com/yirutang/archive/2005/07/05/435556.aspx 
			Type declaringType = method.DeclaringType;
			return declaringType.Module.ResolveField(msilFieldToken, declaringType.GetGenericArguments(), method.GetGenericArguments());
			//TODO: just out of curiosity, how can this last param actually be necessary/ relevant?
		}

		public LocalVariableInfo ResolveLocal(int localIndex)
		{
			return body.LocalVariables[localIndex];
		}

		public MethodBase ResolveMethod(byte[] operandBytes)
		{
			int msilMethodToken = BitConverter.ToInt32(operandBytes, 0);

			//Re the way this code handles generics: that's by the 2nd and 3rd params to ResolveMethod
			//See http://blogs.gotdotnet.com/yirutang/archive/2005/07/05/435556.aspx 
			Type declaringType = method.DeclaringType;
			return declaringType.Module.ResolveMethod(msilMethodToken, declaringType.GetGenericArguments(), method.GetGenericArguments());
		}

		public string ResolveString(byte[] operandBytes)
		{
			int msilStringToken = BitConverter.ToInt32(operandBytes, 0);
			Type declaringType = method.DeclaringType;
			return declaringType.Module.ResolveString(msilStringToken);
		}

		public Type ResolveType(byte[] operandBytes)
		{
			int msilTypeToken = BitConverter.ToInt32(operandBytes, 0);

			Type declaringType = method.DeclaringType;
			return declaringType.Module.ResolveType(msilTypeToken, declaringType.GetGenericArguments(), method.GetGenericArguments());
		}

		public Expression ToExpression()
		{
			List<ParameterExpression> args = method.GetParameters()
				.Select(par => Expression.Parameter(par.ParameterType, par.Name))
				.ToList();

			if (!method.IsStatic)
			{
				args.Insert(0, Expression.Parameter(method.DeclaringType, "this"));
			}

			List<ParameterExpression> locals = body.LocalVariables
				.Select(loc => Expression.Variable(loc.LocalType, "loc" + loc.LocalIndex))
				.ToList();

			var targets = new Dictionary<int, LabelTarget>();

			LabelTarget blockExit = Expression.Label(method.ReturnType, "exit");

			Expression peek = null;
			LabelTarget lineLabel = null;
			var expressions = new List<Expression>();
			for (int index = 0; index < list.Count; index++)
			{
				Instruction instruction = list[index];

				LabelTarget instructionLabel;
				if (targets.TryGetValue(instruction.Index, out instructionLabel))
				{
					lineLabel = instructionLabel;
				}

				if (instruction.Code == OpCodes.Nop)
				{
					continue;
				}

				if (instruction.IsLoadArgument)
				{
					peek = args[instruction.GetOperandArgumentIndex()];
				}
				else if (instruction.IsLoadLocal)
				{
					peek = locals[instruction.GetOperandLocalIndex()];
				}
				else if (instruction.IsLoadInstanceField)
				{
					peek = Expression.Field(peek, instruction.GetOperandField());
				}
				else if (instruction.IsLoadStaticField)
				{
					peek = Expression.Field(null, instruction.GetOperandField());
				}
				else if (instruction.IsCall)
				{
					MethodBase methodBase = instruction.GetOperandMethod();
					peek = Expression.Call(methodBase.IsStatic ? null : peek, (MethodInfo)methodBase);
				}
				else if (instruction.IsSetArgument)
				{
					peek = Expression.Assign(locals[instruction.GetOperandLocalIndex()], peek);
				}
				else if (instruction.IsSetLocal)
				{
					peek = Expression.Assign(locals[instruction.GetOperandLocalIndex()], peek);
				}
				else if (instruction.IsSetField)
				{
					peek = Expression.Assign(Expression.Field(peek, instruction.GetOperandField()), peek);
				}
				else if ((OpCodes.Br_S == instruction.Code) || (OpCodes.Br == instruction.Code))
				{
					// End line statement
					if (null == peek)
					{
						throw new InvalidOperationException();
					}

					LabelTarget target;
					if (!targets.TryGetValue(instruction.Target.Index, out target))
					{
						target = Expression.Label(peek.Type, "L_" + instruction.Target.Index);
						targets.Add(instruction.Target.Index, target);
					}

					peek = Expression.Goto(target, peek);
					if (null != lineLabel)
					{
						peek = Expression.Label(lineLabel, peek);
					}

					expressions.Add(peek);
				}
				else if (instruction.IsReturn)
				{
					if (null == peek)
					{
						throw new InvalidOperationException();
					}

					peek = Expression.Return(blockExit, peek, peek.Type);
					if (null != lineLabel)
					{
						peek = Expression.Label(lineLabel, peek);
					}

					expressions.Add(peek);
				}
				else
				{
					throw new NotSupportedException("Not support " + instruction.Code);
				}
			}

			//args.AddRange(locals);
			return Expression.Label(blockExit, Expression.Block(method.ReturnType, args, expressions));
		}

		public IEnumerable<MemberInfo> UsedMembers()
		{
			foreach (Instruction inst in list)
			{
				MemberInfo member;
				if (inst.TryGetOperandMember(out  member))
				{
					yield return member;
				}
			}
		}

		private int Add(Instruction instruction)
		{
			list.Add(instruction);
			return list.Count - 1;
		}

		private Instruction Peek(int i)
		{
			return list[i];
		}

		/// <summary>
		/// Represents a single MSIL instruction, consisting of an Opcode and an Operand
		/// </summary>
		internal class Instruction
		{
			public static OpCode InvertLoadLocal(OpCode loadOpCode)
			{
				int index = LoadLocalOpCodes.IndexOf(loadOpCode);
				if (0 <= index)
				{
					return StoreLocalOpCodes[index];
				}

				return OpCodes.Nop;
			}

			private static readonly OpCode[] LoadLocalOpCodes =
			{
				OpCodes.Ldloc_0,
				OpCodes.Ldloc_1,
				OpCodes.Ldloc_2,
				OpCodes.Ldloc_3,
				OpCodes.Ldloc_S,
				OpCodes.Ldloc
			};

			private static readonly OpCode[] StoreLocalOpCodes =
			{
				OpCodes.Stloc_0,
				OpCodes.Stloc_1,
				OpCodes.Stloc_2,
				OpCodes.Stloc_3,
				OpCodes.Stloc_S,
				OpCodes.Stloc
			};

			private readonly CodeBlock block;

			private readonly OpCode code;

			private readonly int index;

			private readonly int offset;

			private readonly byte[] operandBytes;

			private Instruction target;

			public Instruction(CodeBlock block, OpCode code, byte[] operandBytes)
			{
				this.block = block;
				index = block.Add(this);
				this.code = code;
				this.operandBytes = operandBytes;
				if (0 != index)
				{
					Instruction previous = Peek(-1);
					offset = previous.offset + previous.Size;
				}
			}

			public override string ToString()
			{
				string operand = "?";
				if (OpCodes.Ldstr == code)
				{
					operand = block.ResolveString(operandBytes);
				}
				else if (OpCodes.Call == code)
				{
					operand = block.ResolveMethod(operandBytes).ToString();
				}
				else if ((OpCodes.Ldsfld == code) || (OpCodes.Ldflda == code) || (OpCodes.Stsfld == code))
				{
					operand = block.ResolveField(operandBytes).ToString();
				}

				return code + " " + operand;
			}

			public FieldInfo GetLoadedField()
			{
				Instruction origin = Peek(-1);
				if (OpCodes.Ldfld == origin.Code)
				{
					return GetOperandField();
				}

				// Пытаюсь инвертировать текущую иструкцию ldlocX --> stlocX
				OpCode targetCode = InvertLoadLocal(origin.Code);
				if (OpCodes.Nop != targetCode)
				{
					int targetIndex = origin.GetOperandLocalIndex();
					foreach (Instruction current in origin.Precedings)
					{
						if (current.IsConditionalBranch)
						{
							// Невозможно разобрать код с переходами
							break;
						}

						if (targetCode == current.Code
							&& targetIndex == current.GetOperandLocalIndex())
						{
							if (current.IsSetLocal)
							{
								Instruction copy;
								if (current.TryGetCopy(out copy))
								{
									if (copy.IsSetField)
									{
										return copy.GetOperandField();
									}
								}

								// Найдена инструкция stlocX

								Instruction source = current.Peek(-1);
								if (source.IsLoadInstanceField)
								{
									// Результирующая переменная
									// была установленна с этого поля
									return current.Peek(-1).GetOperandField();
								}

								if (source.IsLoadLocal)
								{
									// Результирующая переменная
									// была установленна с другой переменной.
									// Теперь придется искать её источник
									targetCode = InvertLoadLocal(source.Code);
									targetIndex = source.GetOperandLocalIndex();
								}
							}
						}
					}
				}

				return null;
			}

			public MemberInfo GetLoadedMember()
			{
				Instruction previous = Peek(-1);
				MemberInfo member;
				if (TryGetMember(previous, out member))
				{
					return member;
				}

				if (!previous.IsLoadLocal)
				{
					return null;
				}

				OpCode setLocalCode = InvertLoadLocal(previous.Code);
				while (true)
				{
					Instruction setLocal = previous.FindPreceding(setLocalCode);
					if (null == setLocal)
					{
						throw new InvalidOperationException();
					}

					if (!setLocal.OperandEquals(previous))
					{
						continue;
					}

					previous = setLocal.Peek(-1);
					if (previous.code == OpCodes.Nop)
					{
						previous = previous.Peek(-1);
					}

					if (previous.IsLoadLocal)
					{
						setLocalCode = InvertLoadLocal(previous.Code);
						continue;
					}

					if (previous.code == OpCodes.Dup)
					{
						previous = setLocal.Peek(1);
					}

					if (previous.code == OpCodes.Ldnull)
					{
						return null;
					}

					if (TryGetMember(previous, out member))
					{
						return member;
					}
				}
			}

			public ParameterInfo GetOperandArgument()
			{
				return block.ResolveArgument(GetOperandArgumentIndex());
			}

			public int GetOperandArgumentIndex()
			{
				if (OpCodes.Ldarg_0 == Code)
				{
					return 0;
				}

				if (OpCodes.Ldarg_1 == Code)
				{
					return 1;
				}

				if (OpCodes.Ldarg_2 == Code)
				{
					return 2;
				}

				if (OpCodes.Ldarg_3 == Code)
				{
					return 3;
				}

				if ((OpCodes.Ldarg_S == Code) || (OpCodes.Starg_S == Code) || (OpCodes.Ldarga_S == Code))
				{
					return GetOperandByte();
				}

				if ((OpCodes.Ldarg == Code) || (OpCodes.Starg == Code) || (OpCodes.Ldarga == Code))
				{
					return GetOperandInt16();
				}

				throw new ArgumentOutOfRangeException();
			}

			public byte GetOperandByte()
			{
				return operandBytes[0];
			}

			/// <summary>
			/// Returns the operand cast to a fieldInfo.  Only applicable to operands of ldfld(a) instructions, will
			/// throw an exception for all others.
			/// </summary>
			public FieldInfo GetOperandField()
			{
				CheckOpCode("GetOperandField is only supported for Ldfld(a)/Stfld opcodes",
					OpCodes.Ldfld, OpCodes.Ldflda, OpCodes.Stfld, /* instance */
					OpCodes.Ldsfld, OpCodes.Ldsflda, OpCodes.Stsfld /* static */);

				return block.ResolveField(operandBytes);
			}

			public short GetOperandInt16()
			{
				return BitConverter.ToInt16(operandBytes, 0);
			}

			public int GetOperandInt32()
			{
				return BitConverter.ToInt32(operandBytes, 0);
			}

			public long GetOperandInt64()
			{
				return BitConverter.ToInt64(operandBytes, 0);
			}

			public LocalVariableInfo GetOperandLocal()
			{
				return block.ResolveLocal(GetOperandLocalIndex());
			}

			public int GetOperandLocalIndex()
			{
				if ((OpCodes.Ldloc_0 == Code)
					|| (OpCodes.Stloc_0 == Code))
				{
					return 0;
				}

				if ((OpCodes.Ldloc_1 == Code)
					|| (OpCodes.Stloc_1 == Code))
				{
					return 1;
				}

				if ((OpCodes.Ldloc_2 == Code)
					|| (OpCodes.Stloc_2 == Code))
				{
					return 2;
				}

				if ((OpCodes.Ldloc_3 == Code)
					|| (OpCodes.Stloc_3 == Code))
				{
					return 3;
				}

				if ((OpCodes.Ldloc_S == Code)
					|| (OpCodes.Stloc_S == Code)
					|| (OpCodes.Ldloca_S == Code))
				{
					return GetOperandByte();
				}

				if ((OpCodes.Ldloc == Code)
					|| (OpCodes.Stloc == Code)
					|| (OpCodes.Ldloca == Code))
				{
					return GetOperandInt16();
				}

				throw new ArgumentOutOfRangeException();
			}

			/// <summary>
			/// Returns the operand cast to a method.  Only applicable to operands of call instructions, will
			/// throw an exception for all others.
			/// </summary>
			public MethodBase GetOperandMethod()
			{
				CheckOpCode("GetOperandMethod is only supported for Call opcode at present. Callvirt and Calli may be supported later",
					OpCodes.Call, OpCodes.Callvirt);
				return block.ResolveMethod(operandBytes);
			}

			/// <summary>
			/// Returns the operand case to a string.  Only applicable to operands of Ldstr instructions, will 
			/// throw an exception for all others
			/// </summary>
			public string GetOperandString()
			{
				CheckOpCode("GetOperandString is only supported for Ldstr opcode at present", OpCodes.Ldstr);
				return block.ResolveString(operandBytes);
			}

			public Instruction Peek(int relativeOffset)
			{
				return block.Peek(index + relativeOffset);
			}

			public bool TryGetOperandMember(out MemberInfo member)
			{
				switch (code.OperandType)
				{
					case OperandType.InlineField:
						member = block.ResolveField(operandBytes);
						return true;

					case OperandType.InlineMethod:
						member = block.ResolveMethod(operandBytes);
						return true;

					case OperandType.InlineTok:
					case OperandType.InlineType:
						member = block.ResolveType(operandBytes);
						return true;
				}

				member = null;
				return false;
			}

			private void CheckOpCode(string message, params OpCode[] requiredOpCodes)
			{
				if (Array.IndexOf(requiredOpCodes, code) < 0)
				{
					throw new InvalidOperationException(message);
				}
			}

			private Instruction FindPreceding(OpCode value)
			{
				return Precedings.Where(inst => inst.code == value).FirstOrDefault();
			}

			private bool OperandEquals(Instruction other)
			{
				if (operandBytes.Length == other.operandBytes.Length)
				{
					return !operandBytes.Where((op, i) => op != other.operandBytes[i]).Any();
				}

				return false;
			}

			private bool TryGetCopy(out Instruction copy)
			{
				copy = null;
				if ((1 < index && OpCodes.Dup == Peek(-1).code))
				{
					copy = Peek(1);
				}
				else if ((2 < index && OpCodes.Dup == Peek(-2).code))
				{
					copy = Peek(-1);
				}

				return null != copy;
			}

			private bool TryGetMember(Instruction instruction, out MemberInfo member)
			{
				if ((OpCodes.Ldfld == instruction.Code)
					|| (OpCodes.Ldflda == instruction.Code)
					|| (OpCodes.Ldsfld == instruction.Code)
					|| (OpCodes.Stfld == instruction.Code)
					|| (OpCodes.Stfld == instruction.Code)
					|| (OpCodes.Stfld == instruction.Code))
				{
					member = instruction.GetOperandField();
					return true;
				}

				if ((OpCodes.Call == instruction.Code)
					|| (OpCodes.Callvirt == instruction.Code))
				{
					member = instruction.GetOperandMethod();
					return true;
				}

				member = null;
				return false;
			}

			public OpCode Code
			{
				get { return code; }
			}

			public int Index
			{
				get { return index; }
			}

			public bool IsBranch
			{
				// ?? get { return (FlowControl.Branch == this.code.FlowControl); }
				get { return IsBranchShort || IsBranchLong; }
			}

			public bool IsBranchLong
			{
				get
				{
					return (OpCodes.Br == code)
						   || (OpCodes.Brtrue == code)
						   || (OpCodes.Brfalse == code);
				}
			}

			public bool IsBranchShort
			{
				get
				{
					return (OpCodes.Br_S == code)
						   || (OpCodes.Brtrue_S == code)
						   || (OpCodes.Brfalse_S == code);
				}
			}

			public bool IsCall
			{
				get
				{
					return (OpCodes.Call == code)
						   || (OpCodes.Calli == code)
						   || (OpCodes.Callvirt == code);
				}
			}

			public bool IsConditionalBranch
			{
				get { return (FlowControl.Cond_Branch == code.FlowControl); }
			}

			public bool IsLoadArgument
			{
				get
				{
					return (OpCodes.Ldarg_0 == code)
						   || (OpCodes.Ldarg_1 == code)
						   || (OpCodes.Ldarg_2 == code)
						   || (OpCodes.Ldarg_3 == code)
						   || (OpCodes.Ldarg_S == code)
						   || (OpCodes.Ldarga_S == code)
						   || (OpCodes.Ldarg == code);
				}
			}

			public bool IsLoadFieldByRef
			{
				get { return OpCodes.Ldflda == code /*&& OpCodes.Ldarg_0 == this.Peek(-1).Code*/; }
			}

			public bool IsLoadInstanceField
			{
				get
				{
					return OpCodes.Ldfld == code
						   || OpCodes.Ldflda == code;
				}
			}

			public bool IsLoadLocal
			{
				get
				{
					return (OpCodes.Ldloc_0 == code)
						   || (OpCodes.Ldloc_1 == code)
						   || (OpCodes.Ldloc_2 == code)
						   || (OpCodes.Ldloc_3 == code)
						   || (OpCodes.Ldloc_S == code)
						   || (OpCodes.Ldloca_S == code)
						   || (OpCodes.Ldloc == code);
				}
			}

			public bool IsLoadStaticField
			{
				get
				{
					return (OpCodes.Ldsfld == code)
						   || (OpCodes.Ldsflda == code);
				}
			}

			public bool IsReturn
			{
				get { return OpCodes.Ret == code; }
			}

			public bool IsSetArgument { get { return (OpCodes.Starg_S == code) || (OpCodes.Starg == code); } }

			public bool IsSetField
			{
				get { return OpCodes.Stfld == code/* && OpCodes.Ldarg_1 == this.Peek(-1).Code*/; }
			}

			public bool IsSetLocal
			{
				get
				{
					return (OpCodes.Stloc_0 == code)
						   || (OpCodes.Stloc_1 == code)
						   || (OpCodes.Stloc_2 == code)
						   || (OpCodes.Stloc_3 == code)
						   || (OpCodes.Stloc_S == code)
						   || (OpCodes.Stloc == code);
				}
			}

			public int Offset
			{
				get { return offset; }
			}

			public Instruction Target
			{
				get
				{
					if ((null == target) && IsBranch)
					{
						int targetOffset = IsBranchLong ? GetOperandInt32() : GetOperandByte();

						targetOffset += (Offset + Size);
						foreach (Instruction instruction in block.list)
						{
							if (instruction.Offset == targetOffset)
							{
								target = instruction;
								break;
							}
						}
					}

					return target;
				}
			}

			protected IEnumerable<Instruction> Precedings
			{
				get
				{
					for (int i = index - 1; 0 < i; i--)
					{
						yield return block.Peek(i);
					}
				}
			}

			protected int Size
			{
				get { return code.Size + operandBytes.Length; }
			}

			//public static OpCode InvertSetLocal(OpCode loadOpCode)
			//{
			//    int index = StoreLocalOpCodes.IndexOf(loadOpCode);
			//    if (0 <= index)
			//    {
			//        return LoadLocalOpCodes[index];
			//    }

			//    return OpCodes.Nop;
			//}
		}
	}

	/// <summary>
	/// Lists of opcodes, indexed by OpCode.Value, and grouped into lists for 1 byte and 2 byte opcodes.
	/// </summary>
	/// <remarks>As of .NET 2.0, 4 byte opcodes are listed as a future possibility in the ECMA standard, but are not yet 
	/// present in the .NET Framework, so we don't accommodate them there.</remarks>
	internal static class OpCodeLists
	{
		public static readonly OpCode?[] DoubleByte = new OpCode?[0x100];

		public static readonly OpCode?[] SingleByte = new OpCode?[0x100];

		/// <summary>
		/// Create the lists, by reflecting over <see cref="OpCodes"/>, and indexing all members, by <see cref="OpCode.Value"/>
		/// </summary>
		static OpCodeLists()
		{
			foreach (FieldInfo fi in typeof(OpCodes)
				.GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var opCode = (OpCode)fi.GetValue(null);
				var value = (ushort)opCode.Value;
				if (value < 0x100)
				{
					SingleByte[value] = opCode;
				}
				else if ((value & 0xff00) == 0xfe00) //all 2 byte codes begin with FE
				{
					DoubleByte[value & 0xff] = opCode; //index by lower byte only
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}
	}
}