using System;
using System.IO;

namespace Exader.IO
{
	public static class BinaryStream
	{
		public static int Read7BitEncodedInt32(this BinaryReader reader)
		{
			byte temp;
			int value = 0;
			int count = 0;

			do
			{
				if (count == 35)
				{
					throw new FormatException("Too many bytes in what should have been a 7 bit encoded Int32.");
				}

				temp = reader.ReadByte();
				value |= (temp & 0x7f) << count;
				count += 7;
			}
			while ((temp & 0x80) != 0);

			return value;
		}

		public static void Write7BitEncodedInt32(this BinaryWriter writer, int value)
		{
			var temp = (uint)value;
			while (0x80 <= temp)
			{
				writer.Write((byte)(temp | 0x80));
				temp = temp >> 7;
			}

			writer.Write((byte)temp);
		}
	}
}