using System;
using System.Collections.Generic;

namespace Exader
{
	public static class Base36
	{
		private const string Alphabyte = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static int Parse(string str)
		{
			int result;
			if (TryParse(str, out result))
			{
				return result;
			}

			throw new FormatException();
		}

		public static string ToBase36String(this int num)
		{
			bool hasSign = num < 0;
			num = Math.Abs(num);
			var alphas = new Stack<char>(5);
			while (0 != num)
			while (0 != num)
			{
				char alpha = Alphabyte[num%36];
				alphas.Push(alpha);
				num = num/36;
			}

			if (hasSign)
			{
				alphas.Push('-');
			}

			return new string(alphas.ToArray());
		}

		public static bool TryParse(string str, out int result)
		{
			str = str.ToUpperInvariant();

			result = 0;
			for (int index = str.Length - 1; 0 <= index; index--)
			{
				int quantor = Alphabyte.IndexOf(str[index]);
				if (-1 == quantor)
				{
					if ('-' == str[index])
					{
						result = -result;
						if (0 == index)
						{
							return true;
						}
					}

					return false;
				}

				result += quantor*(int) Math.Pow(36, str.Length - index - 1);
			}

			return true;
		}
	}
}