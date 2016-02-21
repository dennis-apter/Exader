using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Exader.Reflection
{
    public class MethodBodyReader : IEnumerable<Instruction>
    {
        static MethodBodyReader()
        {
            OneByteOpcodes = new OpCode[0xe1];
            TwoBytesOpcodes = new OpCode[0x1f];

            FieldInfo[] fields = GetOpCodeFields();

            for (int i = 0; i < fields.Length; i++)
            {
                var opcode = (OpCode)fields[i].GetValue(null);
                if (opcode.OpCodeType == OpCodeType.Nternal)
                {
                    continue;
                }

                if (opcode.Size == 1)
                {
                    OneByteOpcodes[opcode.Value] = opcode;
                }
                else
                {
                    TwoBytesOpcodes[opcode.Value & 0xff] = opcode;
                }
            }
        }

        private static FieldInfo[] GetOpCodeFields()
        {
            return typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        }

        private static bool TargetsLocalVariable(OpCode opcode)
        {
            return opcode.Name.Contains("loc");
        }

        private static readonly OpCode[] OneByteOpcodes;

        private static readonly OpCode[] TwoBytesOpcodes;
        private readonly MethodBody body;
        private readonly ByteBuffer il;
        private readonly IList<LocalVariableInfo> locals;
        private readonly MethodBase method;
        private readonly Type[] methodArguments;
        private readonly Module module;
        private readonly ParameterInfo[] parameters;
        private readonly Type[] typeArguments;

        public MethodBodyReader(MethodBase method)
        {
            this.method = method;

            body = method.GetMethodBody();
            if (null == body)
            {
                throw new ArgumentException();
            }

            byte[] bytes = body.GetILAsByteArray();
            if (null == bytes)
            {
                throw new ArgumentException();
            }

            if (!(method is ConstructorInfo))
            {
                methodArguments = method.GetGenericArguments();
            }

            typeArguments = method.DeclaringType.GetGenericArguments();

            parameters = method.GetParameters();
            locals = body.LocalVariables;
            module = method.Module;
            il = new ByteBuffer(bytes);
        }

        public IEnumerable<MemberInfo> ReadUsedMembers(bool throwsOnError = false)
        {
            while (il.Position < il.Buffer.Length)
            {
                OpCode code = ReadOpCode();
                MemberInfo memberInfo;
                if (TryReadOperandMember(code, out memberInfo))
                {
                    yield return memberInfo;
                }
                else if (throwsOnError)
                {
                    throw new NotSupportedException("Cannot resolve op code: " + code);
                }
            }
        }

        private LocalVariableInfo GetLocalVariable(int index)
        {
            return locals[index];
        }

        private ParameterInfo GetParameter(int index)
        {
            if (!method.IsStatic)
            {
                index--;
            }

            return parameters[index];
        }

        private object GetVariable(OpCode code, int index)
        {
            if (TargetsLocalVariable(code))
            {
                return GetLocalVariable(index);
            }

            return GetParameter(index);
        }

        private OpCode ReadOpCode()
        {
            byte op = il.ReadByte();
            return op != 0xfe
                ? OneByteOpcodes[op]
                : TwoBytesOpcodes[il.ReadByte()];
        }

        private bool TryReadOperand(OpCode opCode, out object operand)
        {
            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    operand = null;
                    break;
                case OperandType.InlineSwitch:
                    int length = il.ReadInt32();
                    var branches = new int[length];
                    var offsets = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        offsets[i] = il.ReadInt32();
                    }

                    for (int i = 0; i < length; i++)
                    {
                        branches[i] = il.Position + offsets[i];
                    }

                    operand = branches;
                    break;

                case OperandType.ShortInlineBrTarget:
                    operand = (sbyte)(il.ReadByte() + il.Position);
                    break;
                case OperandType.InlineBrTarget:
                    operand = il.ReadInt32() + il.Position;
                    break;
                case OperandType.ShortInlineI:
                    if (opCode == OpCodes.Ldc_I4_S)
                    {
                        operand = (sbyte)il.ReadByte();
                    }
                    else
                    {
                        operand = il.ReadByte();
                    }
                    break;
                case OperandType.InlineI:
                    operand = il.ReadInt32();
                    break;
                case OperandType.ShortInlineR:
                    operand = il.ReadSingle();
                    break;
                case OperandType.InlineR:
                    operand = il.ReadDouble();
                    break;
                case OperandType.InlineI8:
                    operand = il.ReadInt64();
                    break;
                case OperandType.InlineSig:
                    operand = module.ResolveSignature(il.ReadInt32());
                    break;
                case OperandType.InlineString:
                    operand = module.ResolveString(il.ReadInt32());
                    break;
                case OperandType.ShortInlineVar:
                    operand = GetVariable(opCode, il.ReadByte());
                    break;
                case OperandType.InlineVar:
                    operand = GetVariable(opCode, il.ReadInt16());
                    break;
                default:
                    MemberInfo member;
                    operand = TryReadOperandMember(opCode, out member)
                        ? member
                        : null;
                    break;
            }

            return true;
        }

        private bool TryReadOperandMember(OpCode opCode, out MemberInfo operand)
        {
            switch (opCode.OperandType)
            {
                case OperandType.InlineTok:
                    operand = module.ResolveMember(il.ReadInt32(), typeArguments, methodArguments);
                    break;
                case OperandType.InlineType:
                    operand = module.ResolveType(il.ReadInt32(), typeArguments, methodArguments);
                    break;
                case OperandType.InlineMethod:
                    operand = module.ResolveMethod(il.ReadInt32(), typeArguments, methodArguments);
                    break;
                case OperandType.InlineField:
                    operand = module.ResolveField(il.ReadInt32(), typeArguments, methodArguments);
                    break;
                default:
                    operand = null;
                    return false;
            }

            return true;
        }

        #region IEnumerable<Instruction> Members

        public IEnumerator<Instruction> GetEnumerator()
        {
            Instruction previous = null;
            while (il.Position < il.Buffer.Length)
            {
                OpCode code = ReadOpCode();
                object operand;
                if (TryReadOperand(code, out operand))
                {
                    var instruction = new Instruction(il.Position, code, operand);
                    if (previous != null)
                    {
                        instruction.Previous = previous;
                        previous.Next = instruction;
                    }

                    yield return instruction;

                    previous = instruction;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public MethodBody Body
        {
            get { return body; }
        }

        private class ByteBuffer
        {
            public readonly byte[] Buffer;

            public int Position;

            public ByteBuffer(byte[] buffer)
            {
                Buffer = buffer;
            }

            public byte ReadByte()
            {
                CheckCanRead(1);
                return Buffer[Position++];
            }

            public double ReadDouble()
            {
                if (!BitConverter.IsLittleEndian)
                {
                    byte[] bytes = ReadBytes(8);
                    Array.Reverse(bytes);
                    return BitConverter.ToDouble(bytes, 0);
                }

                CheckCanRead(8);
                double value = BitConverter.ToDouble(Buffer, Position);
                Position += 8;
                return value;
            }

            public short ReadInt16()
            {
                CheckCanRead(2);
                var value = (short)(Buffer[Position]
                    | (Buffer[Position + 1] << 8));
                Position += 2;
                return value;
            }

            public int ReadInt32()
            {
                CheckCanRead(4);
                int value = Buffer[Position]
                    | (Buffer[Position + 1] << 8)
                        | (Buffer[Position + 2] << 16)
                            | (Buffer[Position + 3] << 24);

                Position += 4;
                return value;
            }

            public long ReadInt64()
            {
                CheckCanRead(8);
                var low = (uint)(Buffer[Position]
                    | (Buffer[Position + 1] << 8)
                        | (Buffer[Position + 2] << 16)
                            | (Buffer[Position + 3] << 24));

                var high = (uint)(Buffer[Position + 4]
                    | (Buffer[Position + 5] << 8)
                        | (Buffer[Position + 6] << 16)
                            | (Buffer[Position + 7] << 24));

                long value = (((long)high) << 32) | low;
                Position += 8;
                return value;
            }

            public float ReadSingle()
            {
                if (!BitConverter.IsLittleEndian)
                {
                    byte[] bytes = ReadBytes(4);
                    Array.Reverse(bytes);
                    return BitConverter.ToSingle(bytes, 0);
                }

                CheckCanRead(4);
                float value = BitConverter.ToSingle(Buffer, Position);
                Position += 4;
                return value;
            }

            void CheckCanRead(int count)
            {
                if (Buffer.Length < (Position + count))
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            private byte[] ReadBytes(int length)
            {
                CheckCanRead(length);
                var value = new byte[length];
                System.Buffer.BlockCopy(Buffer, Position, value, 0, length);
                Position += length;
                return value;
            }
        }
    }
}