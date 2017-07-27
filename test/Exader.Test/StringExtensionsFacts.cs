using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Exader
{
    public class StringExtensionsFacts
    {
        private const string ClassFooMethodBar = "\nclass Foo\n{\nmethod Bar {}\n}\n";
        private const string ClassFooMethodBarIndent1 = "\t\n\tclass Foo\n\t{\n\tmethod Bar {}\n\t}\n\t";
        private const string ClassFooMethodBarIndent2 = "\t\t\n\t\tclass Foo\n\t\t{\n\t\tmethod Bar {}\n\t\t}\n\t\t";
        private const string LoremIpsumLowerCase = "lorem ipsum";
        private const string TheQuickBrownFox = "The quick brown fox jumps over the lazy dog";
        private const string TheQuickBrownFoxCamelCase = "TheQuickBrownFoxJumpsOverTheLazyDog";

        [Fact]
        public static void Capitalize()
        {
            Assert.Equal("Lorem", "lorem".Capitalize());
            Assert.Equal("L", "l".Capitalize());
            Assert.Equal(string.Empty, string.Empty.Capitalize());
            Assert.Equal(string.Empty, ((string)null).Capitalize());
        }

        [Fact]
        public static void Collapse()
        {
            Assert.Equal("loremipsum", "__lorem___ipsum__".Collapse('_'));
            Assert.Equal("_lorem_ipsum_", "__lorem___ipsum__".Collapse('_', 1));
        }

        [Fact]
        public static void CollapseWhiteSpaces()
        {
            Assert.Equal("loremipsum", "    lorem    ipsum    ".CollapseWhiteSpaces());
        }

        [Fact]
        public static void CollapseWhiteSpacesToCamelHumps()
        {
            Assert.Equal(TheQuickBrownFoxCamelCase, TheQuickBrownFox.CollapseWhiteSpacesToCamelHumps());
        }

        [Fact]
        public static void CollapseWhiteSpacesToOneSymbols()
        {
            Assert.Equal(" lorem ipsum ", "    lorem    ipsum    ".CollapseWhiteSpaces(1));
        }

        [Fact]
        public static void CollapseWhiteSpacesToTwoSymbols()
        {
            Assert.Equal("  lorem  ipsum  ", "    lorem    ipsum    ".CollapseWhiteSpaces(2));
        }

        [Fact]
        public static void Count()
        {
            Assert.Equal(2, LoremIpsumLowerCase.Count('m'));
            Assert.Equal(4, TheQuickBrownFox.Count('o'));
        }

        [Fact]
        public static void Decapitalize()
        {
            Assert.Equal("lorem", "Lorem".Decapitalize());
            Assert.Equal("l", "L".Decapitalize());
            Assert.Equal(string.Empty, string.Empty.Decapitalize());
            Assert.Equal(string.Empty, ((string)null).Decapitalize());
        }

        [Fact]
        public static void Deparenthesize()
        {
            Assert.Equal("A", "(A)".Deparenthesize());
            Assert.Equal("(A", "(A".Deparenthesize());
            Assert.Equal("A)", "A)".Deparenthesize());
            Assert.Equal("A", "A".Deparenthesize());

            Assert.Equal(string.Empty, string.Empty.Deparenthesize());
            Assert.Equal(string.Empty, ((string)null).Deparenthesize());
        }

        [Fact]
        public static void Distance()
        {
            const string origin = "Test Levenstein Distance";
            const string sample = "Dezd Levenzdein Tizdance";

            Assert.Equal(origin.Distance(sample), 8f);
        }

        [Fact]
        public static void EmptySubstringOrSelf()
        {
            Assert.Equal("ABC", "ABC".SubstringBeforeOrSelf(string.Empty));
            Assert.Equal("ABC", "ABC".SubstringBeforeLastOrSelf(string.Empty));
            Assert.Equal("ABC", "ABC".SubstringAfterOrSelf(string.Empty));
            Assert.Equal("ABC", "ABC".SubstringAfterLastOrSelf(string.Empty));
        }

        [Fact]
        public static void EnsureQuotes()
        {
            Assert.Equal("_._", "._".EnsureStartsWith('_'));
            Assert.Equal("_._", "_._".EnsureStartsWith('_'));

            Assert.Equal("_._", "_._".EnsureEndsWith('_'));
            Assert.Equal("_._", "_.".EnsureEndsWith('_'));
        }

        [Fact]
        public static void EnsureQuotesForEmptyStrings()
        {
            Assert.Equal("_", string.Empty.EnsureStartsWith('_'));
            Assert.Equal("_", string.Empty.EnsureEndsWith('_'));

            Assert.Equal("|", "|".EnsureStartsWith(string.Empty));
            Assert.Equal("|", "|".EnsureEndsWith(string.Empty));
        }

        [Fact]
        public static void EnsureQuotesForNullStrings()
        {
            Assert.Equal("_", ((string)null).EnsureStartsWith('_'));
            Assert.Equal("_", ((string)null).EnsureEndsWith('_'));

            Assert.Equal("|", "|".EnsureStartsWith(null));
            Assert.Equal("|", "|".EnsureEndsWith(null));
        }

        [Fact]
        public static void EqualsIgnoreCase()
        {
            Assert.True("Class".EqualsIgnoreCase("class"));
            Assert.True("class".EqualsIgnoreCase("class"));
            Assert.False("class".EqualsIgnoreCase("struct"));
        }

        [Fact]
        public static void Escape()
        {
            Assert.Equal("C:\\\\\\\"Program Files\\\"\\\\", "C:\\\"Program Files\"\\".Escape());
        }

        [Fact]
        public static void ExpandCamelHumps()
        {
            Assert.Equal("The Quick Brown Fox Jumps Over The Lazy Dog", "TheQuickBrownFoxJumpsOverTheLazyDog".ExpandCamelHumps());
        }

        [Fact]
        public static void ExpandTabs()
        {
            Assert.Equal("a    z", "a\tz".ExpandTabs(4));
        }

        [Fact]
        public static void Indent()
        {
            Assert.Equal(ClassFooMethodBarIndent1, ClassFooMethodBar.Indent());
            Assert.Equal(ClassFooMethodBarIndent2, ClassFooMethodBar.Indent(2));
        }

        [Fact]
        public static void Left()
        {
            Assert.Equal("Foo", "Foo bar".Left(3));
        }

        [Fact]
        public static void LeftInverse()
        {
            Assert.Equal("Foo", "Foo bar".Left(-4));
        }

        [Fact]
        public static void LeftOverflow()
        {
            Assert.Equal("Foo", "Foo".Left(42));
            Assert.Equal("Foo", "Foo".Left(-42));
        }

        [Fact]
        public static void Quote()
        {
            Assert.Equal("_._", "_._".Quote('_'));
            Assert.Equal("_._", "_.".Quote('_'));
            Assert.Equal("_._", "._".Quote('_'));
            Assert.Equal("_._", ".".Quote('_'));

            Assert.Equal("__", string.Empty.Quote('_', true));
            Assert.Equal(string.Empty, string.Empty.Quote('_', false));
            Assert.Equal(".", ".".Quote(string.Empty));

            Assert.Equal("__", ((string)null).Quote('_', true));
            Assert.Equal(string.Empty, ((string)null).Quote('_', false));
            Assert.Equal(".", ".".Quote(null));
        }

        [Fact]
        public static void RemoveLeft()
        {
            Assert.Equal("bar", "Foo bar".RemoveLeft(4));
        }

        [Fact]
        public static void RemoveOverflow()
        {
            Assert.Equal(string.Empty, "Foo bar".RemoveRight(10));
            Assert.Equal(string.Empty, "Foo bar".RemoveLeft(10));
        }

        [Fact]
        public static void RemoveRight()
        {
            Assert.Equal("Foo", "Foo bar".RemoveRight(4));
        }

        [Fact]
        public static void RemoveLeftVersusSubstring()
        {
            Assert.Equal(string.Empty, "Foo bar".RemoveLeft(10));

            Assert.Throws<ArgumentOutOfRangeException>(() => "Foo bar".Substring(10));
        }

        [Fact]
        public static void ReplaceCharacters()
        {
            Assert.Equal("_lorem______ipsum__", "+lorem@#$%^&ipsum--".ReplaceCharacters("_", "+@#$%^&-"));

            Assert.Equal("+lorem@#$%^&ipsum--", "+lorem@#$%^&ipsum--".ReplaceCharacters("_", string.Empty));
            Assert.Equal("+lorem@#$%^&ipsum--", "+lorem@#$%^&ipsum--".ReplaceCharacters("_", (string)null));
            Assert.Equal("+lorem@#$%^&ipsum--", "+lorem@#$%^&ipsum--".ReplaceCharacters("_", (char[])null));
        }

        [Fact]
        public static void ReplaceCharactersPerf()
        {
            const int tryes = 1000000;

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var i in Enumerable.Range(1, tryes))
            {
                "+lorem@#$%^&ipsum--".ReplaceCharacters("_", '+', '@', '#', '$', '%', '^', '&', '-');
            }

            stopwatch.Stop();

            var old = stopwatch.Elapsed;

            stopwatch.Restart();
            foreach (var i in Enumerable.Range(1, tryes))
            {
                "+lorem@#$%^&ipsum--".ReplaceCharacters("_", "+@#$%^&-");
            }

            stopwatch.Stop();

            Assert.True(stopwatch.Elapsed < old);
        }

        [Fact]
        public static void Right()
        {
            Assert.Equal("bar", "Foo bar".Right(3));
        }

        [Fact]
        public static void RightInverse()
        {
            Assert.Equal("bar", "Foo bar".Right(-4));
        }

        [Fact]
        public static void RightOverflow()
        {
            Assert.Equal("Foo", "Foo".Right(42));
            Assert.Equal("Foo", "Foo".Right(-42));
        }

        [Fact]
        public static void Trim()
        {
            Assert.Equal("TObject", "TObjectHelper".TrimEnd("Helper"));
            Assert.Equal("ObjectHelper", "TObjectHelper".TrimStart("T"));
        }

        [Fact]
        public static void TrimOrNull()
        {
            Assert.Null(string.Empty.TrimOrNull());
            Assert.Null(string.Empty.TrimStartOrNull());
            Assert.Null(string.Empty.TrimEndOrNull());

            Assert.Equal("A", " A ".TrimOrNull());
            Assert.Equal("A ", " A ".TrimStartOrNull());
            Assert.Equal(" A", " A ".TrimEndOrNull());

            Assert.Null("ABC".TrimStartOrNull("ABC"));
            Assert.Null("ABC".TrimEndOrNull("ABC"));

            Assert.Equal("BC", "ABC".TrimStartOrNull("A"));
            Assert.Equal("AB", "ABC".TrimEndOrNull("C"));
        }

        [Fact]
        public static void Unindent()
        {
            Assert.Equal(ClassFooMethodBar, ClassFooMethodBarIndent1.Unindent());
            Assert.Equal(ClassFooMethodBar, ClassFooMethodBarIndent2.Unindent(2));
        }

        [Theory]
        [InlineData("\"_\"")]
        [InlineData("'_'")]
        [InlineData("\u00AB_\u00BB")]
        [InlineData("\u2039_\u203A")]
        ////[InlineData("\u201E_\u201C")]
        ////[InlineData("\u201C_\u201D")]
        ////[InlineData("\u2018_\u2019")]
        ////[InlineData("\u201A_\u201B")]
        //[InlineData("\u201A_\u2018")]
        [InlineData("\u201F_\u201D")]
        public static void Unquote(string s)
        {
            Assert.Equal("_", s.Unquote());
        }

        [Theory]
        [InlineData("_\"")]
        [InlineData("_'")]
        [InlineData("_\u00BB")]
        [InlineData("_\u203A")]
        [InlineData("_\u201D")]
        public static void UnquoteNotQuoted(string s)
        {
            Assert.Equal(s, s.Unquote());
        }

        [Fact]
        public static void Unwrap()
        {
            Assert.Equal("_ _", "_\n_".Unwrap());
            Assert.Equal("_ _", "_\r_".Unwrap());
            Assert.Equal("_  _", "_\r\n_".Unwrap());
            Assert.Equal("_ _", "_\r\n_".Unwrap(true));
            Assert.Equal("_ _", "_\r\n_".Unwrap(true));
        }
    }
}