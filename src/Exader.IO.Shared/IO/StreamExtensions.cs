using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Exader.IO
{
	[UsedImplicitly(ImplicitUseTargetFlags.Members)]
	public static class StreamExtensions
	{
		public static void CopyTo(this Stream input, Stream output)
		{
			input.CopyTo(output, 64 * 1024);
		}

		public static void CopyTo(this Stream input, Stream output, int bufferSize)
		{
			var buffer = new byte[bufferSize];
			int count;
			do
			{
				count = input.Read(buffer, 0, bufferSize);
				output.Write(buffer, 0, count);
			}
			while (0 < count);
		}

		public static void ConvertTo(this Stream input, Stream output, Encoding inputEncoding, Encoding outputEncoding, int bufferSize = 1096)
		{
#if NET35 || SILVERLIGHT
			using (var reader = new StreamReader(input, inputEncoding, false, bufferSize))
#else
			using (var reader = new StreamReader(input, inputEncoding, false, bufferSize, true))
#endif
			{
#if NET35 || SILVERLIGHT
				using (var writer = new StreamWriter(output, outputEncoding, bufferSize))
#else
				using (var writer = new StreamWriter(output, outputEncoding, bufferSize, true))
#endif
				{
					int c;
					while (0 <= (c = reader.Read()))
					{
						writer.Write((char)c);
					}
				}
			}
		}

		public static bool IsEnd(this Stream self)
		{
			return !self.CanRead || self.Length <= self.Position;
		}

		/// <summary>
		/// Detect UTF8 encoding.
		/// </summary>
		/// <remarks>
		/// Thanks to devdimi's project Utf8Checker: http://utf8checker.codeplex.com/
		/// </remarks>
		/// <param name="self"></param>
		/// <returns></returns>
		public static bool IsUtf8(this Stream self)
		{
			var position = self.Position;
			bool result = true;
			while (!self.IsEnd())
			{
				if (!IsUtf8Character(self))
				{
					result = false;
					break;
				}
			}

			if (self.CanSeek)
			{
				self.Seek(position, SeekOrigin.Begin);
			}

			return result;
		}

		private static bool IsUtf8Character(Stream stream)
		{
			if (stream.IsEnd())
			{
				return false;
			}

			byte b0 = (byte)stream.ReadByte();
			if (b0 <= 0x7F)
			{
				return true;
			}

			byte b1 = (byte)stream.ReadByte();
			if (0xc2 <= b0 && b0 <= 0xdf)
			{
				if (stream.IsEnd())
				{
					return false;
				}

				if (b1 < 0x80 || b1 > 0xbf)
				{
					return false;
				}

				return true;
			}


			if (stream.IsEnd())
			{
				return false;
			}

			byte b2 = (byte)stream.ReadByte();
			if (b0 == 0xe0)
			{
				if (stream.IsEnd())
				{
					return false;
				}

				if (b1 < 0xa0 || b1 > 0xbf ||
					b2 < 0x80 || b2 > 0xbf)
				{
					return false;
				}

				return true;
			}


			if (0xe1 <= b0 && b0 <= 0xef)
			{
				if (stream.IsEnd())
				{
					return false;
				}

				if (b1 < 0x80 || b1 > 0xbf ||
					b2 < 0x80 || b2 > 0xbf)
				{
					return false;
				}

				return true;
			}

			if (stream.IsEnd())
			{
				return false;
			}

			byte b3 = (byte)stream.ReadByte();
			if (b0 == 0xf0)
			{
				if (stream.IsEnd())
				{
					return false;
				}

				if (b1 < 0x90 || b1 > 0xbf ||
					b2 < 0x80 || b2 > 0xbf ||
					b3 < 0x80 || b3 > 0xbf)
				{
					return false;
				}

				return true;
			}

			if (b0 == 0xf4)
			{
				if (stream.IsEnd())
				{
					return false;
				}

				if (b1 < 0x80 || b1 > 0x8f ||
					b2 < 0x80 || b2 > 0xbf ||
					b3 < 0x80 || b3 > 0xbf)
				{
					return false;
				}

				return true;
			}

			if (0xf1 <= b0 && b0 <= 0xf3)
			{
				if (stream.IsEnd())
				{
					return false;
				}

				if (b1 < 0x80 || b1 > 0xbf ||
					b2 < 0x80 || b2 > 0xbf ||
					b3 < 0x80 || b3 > 0xbf)
				{
					return false;
				}

				return true;
			}

			return false;
		}
	}
}
