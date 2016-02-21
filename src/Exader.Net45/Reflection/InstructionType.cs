// ReSharper disable InconsistentNaming
namespace Exader.Reflection
{
    public enum InstructionType : short
    {
        #region InlineNone
        /// <summary>[InlineNone;Next] Fills space if opcodes are patched. No meaningful operation is performed although a processing cycle can be consumed.</summary>
        Nop = 0 /*0x0*/,

        /// <summary>[InlineNone;Break] Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped.</summary>
        Break = 1 /*0x1*/,

        /// <summary>[InlineNone;Next] Loads the argument at index 0 onto the evaluation stack.</summary>
        Ldarg_0 = 2 /*0x2*/,

        /// <summary>[InlineNone;Next] Loads the argument at index 1 onto the evaluation stack.</summary>
        Ldarg_1 = 3 /*0x3*/,

        /// <summary>[InlineNone;Next] Loads the argument at index 2 onto the evaluation stack.</summary>
        Ldarg_2 = 4 /*0x4*/,

        /// <summary>[InlineNone;Next] Loads the argument at index 3 onto the evaluation stack.</summary>
        Ldarg_3 = 5 /*0x5*/,

        /// <summary>[InlineNone;Next] Loads the local variable at index 0 onto the evaluation stack.</summary>
        Ldloc_0 = 6 /*0x6*/,

        /// <summary>[InlineNone;Next] Loads the local variable at index 1 onto the evaluation stack.</summary>
        Ldloc_1 = 7 /*0x7*/,

        /// <summary>[InlineNone;Next] Loads the local variable at index 2 onto the evaluation stack.</summary>
        Ldloc_2 = 8 /*0x8*/,

        /// <summary>[InlineNone;Next] Loads the local variable at index 3 onto the evaluation stack.</summary>
        Ldloc_3 = 9 /*0x9*/,

        /// <summary>[InlineNone;Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 0.</summary>
        Stloc_0 = 10 /*0xA*/,

        /// <summary>[InlineNone;Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 1.</summary>
        Stloc_1 = 11 /*0xB*/,

        /// <summary>[InlineNone;Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 2.</summary>
        Stloc_2 = 12 /*0xC*/,

        /// <summary>[InlineNone;Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 3.</summary>
        Stloc_3 = 13 /*0xD*/,

        /// <summary>[InlineNone;Next] Pushes a null reference (type O) onto the evaluation stack.</summary>
        Ldnull = 20 /*0x14*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of -1 onto the evaluation stack as an int32.</summary>
        Ldc_I4_M1 = 21 /*0x15*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 0 onto the evaluation stack as an int32.</summary>
        Ldc_I4_0 = 22 /*0x16*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 1 onto the evaluation stack as an int32.</summary>
        Ldc_I4_1 = 23 /*0x17*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 2 onto the evaluation stack as an int32.</summary>
        Ldc_I4_2 = 24 /*0x18*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 3 onto the evaluation stack as an int32.</summary>
        Ldc_I4_3 = 25 /*0x19*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 4 onto the evaluation stack as an int32.</summary>
        Ldc_I4_4 = 26 /*0x1A*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 5 onto the evaluation stack as an int32.</summary>
        Ldc_I4_5 = 27 /*0x1B*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 6 onto the evaluation stack as an int32.</summary>
        Ldc_I4_6 = 28 /*0x1C*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 7 onto the evaluation stack as an int32.</summary>
        Ldc_I4_7 = 29 /*0x1D*/,

        /// <summary>[InlineNone;Next] Pushes the integer value of 8 onto the evaluation stack as an int32.</summary>
        Ldc_I4_8 = 30 /*0x1E*/,

        /// <summary>[InlineNone;Next] Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.</summary>
        Dup = 37 /*0x25*/,

        /// <summary>[InlineNone;Next] Removes the value currently on top of the evaluation stack.</summary>
        Pop = 38 /*0x26*/,

        /// <summary>[InlineNone;Return] Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.</summary>
        Ret = 42 /*0x2A*/,

        /// <summary>[InlineNone;Next] Loads a value of type int8 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_I1 = 70 /*0x46*/,

        /// <summary>[InlineNone;Next] Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_U1 = 71 /*0x47*/,

        /// <summary>[InlineNone;Next] Loads a value of type int16 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_I2 = 72 /*0x48*/,

        /// <summary>[InlineNone;Next] Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_U2 = 73 /*0x49*/,

        /// <summary>[InlineNone;Next] Loads a value of type int32 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_I4 = 74 /*0x4A*/,

        /// <summary>[InlineNone;Next] Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_U4 = 75 /*0x4B*/,

        /// <summary>[InlineNone;Next] Loads a value of type int64 as an int64 onto the evaluation stack indirectly.</summary>
        Ldind_I8 = 76 /*0x4C*/,

        /// <summary>[InlineNone;Next] Loads a value of type native int as a native int onto the evaluation stack indirectly.</summary>
        Ldind_I = 77 /*0x4D*/,

        /// <summary>[InlineNone;Next] Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.</summary>
        Ldind_R4 = 78 /*0x4E*/,

        /// <summary>[InlineNone;Next] Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.</summary>
        Ldind_R8 = 79 /*0x4F*/,

        /// <summary>[InlineNone;Next] Loads an object reference as a type O (object reference) onto the evaluation stack indirectly.</summary>
        Ldind_Ref = 80 /*0x50*/,

        /// <summary>[InlineNone;Next] Stores a object reference value at a supplied address.</summary>
        Stind_Ref = 81 /*0x51*/,

        /// <summary>[InlineNone;Next] Stores a value of type int8 at a supplied address.</summary>
        Stind_I1 = 82 /*0x52*/,

        /// <summary>[InlineNone;Next] Stores a value of type int16 at a supplied address.</summary>
        Stind_I2 = 83 /*0x53*/,

        /// <summary>[InlineNone;Next] Stores a value of type int32 at a supplied address.</summary>
        Stind_I4 = 84 /*0x54*/,

        /// <summary>[InlineNone;Next] Stores a value of type int64 at a supplied address.</summary>
        Stind_I8 = 85 /*0x55*/,

        /// <summary>[InlineNone;Next] Stores a value of type float32 at a supplied address.</summary>
        Stind_R4 = 86 /*0x56*/,

        /// <summary>[InlineNone;Next] Stores a value of type float64 at a supplied address.</summary>
        Stind_R8 = 87 /*0x57*/,

        /// <summary>[InlineNone;Next] Adds two values and pushes the result onto the evaluation stack.</summary>
        Add = 88 /*0x58*/,

        /// <summary>[InlineNone;Next] Subtracts one value from another and pushes the result onto the evaluation stack.</summary>
        Sub = 89 /*0x59*/,

        /// <summary>[InlineNone;Next] Multiplies two values and pushes the result on the evaluation stack.</summary>
        Mul = 90 /*0x5A*/,

