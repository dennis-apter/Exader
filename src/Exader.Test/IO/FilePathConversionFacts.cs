using System;
using Xunit;

namespace Exader.IO
{
    public class FilePathConversionFacts
    {
        [Theory]
        [InlineData("c:/d", "d:/c")]
        [InlineData("//s/s/d", "//h/s/c")]
        public void Combine_BothIsAbsolute(string a, string b)
        {
            Assert.Throws<ArgumentException>(() => FilePath.Combine(a, b));
        }

        [Fact]
        public void Combine_WithEmpty()
        {
            var basePath = (FilePath)@"c:\d\sd";

            Assert.Equal(basePath, basePath / string.Empty);
            Assert.Equal(basePath, basePath / FilePath.Empty);
            Assert.Equal(basePath, basePath.Combine(string.Empty));
            Assert.Equal(basePath, basePath.Combine(FilePath.Empty));
        }

        [Fact]
        public void ToAbsoluteString()
        {
            var currentDir = Environment.CurrentDirectory;
            var pathInfo = FilePath.Parse("f.e");

            Assert.False(pathInfo.IsAbsolute);
            Assert.Equal(currentDir + @"\f.e", pathInfo.ToAbsoluteString());
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
            Assert.Throws<ArgumentException>(() => FilePath.Parse(p).ToRelativeString(b));
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
            var fp = (FilePath)p;
            var result = fp % b;
            var resultString = fp.ToRelativeString(b);

            Assert.True(result.IsExternal);
            Assert.Equal(r, result);
            Assert.Equal(r, resultString);
            Assert.Equal(fp, b / result);
            Assert.Equal(fp, (FilePath)b / resultString);
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
            var resultString = fp.ToRelativeString(b);

            Assert.True(result.IsExternal);
            Assert.Equal(r, result);
            Assert.Equal(r, resultString);
            Assert.Equal(fp, b / result);
            Assert.Equal(fp, (FilePath)b / resultString);
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
            FilePath fp;
            Assert.False(FilePath.Parse(p).TryToRelative(b, out fp));
            Assert.Null(fp);

            string s;
            Assert.False(FilePath.Parse(p).TryToRelativeString(b, out s));
            Assert.Null(s);
        }
    }
}
