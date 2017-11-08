using System.Globalization;
using Xunit;

// ReSharper disable RedundantArgumentDefaultValue

namespace Exader
{
    public class SubstringTest
    {
        [Fact]
        public static void SubstringAfterLastWithChar()
        {
            Assert.Equal("C", "A>B>C".SubstringAfterLast('>'));
            Assert.Equal(">C", "A>B>C".SubstringAfterLast('>', true));

            string rest;

            Assert.Equal("C", "A>B>C".SubstringAfterLast('>', out rest));
            Assert.Equal("A>B", rest);

            Assert.Equal(">C", "A>B>C".SubstringAfterLast('>', out rest, true));
            Assert.Equal("A>B", rest);
        }

        [Fact]
        public static void SubstringAfterLastWithString()
        {
            Assert.Equal("C", "A>>B>>C".SubstringAfterLast(">>"));
            Assert.Equal(">>C", "A>>B>>C".SubstringAfterLast(">>", true));

            string rest;

            Assert.Equal("C", "A>>B>>C".SubstringAfterLast(">>", out rest));
            Assert.Equal("A>>B", rest);

            Assert.Equal(">>C", "A>>B>>C".SubstringAfterLast(">>", out rest, true));
            Assert.Equal("A>>B", rest);
        }

        [Fact]
        public static void SubstringAfterWithChar()
        {
            Assert.Equal("B>C", "A>B>C".SubstringAfter('>'));
            Assert.Equal(">B>C", "A>B>C".SubstringAfter('>', true));

            string rest;

            Assert.Equal("B>C", "A>B>C".SubstringAfter('>', out rest));
            Assert.Equal("A", rest);

            Assert.Equal(">B>C", "A>B>C".SubstringAfter('>', out rest, true));
            Assert.Equal("A", rest);
        }

        [Fact]
        public static void SubstringAfterWithString()
        {
            Assert.Equal("B>>C", "A>>B>>C".SubstringAfter(">>"));
            Assert.Equal(">>B>>C", "A>>B>>C".SubstringAfter(">>", true));

            string rest;

            Assert.Equal("B>>C", "A>>B>>C".SubstringAfter(">>", out rest));
            Assert.Equal("A", rest);

            Assert.Equal(">>B>>C", "A>>B>>C".SubstringAfter(">>", out rest, true));
            Assert.Equal("A", rest);
        }

        [Fact]
        public static void SubstringBeforeLastWithChar()
        {
            Assert.Equal("A>B", "A>B>C".SubstringBeforeLast('>'));
            Assert.Equal("A>B>", "A>B>C".SubstringBeforeLast('>', true));

            string rest;

            Assert.Equal("A>B", "A>B>C".SubstringBeforeLast('>', out rest));
            Assert.Equal("C", rest);

            Assert.Equal("A>B>", "A>B>C".SubstringBeforeLast('>', out rest, true));
            Assert.Equal("C", rest);
        }

        [Fact]
        public static void SubstringBeforeLastWithString()
        {
            Assert.Equal("A>>B", "A>>B>>C".SubstringBeforeLast(">>"));
            Assert.Equal("A>>B>>", "A>>B>>C".SubstringBeforeLast(">>", true));

            string rest;

            Assert.Equal("A>>B", "A>>B>>C".SubstringBeforeLast(">>", out rest));
            Assert.Equal("C", rest);

            Assert.Equal("A>>B>>", "A>>B>>C".SubstringBeforeLast(">>", out rest, true));
            Assert.Equal("C", rest);
        }

        [Fact]
        public static void SubstringBeforeWithChar()
        {
            Assert.Equal("A", "A>B>C".SubstringBefore('>'));
            Assert.Equal("A>", "A>B>C".SubstringBefore('>', true));

            string rest;

            Assert.Equal("A", "A>B>C".SubstringBefore('>', out rest));
            Assert.Equal("B>C", rest);

            Assert.Equal("A>", "A>B>C".SubstringBefore('>', out rest, true));
            Assert.Equal("B>C", rest);
        }

        [Fact]
        public static void SubstringBeforeWithString()
        {
            Assert.Equal("A", "A>>B>>C".SubstringBefore(">>"));
            Assert.Equal("A>>", "A>>B>>C".SubstringBefore(">>", true));

            string rest;

            Assert.Equal("A", "A>>B>>C".SubstringBefore(">>", out rest));
            Assert.Equal("B>>C", rest);

            Assert.Equal("A>>", "A>>B>>C".SubstringBefore(">>", out rest, true));
            Assert.Equal("B>>C", rest);
        }

        [Theory]
        [InlineData("", '.')]
        [InlineData("_", '.')]
        public static void SubstringWithNotContainsCharacter(string self, char token)
        {
            string rest;

            // After

            Assert.Equal(string.Empty,
                self.SubstringAfter(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfter(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfter(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringAfter(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);

            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, out rest, false, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, out rest, true, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);

            // Before

            Assert.Equal(string.Empty,
                self.SubstringBefore(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBefore(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBefore(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringBefore(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);

            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, out rest, false, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, out rest, true, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", ".")]
        [InlineData("_", ".")]
        [InlineData("_", null)]
        [InlineData(null, null)]
        public static void SubstringWithNotContainsSubstring(string self, string token)
        {
            string rest;

            // After

            Assert.Equal(string.Empty,
                self.SubstringAfter(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfter(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfter(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringAfter(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringAfterOrSelf(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);

            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringAfterLast(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, out rest, false, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringAfterLastOrSelf(token, out rest, true, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);

            // Before

            Assert.Equal(string.Empty,
                self.SubstringBefore(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBefore(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBefore(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringBefore(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringBeforeOrSelf(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal("", rest);

            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, out rest, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);
            Assert.Equal(string.Empty,
                self.SubstringBeforeLast(token, out rest, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self, rest);

            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, false, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, true, CultureInfo.CurrentCulture, CompareOptions.Ordinal));
            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, out rest, false, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);
            Assert.Equal(self,
                self.SubstringBeforeLastOrSelf(token, out rest, true, CultureInfo.CurrentCulture,
                    CompareOptions.Ordinal));
            Assert.Equal("", rest);
        }
    }
}