        /// <summary>[InlineNone;Next] Divides two values and pushes the result as a floating-point (type F) or quotient (type int32) onto the evaluation stack.</summary>
        Div = 91 /*0x5B*/,

        /// <summary>[InlineNone;Next] Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.</summary>
        Div_Un = 92 /*0x5C*/,

        /// <summary>[InlineNone;Next] Divides two values and pushes the remainder onto the evaluation stack.</summary>
        Rem = 93 /*0x5D*/,

        /// <summary>[InlineNone;Next] Divides two unsigned values and pushes the remainder onto the evaluation stack.</summary>
        Rem_Un = 94 /*0x5E*/,

        /// <summary>[InlineNone;Next] Computes the bitwise AND of two values and pushes the result onto the evaluation stack.</summary>
        And = 95 /*0x5F*/,

        /// <summary>[InlineNone;Next] Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack.</summary>
        Or = 96 /*0x60*/,

        /// <summary>[InlineNone;Next] Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result onto the evaluation stack.</summary>
        Xor = 97 /*0x61*/,

        /// <summary>[InlineNone;Next] Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the result onto the evaluation stack.</summary>
        Shl = 98 /*0x62*/,

        /// <summary>[InlineNone;Next] Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result onto the evaluation stack.</summary>
        Shr = 99 /*0x63*/,

        /// <summary>[InlineNone;Next] Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits, pushing the result onto the evaluation stack.</summary>
        Shr_Un = 100 /*0x64*/,

        /// <summary>[InlineNone;Next] Negates a value and pushes the result onto the evaluation stack.</summary>
        Neg = 101 /*0x65*/,

        /// <summary>[InlineNone;Next] Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.</summary>
        Not = 102 /*0x66*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.</summary>
        Conv_I1 = 103 /*0x67*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.</summary>
        Conv_I2 = 104 /*0x68*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to int32.</summary>
        Conv_I4 = 105 /*0x69*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to int64.</summary>
        Conv_I8 = 106 /*0x6A*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to float32.</summary>
        Conv_R4 = 107 /*0x6B*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to float64.</summary>
        Conv_R8 = 108 /*0x6C*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.</summary>
        Conv_U4 = 109 /*0x6D*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.</summary>
        Conv_U8 = 110 /*0x6E*/,

        /// <summary>[InlineNone;Next] Converts the unsigned integer value on top of the evaluation stack to float32.</summary>
        Conv_R_Un = 118 /*0x76*/,

        /// <summary>[InlineNone;Throw] Throws the exception object currently on the evaluation stack.</summary>
        Throw = 122 /*0x7A*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I1_Un = 130 /*0x82*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I2_Un = 131 /*0x83*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to signed int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I4_Un = 132 /*0x84*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to signed int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I8_Un = 133 /*0x85*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U1_Un = 134 /*0x86*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U2_Un = 135 /*0x87*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U4_Un = 136 /*0x88*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U8_Un = 137 /*0x89*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to signed native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I_Un = 138 /*0x8A*/,

        /// <summary>[InlineNone;Next] Converts the unsigned value on top of the evaluation stack to unsigned native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U_Un = 139 /*0x8B*/,

        /// <summary>[InlineNone;Next] Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.</summary>
        Ldlen = 142 /*0x8E*/,

        /// <summary>[InlineNone;Next] Loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_I1 = 144 /*0x90*/,

        /// <summary>[InlineNone;Next] Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_U1 = 145 /*0x91*/,

        /// <summary>[InlineNone;Next] Loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_I2 = 146 /*0x92*/,

        /// <summary>[InlineNone;Next] Loads the element with type unsigned int16 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_U2 = 147 /*0x93*/,

        /// <summary>[InlineNone;Next] Loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_I4 = 148 /*0x94*/,

        /// <summary>[InlineNone;Next] Loads the element with type unsigned int32 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_U4 = 149 /*0x95*/,

        /// <summary>[InlineNone;Next] Loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64.</summary>
        Ldelem_I8 = 150 /*0x96*/,

        /// <summary>[InlineNone;Next] Loads the element with type native int at a specified array index onto the top of the evaluation stack as a native int.</summary>
        Ldelem_I = 151 /*0x97*/,

        /// <summary>[InlineNone;Next] Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float).</summary>
        Ldelem_R4 = 152 /*0x98*/,

        /// <summary>[InlineNone;Next] Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float).</summary>
        Ldelem_R8 = 153 /*0x99*/,

        /// <summary>[InlineNone;Next] Loads the element containing an object reference at a specified array index onto the top of the evaluation stack as type O (object reference).</summary>
        Ldelem_Ref = 154 /*0x9A*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the native int value on the evaluation stack.</summary>
        Stelem_I = 155 /*0x9B*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the int8 value on the evaluation stack.</summary>
        Stelem_I1 = 156 /*0x9C*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the int16 value on the evaluation stack.</summary>
        Stelem_I2 = 157 /*0x9D*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the int32 value on the evaluation stack.</summary>
        Stelem_I4 = 158 /*0x9E*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the int64 value on the evaluation stack.</summary>
        Stelem_I8 = 159 /*0x9F*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the float32 value on the evaluation stack.</summary>
        Stelem_R4 = 160 /*0xA0*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the float64 value on the evaluation stack.</summary>
        Stelem_R8 = 161 /*0xA1*/,

        /// <summary>[InlineNone;Next] Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.</summary>
        Stelem_Ref = 162 /*0xA2*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to signed int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I1 = 179 /*0xB3*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U1 = 180 /*0xB4*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to signed int16 and extending it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I2 = 181 /*0xB5*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U2 = 182 /*0xB6*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to signed int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I4 = 183 /*0xB7*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to unsigned int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U4 = 184 /*0xB8*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to signed int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I8 = 185 /*0xB9*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to unsigned int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U8 = 186 /*0xBA*/,

        /// <summary>[InlineNone;Next] Throws <see cref="T:System.ArithmeticException" /> if value is not a finite number.</summary>
        Ckfinite = 195 /*0xC3*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.</summary>
        Conv_U2 = 209 /*0xD1*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.</summary>
        Conv_U1 = 210 /*0xD2*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to native int.</summary>
        Conv_I = 211 /*0xD3*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to signed native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I = 212 /*0xD4*/,

        /// <summary>[InlineNone;Next] Converts the signed value on top of the evaluation stack to unsigned native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U = 213 /*0xD5*/,

        /// <summary>[InlineNone;Next] Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Add_Ovf = 214 /*0xD6*/,

        /// <summary>[InlineNone;Next] Adds two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Add_Ovf_Un = 215 /*0xD7*/,

        /// <summary>[InlineNone;Next] Multiplies two integer values, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Mul_Ovf = 216 /*0xD8*/,

