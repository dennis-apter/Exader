using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Exader
{
    public static class StringBuilderExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRepeatOf(this StringBuilder self, char token, int startIndex = 0)
        {
            for (int j = startIndex; j < self.Length; j++)
            {
                if (self[j] != token)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
