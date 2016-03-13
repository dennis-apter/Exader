using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Exader
{
    public static class StringBuilderExtensions
    {
#if NET35
        public static StringBuilder Clear(this StringBuilder self)
        {
            self.Remove(0, self.Length);
            return self;
        }
#endif

#if !SILVERLIGHT && !NET35
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Pure]
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