        /// <summary>[InlineNone;Next] Multiplies two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Mul_Ovf_Un = 217 /*0xD9*/,

        /// <summary>[InlineNone;Next] Subtracts one integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Sub_Ovf = 218 /*0xDA*/,

        /// <summary>[InlineNone;Next] Subtracts one unsigned integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Sub_Ovf_Un = 219 /*0xDB*/,

        /// <summary>[InlineNone;Return] Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.</summary>
        Endfinally = 220 /*0xDC*/,

        /// <summary>[InlineNone;Next] Stores a value of type native int at a supplied address.</summary>
        Stind_I = 223 /*0xDF*/,

        /// <summary>[InlineNone;Next] Converts the value on top of the evaluation stack to unsigned native int, and extends it to native int.</summary>
        Conv_U = 224 /*0xE0*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix7 = 248 /*0xF8*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix6 = 249 /*0xF9*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix5 = 250 /*0xFA*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix4 = 251 /*0xFB*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix3 = 252 /*0xFC*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix2 = 253 /*0xFD*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefix1 = 254 /*0xFE*/,

        /// <summary>[InlineNone;Meta] This is a reserved instruction.</summary>
        Prefixref = 255 /*0xFF*/,

        /// <summary>[InlineNone;Next] Returns an unmanaged pointer to the argument list of the current method.</summary>
        Arglist = -512 /*0xFE00*/,

        /// <summary>[InlineNone;Next] Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Ceq = -511 /*0xFE01*/,

        /// <summary>[InlineNone;Next] Compares two values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Cgt = -510 /*0xFE02*/,

        /// <summary>[InlineNone;Next] Compares two unsigned or unordered values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Cgt_Un = -509 /*0xFE03*/,

        /// <summary>[InlineNone;Next] Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Clt = -508 /*0xFE04*/,

        /// <summary>[InlineNone;Next] Compares the unsigned or unordered values <paramref name="value1" /> and <paramref name="value2" />. If <paramref name="value1" /> is less than <paramref name="value2" />, then the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Clt_Un = -507 /*0xFE05*/,

        /// <summary>[InlineNone;Next] Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (a transient pointer, type *) of the first allocated byte onto the evaluation stack.</summary>
        Localloc = -497 /*0xFE0F*/,

        /// <summary>[InlineNone;Return] Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.</summary>
        Endfilter = -495 /*0xFE11*/,

        /// <summary>[InlineNone;Meta] Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.</summary>
        Volatile = -493 /*0xFE13*/,

        /// <summary>[InlineNone;Meta] Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.</summary>
        Tailcall = -492 /*0xFE14*/,

        /// <summary>[InlineNone;Next] Copies a specified number bytes from a source address to a destination address.</summary>
        Cpblk = -489 /*0xFE17*/,

        /// <summary>[InlineNone;Next] Initializes a specified block of memory at a specific address to a given size and initial value.</summary>
        Initblk = -488 /*0xFE18*/,

        /// <summary>[InlineNone;Throw] Rethrows the current exception.</summary>
        Rethrow = -486 /*0xFE1A*/,

        /// <summary>[InlineNone;Next] Retrieves the type token embedded in a typed reference.</summary>
        Refanytype = -483 /*0xFE1D*/,

        /// <summary>[InlineNone;Meta] Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.</summary>
        Readonly = -482 /*0xFE1E*/,
        #endregion

        #region ShortInlineVar
        /// <summary>[ShortInlineVar;Next] Loads the argument (referenced by a specified short form index) onto the evaluation stack.</summary>
        Ldarg_S = 14 /*0xE*/,

        /// <summary>[ShortInlineVar;Next] Load an argument address, in short form, onto the evaluation stack.</summary>
        Ldarga_S = 15 /*0xF*/,

        /// <summary>[ShortInlineVar;Next] Stores the value on top of the evaluation stack in the argument slot at a specified index, short form.</summary>
        Starg_S = 16 /*0x10*/,

        /// <summary>[ShortInlineVar;Next] Loads the local variable at a specific index onto the evaluation stack, short form.</summary>
        Ldloc_S = 17 /*0x11*/,

        /// <summary>[ShortInlineVar;Next] Loads the address of the local variable at a specific index onto the evaluation stack, short form.</summary>
        Ldloca_S = 18 /*0x12*/,

        /// <summary>[ShortInlineVar;Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at <paramref name="index" /> (short form).</summary>
        Stloc_S = 19 /*0x13*/,
        #endregion

        #region ShortInlineI
        /// <summary>[ShortInlineI;Next] Pushes the supplied int8 value onto the evaluation stack as an int32, short form.</summary>
        Ldc_I4_S = 31 /*0x1F*/,

        /// <summary>[ShortInlineI;Meta] Indicates that an address currently atop the evaluation stack might not be aligned to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk instruction.</summary>
        Unaligned = -494 /*0xFE12*/,
        #endregion

        #region InlineI
        /// <summary>[InlineI;Next] Pushes a supplied value of type int32 onto the evaluation stack as an int32.</summary>
        Ldc_I4 = 32 /*0x20*/,
        #endregion

        #region InlineI8
        /// <summary>[InlineI8;Next] Pushes a supplied value of type int64 onto the evaluation stack as an int64.</summary>
        Ldc_I8 = 33 /*0x21*/,
        #endregion

        #region ShortInlineR
        /// <summary>[ShortInlineR;Next] Pushes a supplied value of type float32 onto the evaluation stack as type F (float).</summary>
        Ldc_R4 = 34 /*0x22*/,
        #endregion

        #region InlineR
        /// <summary>[InlineR;Next] Pushes a supplied value of type float64 onto the evaluation stack as type F (float).</summary>
        Ldc_R8 = 35 /*0x23*/,
        #endregion

        #region InlineMethod
        /// <summary>[InlineMethod;Call] Exits current method and jumps to specified method.</summary>
        Jmp = 39 /*0x27*/,

        /// <summary>[InlineMethod;Call] Calls the method indicated by the passed method descriptor.</summary>
        Call = 40 /*0x28*/,

        /// <summary>[InlineMethod;Call] Calls a late-bound method on an object, pushing the return value onto the evaluation stack.</summary>
        Callvirt = 111 /*0x6F*/,

        /// <summary>[InlineMethod;Call] Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.</summary>
        Newobj = 115 /*0x73*/,

        /// <summary>[InlineMethod;Next] Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.</summary>
        Ldftn = -506 /*0xFE06*/,

        /// <summary>[InlineMethod;Next] Pushes an unmanaged pointer (type native int) to the native code implementing a particular virtual method associated with a specified object onto the evaluation stack.</summary>
        Ldvirtftn = -505 /*0xFE07*/,
        #endregion

        #region InlineSig
        /// <summary>[InlineSig;Call] Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention.</summary>
        Calli = 41 /*0x29*/,
        #endregion

