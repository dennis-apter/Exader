using System;

namespace Exader
{
    public static class UriHelper
    {
        public static bool TryParse(string value, out Uri result)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.ToLower().EnsureStartsWith("http://", StringComparison.OrdinalIgnoreCase);
                if (Uri.TryCreate(value, UriKind.Absolute, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}
