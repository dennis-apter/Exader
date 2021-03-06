using System;
using System.IO;
using Xunit;

namespace Exader.IO
{
    public class FilePathConversionFacts
    {
        [Theory]
        [InlineData("", "d:/c")]
        [InlineData("c:/d", "d:/c")]
        [InlineData("//s/s/d", "//h/s/c")]
        public void Combine_BothIsAbsolute(string l, string r)
        {
            Assert.Throws<ArgumentException>(() => FilePath.Combine(l, r));
        }

        [Theory]
        [InlineData(@"c:\d\sd\", "", @"c:\d\sd\")]
        [InlineData(@"\d\f", "", @"\d\f")]
        [InlineData("", @"\d\f", @"\d\f")]
        public void Combine(string l, string r, string f)
        {
            Assert.Equal(f, ((FilePath) l / (FilePath) r).ToWindowsString());
        }

        [Theory]
        [InlineData(@"c:\d\sd\", "", @"c:\d\sd\.e")]
        [InlineData(@"\d\f", "", @"\d\f\.e")]
        [InlineData("", @"\d\f", @"\d\f.e")]
        public void Combine_Or(string l, string r, string f)
        {
            Assert.Equal(f, ((FilePath) l | (r + ".e")).ToWindowsString());
        }

        [Theory]
        [InlineData(@"c:\d\sd\", null)]
        [InlineData(@"\d\f", null)]
        [InlineData(null, @"\d\f")]
        [InlineData(null, null)]
        public void Combine_Null(string a, string b)
        {
            Assert.Throws<ArgumentNullException>(() => FilePath.Combine(a, b));
        }

        [Theory]
        [InlineData("c:/d/sd/", "d:/d/")]
        [InlineData("/d/sd/", "d:/d/")]
        [InlineData("d/sd/", "d:/d/")]
        [InlineData("c:/d/sd/", "d/")]
        [InlineData("//s/s/d/sd/", "//h/s/d/")]
        [InlineData("/d/sd/", "//h/s/d/")]
        [InlineData("d/sd/", "//h/s/d/")]
        [InlineData("//s/s/d/sd/", "/d/")]
        [InlineData("//s/s/d/sd/", "d/")]
        public void ToRelative_DifferentRoots(string p, string b)
        {
            Assert.Throws<ArgumentException>(() => FilePath.Parse(p) % b);
            Assert.Throws<ArgumentException>(() => FilePath.Parse(p).ToRelativeWindowsString(b));
        }

        [Theory]
        [InlineData("//s/s/d/sd/", "//s/s/d/x/", @"..\sd\")]
        [InlineData("//s/s/d/sd/f.e", "//s/s/d/x/", @"..\sd\f.e")]
        [InlineData("//s/s/d/sd/ssd/", "//s/s/d/x/", @"..\sd\ssd\")]
        [InlineData("c:/d/sd/", "c:/d/x/", @"..\sd\")]
        [InlineData("c:/d/sd/f.e", "c:/d/x/", @"..\sd\f.e")]
        [InlineData("c:/d/sd/ssd/", "c:/d/x/", @"..\sd\ssd\")]
        [InlineData("/d/sd/", "/d/x/", @"..\sd\")]
        [InlineData("/d/sd/f.e", "/d/x/", @"..\sd\f.e")]
        [InlineData("/d/sd/ssd/", "/d/x/", @"..\sd\ssd\")]
        [InlineData("d/sd/", "d/x/", @"..\sd\")]
        [InlineData("d/sd/f.e", "d/x/", @"..\sd\f.e")]
        [InlineData("d/sd/ssd/", "d/x/", @"..\sd\ssd\")]
        public void ToRelative_DirectoryBase(string p, string b, string r)
        {
            var fp = (FilePath) p;
            var result = fp % b;
            var resultString = fp.ToRelativeWindowsString(b);

            Assert.True(result.IsExternal);
            Assert.Equal(r, result.ToWindowsString());
            Assert.Equal(r, resultString);
            Assert.Equal(fp, (b / result));
            Assert.Equal(fp, ((FilePath) b / resultString));
        }

        [Theory]
        [InlineData("//s/s/d/sd/", "//s/s/d/f.e", @"..\sd\")]
        [InlineData("//s/s/d/sd/f.e", "//s/s/d/f.e", @"..\sd\f.e")]
        [InlineData("//s/s/d/sd/ssd/", "//s/s/d/f.e", @"..\sd\ssd\")]
        [InlineData("c:/d/sd/", "c:/d/f.e", @"..\sd\")]
        [InlineData("c:/d/sd/f.e", "c:/d/f.e", @"..\sd\f.e")]
        [InlineData("c:/d/sd/ssd/", "c:/d/f.e", @"..\sd\ssd\")]
        [InlineData("/d/sd/", "/d/f.e", @"..\sd\")]
        [InlineData("/d/sd/f.e", "/d/f.e", @"..\sd\f.e")]
        [InlineData("/d/sd/ssd/", "/d/f.e", @"..\sd\ssd\")]
        [InlineData("d/sd/", "d/f.e", @"..\sd\")]
        [InlineData("d/sd/f.e", "d/f.e", @"..\sd\f.e")]
        [InlineData("d/sd/ssd/", "d/f.e", @"..\sd\ssd\")]
        public void ToRelative_FileBase(string p, string b, string r)
        {
            var fp = (FilePath) p;
            var result = fp % b;
            var resultString = fp.ToRelativeWindowsString(b);

            Assert.True(result.IsExternal);
            Assert.Equal(r, result.ToWindowsString());
            Assert.Equal(r, resultString);
            Assert.Equal(fp, b / result);
            Assert.Equal(fp, (FilePath) b / resultString);
        }

        [Theory]
        [InlineData("c:/d/sd/", "d:/d/")]
        [InlineData("/d/sd/", "d:/d/")]
        [InlineData("d/sd/", "d:/d/")]
        [InlineData("c:/d/sd/", "d/")]
        [InlineData("//s/s/d/sd/", "//h/s/d/")]
        [InlineData("/d/sd/", "//h/s/d/")]
        [InlineData("d/sd/", "//h/s/d/")]
        [InlineData("//s/s/d/sd/", "/d/")]
        [InlineData("//s/s/d/sd/", "d/")]
        public void TryToRelative_DifferentRoots(string p, string b)
        {
            Assert.False(FilePath.Parse(p).TryToRelative(b, out var fp));
            Assert.Null(fp);

            string s;
            Assert.False(FilePath.Parse(p).TryToRelativeWindowsString(b, out s));
            Assert.Null(s);
        }

        [Fact]
        public void CastEmptyString()
        {
            var emptyFilePath = (FilePath) "";
            Assert.Equal(FilePath.Empty, emptyFilePath);
        }

        [Fact]
        public void CastNullString()
        {
            string nullString = null;
            var nullFilePath = (FilePath) nullString;
            Assert.Null(nullFilePath);

            nullString = nullFilePath;
            Assert.Null(nullString);
        }

        [Fact]
        public void ToAbsoluteString()
        {
            var currentDir = Path.GetFullPath(".\\");
            var pathInfo = FilePath.Parse("f.e");

            Assert.False(pathInfo.IsAbsolute);
            Assert.Equal(currentDir + @"f.e", pathInfo.ToAbsoluteString());
        }
    }
}