        #region ShortInlineBrTarget
        /// <summary>[ShortInlineBrTarget;Branch] Unconditionally transfers control to a target instruction (short form).</summary>
        Br_S = 43 /*0x2B*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction if <paramref name="value" /> is false, a null reference, or zero.</summary>
        Brfalse_S = 44 /*0x2C*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if <paramref name="value" /> is true, not null, or non-zero.</summary>
        Brtrue_S = 45 /*0x2D*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if two values are equal.</summary>
        Beq_S = 46 /*0x2E*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than or equal to the second value.</summary>
        Bge_S = 47 /*0x2F*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than the second value.</summary>
        Bgt_S = 48 /*0x30*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than or equal to the second value.</summary>
        Ble_S = 49 /*0x31*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than the second value.</summary>
        Blt_S = 50 /*0x32*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) when two unsigned integer values or unordered float values are not equal.</summary>
        Bne_Un_S = 51 /*0x33*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bge_Un_S = 52 /*0x34*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bgt_Un_S = 53 /*0x35*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.</summary>
        Ble_Un_S = 54 /*0x36*/,

        /// <summary>[ShortInlineBrTarget;Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Blt_Un_S = 55 /*0x37*/,

        /// <summary>[ShortInlineBrTarget;Branch] Exits a protected region of code, unconditionally transferring control to a target instruction (short form).</summary>
        Leave_S = 222 /*0xDE*/,
        #endregion

        #region InlineBrTarget
        /// <summary>[InlineBrTarget;Branch] Unconditionally transfers control to a target instruction.</summary>
        Br = 56 /*0x38*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if <paramref name="value" /> is false, a null reference (Nothing in Visual Basic), or zero.</summary>
        Brfalse = 57 /*0x39*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if <paramref name="value" /> is true, not null, or non-zero.</summary>
        Brtrue = 58 /*0x3A*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if two values are equal.</summary>
        Beq = 59 /*0x3B*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is greater than or equal to the second value.</summary>
        Bge = 60 /*0x3C*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is greater than the second value.</summary>
        Bgt = 61 /*0x3D*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is less than or equal to the second value.</summary>
        Ble = 62 /*0x3E*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is less than the second value.</summary>
        Blt = 63 /*0x3F*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.</summary>
        Bne_Un = 64 /*0x40*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bge_Un = 65 /*0x41*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bgt_Un = 66 /*0x42*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.</summary>
        Ble_Un = 67 /*0x43*/,

        /// <summary>[InlineBrTarget;Cond_Branch] Transfers control to a target instruction if the first value is less than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Blt_Un = 68 /*0x44*/,

        /// <summary>[InlineBrTarget;Branch] Exits a protected region of code, unconditionally transferring control to a specific target instruction.</summary>
        Leave = 221 /*0xDD*/,
        #endregion

        #region InlineSwitch
        /// <summary>[InlineSwitch;Cond_Branch] Implements a jump table.</summary>
        Switch = 69 /*0x45*/,
        #endregion

        #region InlineType
        /// <summary>[InlineType;Next] Copies the value type located at the address of an object (type &amp;, * or native int) to the address of the destination object (type &amp;, * or native int).</summary>
        Cpobj = 112 /*0x70*/,

        /// <summary>[InlineType;Next] Copies the value type object pointed to by an address to the top of the evaluation stack.</summary>
        Ldobj = 113 /*0x71*/,

        /// <summary>[InlineType;Next] Attempts to cast an object passed by reference to the specified class.</summary>
        Castclass = 116 /*0x74*/,

        /// <summary>[InlineType;Next] Tests whether an object reference (type O) is an instance of a particular class.</summary>
        Isinst = 117 /*0x75*/,

        /// <summary>[InlineType;Next] Converts the boxed representation of a value type to its unboxed form.</summary>
        Unbox = 121 /*0x79*/,

        /// <summary>[InlineType;Next] Copies a value of a specified type from the evaluation stack into a supplied memory address.</summary>
        Stobj = 129 /*0x81*/,

        /// <summary>[InlineType;Next] Converts a value type to an object reference (type O).</summary>
        Box = 140 /*0x8C*/,

        /// <summary>[InlineType;Next] Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.</summary>
        Newarr = 141 /*0x8D*/,

        /// <summary>[InlineType;Next] Loads the address of the array element at a specified array index onto the top of the evaluation stack as type &amp; (managed pointer).</summary>
        Ldelema = 143 /*0x8F*/,

        /// <summary>[InlineType;Next] Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction. </summary>
        Ldelem = 163 /*0xA3*/,

        /// <summary>[InlineType;Next] Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.</summary>
        Stelem = 164 /*0xA4*/,

        /// <summary>[InlineType;Next] Converts the boxed representation of a type specified in the instruction to its unboxed form. </summary>
        Unbox_Any = 165 /*0xA5*/,

        /// <summary>[InlineType;Next] Retrieves the address (type &amp;) embedded in a typed reference.</summary>
        Refanyval = 194 /*0xC2*/,

        /// <summary>[InlineType;Next] Pushes a typed reference to an instance of a specific type onto the evaluation stack.</summary>
        Mkrefany = 198 /*0xC6*/,

        /// <summary>[InlineType;Next] Initializes each field of the value type at a specified address to a null reference or a 0 of the appropriate primitive type.</summary>
        Initobj = -491 /*0xFE15*/,

        /// <summary>[InlineType;Meta] Constrains the type on which a virtual method call is made.</summary>
        Constrained = -490 /*0xFE16*/,

        /// <summary>[InlineType;Next] Pushes the size, in bytes, of a supplied value type onto the evaluation stack.</summary>
        Sizeof = -484 /*0xFE1C*/,
        #endregion

        #region InlineString
        /// <summary>[InlineString;Next] Pushes a new object reference to a string literal stored in the metadata.</summary>
        Ldstr = 114 /*0x72*/,
        #endregion

        #region InlineField
        /// <summary>[InlineField;Next] Finds the value of a field in the object whose reference is currently on the evaluation stack.</summary>
        Ldfld = 123 /*0x7B*/,

        /// <summary>[InlineField;Next] Finds the address of a field in the object whose reference is currently on the evaluation stack.</summary>
        Ldflda = 124 /*0x7C*/,

        /// <summary>[InlineField;Next] Replaces the value stored in the field of an object reference or pointer with a new value.</summary>
        Stfld = 125 /*0x7D*/,

        /// <summary>[InlineField;Next] Pushes the value of a static field onto the evaluation stack.</summary>
        Ldsfld = 126 /*0x7E*/,

        /// <summary>[InlineField;Next] Pushes the address of a static field onto the evaluation stack.</summary>
        Ldsflda = 127 /*0x7F*/,

