using System.Reflection.Emit;

namespace Exader.Reflection
{
    public sealed class Instruction
    {
        private readonly OpCode code;

        private readonly int offset;

        private readonly object operand;

        internal Instruction(int offset, OpCode code, object operand)
        {
            this.offset = offset;
            this.code = code;
            this.operand = operand;
        }

        public override string ToString()
        {
            return code.Name;
        }

        public int GetSize()
        {
            int size = code.Size;

            switch (code.OperandType)
            {
                case OperandType.InlineSwitch:
                    size += (1 + ((int[])operand).Length) * 4;
                    break;
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    size += 8;
                    break;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    size += 4;
                    break;
                case OperandType.InlineVar:
                    size += 2;
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    size += 1;
                    break;
            }

            return size;
        }

        public OpCode Code
        {
            get { return code; }
        }

        public bool IsBranch
        {
            get { return code.FlowControl == FlowControl.Branch; }
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
            get { return code.FlowControl == FlowControl.Cond_Branch; }
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

        public bool IsLoadInstanceField
        {
            get
            {
                return (OpCodes.Ldfld == code) || (OpCodes.Ldflda == code);
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
                return (OpCodes.Ldsfld == code) || (OpCodes.Ldsflda == code);
            }
        }

        public bool IsReturn
        {
            get { return OpCodes.Ret == code; }
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

        public Instruction Next
        {
            get;
            internal set;
        }

        public int Offset
        {
            get { return offset; }
        }

        public object Operand
        {
            get { return operand; }
        }

        public Instruction Previous
        {
            get;
            internal set;
        }
    }
}