        /// <summary>[InlineField;Next] Replaces the value of a static field with a value from the evaluation stack.</summary>
        Stsfld = 128 /*0x80*/,
        #endregion

        #region InlineTok
        /// <summary>[InlineTok;Next] Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.</summary>
        Ldtoken = 208 /*0xD0*/,
        #endregion

        #region InlineVar
        /// <summary>[InlineVar;Next] Loads an argument (referenced by a specified index value) onto the stack.</summary>
        Ldarg = -503 /*0xFE09*/,

        /// <summary>[InlineVar;Next] Load an argument address onto the evaluation stack.</summary>
        Ldarga = -502 /*0xFE0A*/,

        /// <summary>[InlineVar;Next] Stores the value on top of the evaluation stack in the argument slot at a specified index.</summary>
        Starg = -501 /*0xFE0B*/,

        /// <summary>[InlineVar;Next] Loads the local variable at a specific index onto the evaluation stack.</summary>
        Ldloc = -500 /*0xFE0C*/,

        /// <summary>[InlineVar;Next] Loads the address of the local variable at a specific index onto the evaluation stack.</summary>
        Ldloca = -499 /*0xFE0D*/,

        /// <summary>[InlineVar;Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.</summary>
        Stloc = -498 /*0xFE0E*/,
        #endregion
    }

    public enum InlineNoneInstructionType : short
    {
        /// <summary>[Next] Fills space if opcodes are patched. No meaningful operation is performed although a processing cycle can be consumed.</summary>
        Nop = 0 /*0x0*/,

        /// <summary>[Break] Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped.</summary>
        Break = 1 /*0x1*/,

        /// <summary>[Next] Loads the argument at index 0 onto the evaluation stack.</summary>
        Ldarg_0 = 2 /*0x2*/,

        /// <summary>[Next] Loads the argument at index 1 onto the evaluation stack.</summary>
        Ldarg_1 = 3 /*0x3*/,

        /// <summary>[Next] Loads the argument at index 2 onto the evaluation stack.</summary>
        Ldarg_2 = 4 /*0x4*/,

        /// <summary>[Next] Loads the argument at index 3 onto the evaluation stack.</summary>
        Ldarg_3 = 5 /*0x5*/,

        /// <summary>[Next] Loads the local variable at index 0 onto the evaluation stack.</summary>
        Ldloc_0 = 6 /*0x6*/,

        /// <summary>[Next] Loads the local variable at index 1 onto the evaluation stack.</summary>
        Ldloc_1 = 7 /*0x7*/,

        /// <summary>[Next] Loads the local variable at index 2 onto the evaluation stack.</summary>
        Ldloc_2 = 8 /*0x8*/,

        /// <summary>[Next] Loads the local variable at index 3 onto the evaluation stack.</summary>
        Ldloc_3 = 9 /*0x9*/,

        /// <summary>[Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 0.</summary>
        Stloc_0 = 10 /*0xA*/,

        /// <summary>[Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 1.</summary>
        Stloc_1 = 11 /*0xB*/,

        /// <summary>[Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 2.</summary>
        Stloc_2 = 12 /*0xC*/,

        /// <summary>[Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 3.</summary>
        Stloc_3 = 13 /*0xD*/,

        /// <summary>[Next] Pushes a null reference (type O) onto the evaluation stack.</summary>
        Ldnull = 20 /*0x14*/,

        /// <summary>[Next] Pushes the integer value of -1 onto the evaluation stack as an int32.</summary>
        Ldc_I4_M1 = 21 /*0x15*/,

        /// <summary>[Next] Pushes the integer value of 0 onto the evaluation stack as an int32.</summary>
        Ldc_I4_0 = 22 /*0x16*/,

        /// <summary>[Next] Pushes the integer value of 1 onto the evaluation stack as an int32.</summary>
        Ldc_I4_1 = 23 /*0x17*/,

        /// <summary>[Next] Pushes the integer value of 2 onto the evaluation stack as an int32.</summary>
        Ldc_I4_2 = 24 /*0x18*/,

        /// <summary>[Next] Pushes the integer value of 3 onto the evaluation stack as an int32.</summary>
        Ldc_I4_3 = 25 /*0x19*/,

        /// <summary>[Next] Pushes the integer value of 4 onto the evaluation stack as an int32.</summary>
        Ldc_I4_4 = 26 /*0x1A*/,

        /// <summary>[Next] Pushes the integer value of 5 onto the evaluation stack as an int32.</summary>
        Ldc_I4_5 = 27 /*0x1B*/,

        /// <summary>[Next] Pushes the integer value of 6 onto the evaluation stack as an int32.</summary>
        Ldc_I4_6 = 28 /*0x1C*/,

        /// <summary>[Next] Pushes the integer value of 7 onto the evaluation stack as an int32.</summary>
        Ldc_I4_7 = 29 /*0x1D*/,

        /// <summary>[Next] Pushes the integer value of 8 onto the evaluation stack as an int32.</summary>
        Ldc_I4_8 = 30 /*0x1E*/,

        /// <summary>[Next] Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.</summary>
        Dup = 37 /*0x25*/,

        /// <summary>[Next] Removes the value currently on top of the evaluation stack.</summary>
        Pop = 38 /*0x26*/,

        /// <summary>[Return] Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.</summary>
        Ret = 42 /*0x2A*/,

        /// <summary>[Next] Loads a value of type int8 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_I1 = 70 /*0x46*/,

        /// <summary>[Next] Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_U1 = 71 /*0x47*/,

        /// <summary>[Next] Loads a value of type int16 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_I2 = 72 /*0x48*/,

        /// <summary>[Next] Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_U2 = 73 /*0x49*/,

        /// <summary>[Next] Loads a value of type int32 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_I4 = 74 /*0x4A*/,

        /// <summary>[Next] Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.</summary>
        Ldind_U4 = 75 /*0x4B*/,

        /// <summary>[Next] Loads a value of type int64 as an int64 onto the evaluation stack indirectly.</summary>
        Ldind_I8 = 76 /*0x4C*/,

        /// <summary>[Next] Loads a value of type native int as a native int onto the evaluation stack indirectly.</summary>
        Ldind_I = 77 /*0x4D*/,

        /// <summary>[Next] Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.</summary>
        Ldind_R4 = 78 /*0x4E*/,

        /// <summary>[Next] Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.</summary>
        Ldind_R8 = 79 /*0x4F*/,

        /// <summary>[Next] Loads an object reference as a type O (object reference) onto the evaluation stack indirectly.</summary>
        Ldind_Ref = 80 /*0x50*/,

        /// <summary>[Next] Stores a object reference value at a supplied address.</summary>
        Stind_Ref = 81 /*0x51*/,

        /// <summary>[Next] Stores a value of type int8 at a supplied address.</summary>
        Stind_I1 = 82 /*0x52*/,

        /// <summary>[Next] Stores a value of type int16 at a supplied address.</summary>
        Stind_I2 = 83 /*0x53*/,

        /// <summary>[Next] Stores a value of type int32 at a supplied address.</summary>
        Stind_I4 = 84 /*0x54*/,

        /// <summary>[Next] Stores a value of type int64 at a supplied address.</summary>
        Stind_I8 = 85 /*0x55*/,

        /// <summary>[Next] Stores a value of type float32 at a supplied address.</summary>
        Stind_R4 = 86 /*0x56*/,

        /// <summary>[Next] Stores a value of type float64 at a supplied address.</summary>
        Stind_R8 = 87 /*0x57*/,

        /// <summary>[Next] Adds two values and pushes the result onto the evaluation stack.</summary>
        Add = 88 /*0x58*/,

        /// <summary>[Next] Subtracts one value from another and pushes the result onto the evaluation stack.</summary>
        Sub = 89 /*0x59*/,

        /// <summary>[Next] Multiplies two values and pushes the result on the evaluation stack.</summary>
        Mul = 90 /*0x5A*/,

        /// <summary>[Next] Divides two values and pushes the result as a floating-point (type F) or quotient (type int32) onto the evaluation stack.</summary>
        Div = 91 /*0x5B*/,

        /// <summary>[Next] Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.</summary>
        Div_Un = 92 /*0x5C*/,

        /// <summary>[Next] Divides two values and pushes the remainder onto the evaluation stack.</summary>
        Rem = 93 /*0x5D*/,

        /// <summary>[Next] Divides two unsigned values and pushes the remainder onto the evaluation stack.</summary>
        Rem_Un = 94 /*0x5E*/,

        /// <summary>[Next] Computes the bitwise AND of two values and pushes the result onto the evaluation stack.</summary>
        And = 95 /*0x5F*/,

        /// <summary>[Next] Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack.</summary>
        Or = 96 /*0x60*/,

        /// <summary>[Next] Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result onto the evaluation stack.</summary>
        Xor = 97 /*0x61*/,

        /// <summary>[Next] Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the result onto the evaluation stack.</summary>
        Shl = 98 /*0x62*/,

        /// <summary>[Next] Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result onto the evaluation stack.</summary>
        Shr = 99 /*0x63*/,

        /// <summary>[Next] Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits, pushing the result onto the evaluation stack.</summary>
        Shr_Un = 100 /*0x64*/,

        /// <summary>[Next] Negates a value and pushes the result onto the evaluation stack.</summary>
        Neg = 101 /*0x65*/,

        /// <summary>[Next] Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.</summary>
        Not = 102 /*0x66*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.</summary>
        Conv_I1 = 103 /*0x67*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.</summary>
        Conv_I2 = 104 /*0x68*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to int32.</summary>
        Conv_I4 = 105 /*0x69*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to int64.</summary>
        Conv_I8 = 106 /*0x6A*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to float32.</summary>
        Conv_R4 = 107 /*0x6B*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to float64.</summary>
        Conv_R8 = 108 /*0x6C*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.</summary>
        Conv_U4 = 109 /*0x6D*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.</summary>
        Conv_U8 = 110 /*0x6E*/,

        /// <summary>[Next] Converts the unsigned integer value on top of the evaluation stack to float32.</summary>
        Conv_R_Un = 118 /*0x76*/,

        /// <summary>[Throw] Throws the exception object currently on the evaluation stack.</summary>
        Throw = 122 /*0x7A*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I1_Un = 130 /*0x82*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I2_Un = 131 /*0x83*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to signed int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I4_Un = 132 /*0x84*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to signed int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I8_Un = 133 /*0x85*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U1_Un = 134 /*0x86*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U2_Un = 135 /*0x87*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U4_Un = 136 /*0x88*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U8_Un = 137 /*0x89*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to signed native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I_Un = 138 /*0x8A*/,

        /// <summary>[Next] Converts the unsigned value on top of the evaluation stack to unsigned native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U_Un = 139 /*0x8B*/,

        /// <summary>[Next] Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.</summary>
        Ldlen = 142 /*0x8E*/,

        /// <summary>[Next] Loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_I1 = 144 /*0x90*/,

        /// <summary>[Next] Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_U1 = 145 /*0x91*/,

        /// <summary>[Next] Loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_I2 = 146 /*0x92*/,

        /// <summary>[Next] Loads the element with type unsigned int16 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_U2 = 147 /*0x93*/,

        /// <summary>[Next] Loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_I4 = 148 /*0x94*/,

        /// <summary>[Next] Loads the element with type unsigned int32 at a specified array index onto the top of the evaluation stack as an int32.</summary>
        Ldelem_U4 = 149 /*0x95*/,

        /// <summary>[Next] Loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64.</summary>
        Ldelem_I8 = 150 /*0x96*/,

        /// <summary>[Next] Loads the element with type native int at a specified array index onto the top of the evaluation stack as a native int.</summary>
        Ldelem_I = 151 /*0x97*/,

        /// <summary>[Next] Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float).</summary>
        Ldelem_R4 = 152 /*0x98*/,

        /// <summary>[Next] Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float).</summary>
        Ldelem_R8 = 153 /*0x99*/,

        /// <summary>[Next] Loads the element containing an object reference at a specified array index onto the top of the evaluation stack as type O (object reference).</summary>
        Ldelem_Ref = 154 /*0x9A*/,

        /// <summary>[Next] Replaces the array element at a given index with the native int value on the evaluation stack.</summary>
        Stelem_I = 155 /*0x9B*/,

        /// <summary>[Next] Replaces the array element at a given index with the int8 value on the evaluation stack.</summary>
        Stelem_I1 = 156 /*0x9C*/,

        /// <summary>[Next] Replaces the array element at a given index with the int16 value on the evaluation stack.</summary>
        Stelem_I2 = 157 /*0x9D*/,

        /// <summary>[Next] Replaces the array element at a given index with the int32 value on the evaluation stack.</summary>
        Stelem_I4 = 158 /*0x9E*/,

        /// <summary>[Next] Replaces the array element at a given index with the int64 value on the evaluation stack.</summary>
        Stelem_I8 = 159 /*0x9F*/,

        /// <summary>[Next] Replaces the array element at a given index with the float32 value on the evaluation stack.</summary>
        Stelem_R4 = 160 /*0xA0*/,

        /// <summary>[Next] Replaces the array element at a given index with the float64 value on the evaluation stack.</summary>
        Stelem_R8 = 161 /*0xA1*/,

        /// <summary>[Next] Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.</summary>
        Stelem_Ref = 162 /*0xA2*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to signed int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I1 = 179 /*0xB3*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U1 = 180 /*0xB4*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to signed int16 and extending it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I2 = 181 /*0xB5*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U2 = 182 /*0xB6*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to signed int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I4 = 183 /*0xB7*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to unsigned int32, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U4 = 184 /*0xB8*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to signed int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I8 = 185 /*0xB9*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to unsigned int64, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U8 = 186 /*0xBA*/,

        /// <summary>[Next] Throws <see cref="T:System.ArithmeticException" /> if value is not a finite number.</summary>
        Ckfinite = 195 /*0xC3*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.</summary>
        Conv_U2 = 209 /*0xD1*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.</summary>
        Conv_U1 = 210 /*0xD2*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to native int.</summary>
        Conv_I = 211 /*0xD3*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to signed native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_I = 212 /*0xD4*/,

        /// <summary>[Next] Converts the signed value on top of the evaluation stack to unsigned native int, throwing <see cref="T:System.OverflowException" /> on overflow.</summary>
        Conv_Ovf_U = 213 /*0xD5*/,

        /// <summary>[Next] Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Add_Ovf = 214 /*0xD6*/,

        /// <summary>[Next] Adds two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Add_Ovf_Un = 215 /*0xD7*/,

        /// <summary>[Next] Multiplies two integer values, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Mul_Ovf = 216 /*0xD8*/,

        /// <summary>[Next] Multiplies two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Mul_Ovf_Un = 217 /*0xD9*/,

        /// <summary>[Next] Subtracts one integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Sub_Ovf = 218 /*0xDA*/,

        /// <summary>[Next] Subtracts one unsigned integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.</summary>
        Sub_Ovf_Un = 219 /*0xDB*/,

        /// <summary>[Return] Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.</summary>
        Endfinally = 220 /*0xDC*/,

        /// <summary>[Next] Stores a value of type native int at a supplied address.</summary>
        Stind_I = 223 /*0xDF*/,

        /// <summary>[Next] Converts the value on top of the evaluation stack to unsigned native int, and extends it to native int.</summary>
        Conv_U = 224 /*0xE0*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix7 = 248 /*0xF8*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix6 = 249 /*0xF9*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix5 = 250 /*0xFA*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix4 = 251 /*0xFB*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix3 = 252 /*0xFC*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix2 = 253 /*0xFD*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefix1 = 254 /*0xFE*/,

        /// <summary>[Meta] This is a reserved instruction.</summary>
        Prefixref = 255 /*0xFF*/,

        /// <summary>[Next] Returns an unmanaged pointer to the argument list of the current method.</summary>
        Arglist = -512 /*0xFE00*/,

        /// <summary>[Next] Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Ceq = -511 /*0xFE01*/,

        /// <summary>[Next] Compares two values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Cgt = -510 /*0xFE02*/,

        /// <summary>[Next] Compares two unsigned or unordered values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Cgt_Un = -509 /*0xFE03*/,

        /// <summary>[Next] Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Clt = -508 /*0xFE04*/,

        /// <summary>[Next] Compares the unsigned or unordered values <paramref name="value1" /> and <paramref name="value2" />. If <paramref name="value1" /> is less than <paramref name="value2" />, then the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.</summary>
        Clt_Un = -507 /*0xFE05*/,

        /// <summary>[Next] Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (a transient pointer, type *) of the first allocated byte onto the evaluation stack.</summary>
        Localloc = -497 /*0xFE0F*/,

        /// <summary>[Return] Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.</summary>
        Endfilter = -495 /*0xFE11*/,

        /// <summary>[Meta] Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.</summary>
        Volatile = -493 /*0xFE13*/,

        /// <summary>[Meta] Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.</summary>
        Tailcall = -492 /*0xFE14*/,

        /// <summary>[Next] Copies a specified number bytes from a source address to a destination address.</summary>
        Cpblk = -489 /*0xFE17*/,

        /// <summary>[Next] Initializes a specified block of memory at a specific address to a given size and initial value.</summary>
        Initblk = -488 /*0xFE18*/,

        /// <summary>[Throw] Rethrows the current exception.</summary>
        Rethrow = -486 /*0xFE1A*/,

        /// <summary>[Next] Retrieves the type token embedded in a typed reference.</summary>
        Refanytype = -483 /*0xFE1D*/,

        /// <summary>[Meta] Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.</summary>
        Readonly = -482 /*0xFE1E*/,
    }

    public enum ShortInlineVarInstructionType : short
    {
        /// <summary>[Next] Loads the argument (referenced by a specified short form index) onto the evaluation stack.</summary>
        Ldarg_S = 14 /*0xE*/,

        /// <summary>[Next] Load an argument address, in short form, onto the evaluation stack.</summary>
        Ldarga_S = 15 /*0xF*/,

        /// <summary>[Next] Stores the value on top of the evaluation stack in the argument slot at a specified index, short form.</summary>
        Starg_S = 16 /*0x10*/,

        /// <summary>[Next] Loads the local variable at a specific index onto the evaluation stack, short form.</summary>
        Ldloc_S = 17 /*0x11*/,

        /// <summary>[Next] Loads the address of the local variable at a specific index onto the evaluation stack, short form.</summary>
        Ldloca_S = 18 /*0x12*/,

        /// <summary>[Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at <paramref name="index" /> (short form).</summary>
        Stloc_S = 19 /*0x13*/,
    }

    public enum ShortInlineIInstructionType : short
    {
        /// <summary>[Next] Pushes the supplied int8 value onto the evaluation stack as an int32, short form.</summary>
        Ldc_I4_S = 31 /*0x1F*/,

        /// <summary>[Meta] Indicates that an address currently atop the evaluation stack might not be aligned to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk instruction.</summary>
        Unaligned = -494 /*0xFE12*/,
    }

    public enum InlineMethodInstructionType : short
    {
        /// <summary>[Call] Exits current method and jumps to specified method.</summary>
        Jmp = 39 /*0x27*/,

        /// <summary>[Call] Calls the method indicated by the passed method descriptor.</summary>
        Call = 40 /*0x28*/,

        /// <summary>[Call] Calls a late-bound method on an object, pushing the return value onto the evaluation stack.</summary>
        Callvirt = 111 /*0x6F*/,

        /// <summary>[Call] Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.</summary>
        Newobj = 115 /*0x73*/,

        /// <summary>[Next] Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.</summary>
        Ldftn = -506 /*0xFE06*/,

        /// <summary>[Next] Pushes an unmanaged pointer (type native int) to the native code implementing a particular virtual method associated with a specified object onto the evaluation stack.</summary>
        Ldvirtftn = -505 /*0xFE07*/,
    }

    public enum ShortInlineBrTargetInstructionType : short
    {
        /// <summary>[Branch] Unconditionally transfers control to a target instruction (short form).</summary>
        Br_S = 43 /*0x2B*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if <paramref name="value" /> is false, a null reference, or zero.</summary>
        Brfalse_S = 44 /*0x2C*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if <paramref name="value" /> is true, not null, or non-zero.</summary>
        Brtrue_S = 45 /*0x2D*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if two values are equal.</summary>
        Beq_S = 46 /*0x2E*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than or equal to the second value.</summary>
        Bge_S = 47 /*0x2F*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than the second value.</summary>
        Bgt_S = 48 /*0x30*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than or equal to the second value.</summary>
        Ble_S = 49 /*0x31*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than the second value.</summary>
        Blt_S = 50 /*0x32*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) when two unsigned integer values or unordered float values are not equal.</summary>
        Bne_Un_S = 51 /*0x33*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bge_Un_S = 52 /*0x34*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bgt_Un_S = 53 /*0x35*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.</summary>
        Ble_Un_S = 54 /*0x36*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction (short form) if the first value is less than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Blt_Un_S = 55 /*0x37*/,

        /// <summary>[Branch] Exits a protected region of code, unconditionally transferring control to a target instruction (short form).</summary>
        Leave_S = 222 /*0xDE*/,
    }

    public enum InlineBrTargetInstructionType : short
    {
        /// <summary>[Branch] Unconditionally transfers control to a target instruction.</summary>
        Br = 56 /*0x38*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if <paramref name="value" /> is false, a null reference (Nothing in Visual Basic), or zero.</summary>
        Brfalse = 57 /*0x39*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if <paramref name="value" /> is true, not null, or non-zero.</summary>
        Brtrue = 58 /*0x3A*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if two values are equal.</summary>
        Beq = 59 /*0x3B*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is greater than or equal to the second value.</summary>
        Bge = 60 /*0x3C*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is greater than the second value.</summary>
        Bgt = 61 /*0x3D*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is less than or equal to the second value.</summary>
        Ble = 62 /*0x3E*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is less than the second value.</summary>
        Blt = 63 /*0x3F*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.</summary>
        Bne_Un = 64 /*0x40*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bge_Un = 65 /*0x41*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Bgt_Un = 66 /*0x42*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.</summary>
        Ble_Un = 67 /*0x43*/,

        /// <summary>[Cond_Branch] Transfers control to a target instruction if the first value is less than the second value, when comparing unsigned integer values or unordered float values.</summary>
        Blt_Un = 68 /*0x44*/,

        /// <summary>[Branch] Exits a protected region of code, unconditionally transferring control to a specific target instruction.</summary>
        Leave = 221 /*0xDD*/,
    }

    public enum InlineTypeInstructionType : short
    {
        /// <summary>[Next] Copies the value type located at the address of an object (type &amp;, * or native int) to the address of the destination object (type &amp;, * or native int).</summary>
        Cpobj = 112 /*0x70*/,

        /// <summary>[Next] Copies the value type object pointed to by an address to the top of the evaluation stack.</summary>
        Ldobj = 113 /*0x71*/,

        /// <summary>[Next] Attempts to cast an object passed by reference to the specified class.</summary>
        Castclass = 116 /*0x74*/,

        /// <summary>[Next] Tests whether an object reference (type O) is an instance of a particular class.</summary>
        Isinst = 117 /*0x75*/,

        /// <summary>[Next] Converts the boxed representation of a value type to its unboxed form.</summary>
        Unbox = 121 /*0x79*/,

        /// <summary>[Next] Copies a value of a specified type from the evaluation stack into a supplied memory address.</summary>
        Stobj = 129 /*0x81*/,

        /// <summary>[Next] Converts a value type to an object reference (type O).</summary>
        Box = 140 /*0x8C*/,

        /// <summary>[Next] Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.</summary>
        Newarr = 141 /*0x8D*/,

        /// <summary>[Next] Loads the address of the array element at a specified array index onto the top of the evaluation stack as type &amp; (managed pointer).</summary>
        Ldelema = 143 /*0x8F*/,

        /// <summary>[Next] Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction. </summary>
        Ldelem = 163 /*0xA3*/,

        /// <summary>[Next] Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.</summary>
        Stelem = 164 /*0xA4*/,

        /// <summary>[Next] Converts the boxed representation of a type specified in the instruction to its unboxed form. </summary>
        Unbox_Any = 165 /*0xA5*/,

        /// <summary>[Next] Retrieves the address (type &amp;) embedded in a typed reference.</summary>
        Refanyval = 194 /*0xC2*/,

        /// <summary>[Next] Pushes a typed reference to an instance of a specific type onto the evaluation stack.</summary>
        Mkrefany = 198 /*0xC6*/,

        /// <summary>[Next] Initializes each field of the value type at a specified address to a null reference or a 0 of the appropriate primitive type.</summary>
        Initobj = -491 /*0xFE15*/,

        /// <summary>[Meta] Constrains the type on which a virtual method call is made.</summary>
        Constrained = -490 /*0xFE16*/,

        /// <summary>[Next] Pushes the size, in bytes, of a supplied value type onto the evaluation stack.</summary>
        Sizeof = -484 /*0xFE1C*/,
    }

    public enum InlineFieldInstructionType : short
    {
        /// <summary>[Next] Finds the value of a field in the object whose reference is currently on the evaluation stack.</summary>
        Ldfld = 123 /*0x7B*/,

        /// <summary>[Next] Finds the address of a field in the object whose reference is currently on the evaluation stack.</summary>
        Ldflda = 124 /*0x7C*/,

        /// <summary>[Next] Replaces the value stored in the field of an object reference or pointer with a new value.</summary>
        Stfld = 125 /*0x7D*/,

        /// <summary>[Next] Pushes the value of a static field onto the evaluation stack.</summary>
        Ldsfld = 126 /*0x7E*/,

        /// <summary>[Next] Pushes the address of a static field onto the evaluation stack.</summary>
        Ldsflda = 127 /*0x7F*/,

        /// <summary>[Next] Replaces the value of a static field with a value from the evaluation stack.</summary>
        Stsfld = 128 /*0x80*/,
    }

    public enum InlineVarInstructionType : short
    {
        /// <summary>[Next] Loads an argument (referenced by a specified index value) onto the stack.</summary>
        Ldarg = -503 /*0xFE09*/,

        /// <summary>[Next] Load an argument address onto the evaluation stack.</summary>
        Ldarga = -502 /*0xFE0A*/,

        /// <summary>[Next] Stores the value on top of the evaluation stack in the argument slot at a specified index.</summary>
        Starg = -501 /*0xFE0B*/,

        /// <summary>[Next] Loads the local variable at a specific index onto the evaluation stack.</summary>
        Ldloc = -500 /*0xFE0C*/,

        /// <summary>[Next] Loads the address of the local variable at a specific index onto the evaluation stack.</summary>
        Ldloca = -499 /*0xFE0D*/,

        /// <summary>[Next] Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.</summary>
        Stloc = -498 /*0xFE0E*/,
    }
}