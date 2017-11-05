using Xunit;

namespace Exader.IO
{
    public class FilePathPropertiesFacts
    {
        [Theory]
        [InlineData(@"c:", @"C:", "", "")]
        [InlineData(@"c:\", @"C:", @"\", "")]
        [InlineData(@"c:\file", @"C:", @"\", @"file")]
        [InlineData(@"c:\dir\", @"C:", @"\", @"dir\")]
        [InlineData(@"\\server", @"\\SERVER", @"\", "")]
        [InlineData(@"\\server\", @"\\SERVER", @"\", "")]
        [InlineData(@"\\server\share\", @"\\SERVER", @"\share\", "")]
        [InlineData(@"\\server\share\file", @"\\SERVER", @"\share\", @"file")]
        [InlineData(@"\\server\share\dir\", @"\\SERVER", @"\share\", @"dir\")]
        [InlineData(@"file", "", "", @"file")]
        [InlineData(@".\file", "", "", @"file")]
        [InlineData(@"..\file", "", "", @"..\file")]
        [InlineData(@".\..\file", "", "", @"..\file")]
        [InlineData(@"dir\", "", "", @"dir\")]
        [InlineData(@".\dir\", "", "", @"dir\")]
        [InlineData(@"..\dir\", "", "", @"..\dir\")]
        [InlineData(@".\..\dir\", "", "", @"..\dir\")]
        [InlineData(@"dir\.\", "", "", @"dir\")]
        [InlineData(@"dir\.\.\", "", "", @"dir\")]
        [InlineData(@"dir\..\", "", "", @"dir\..\")]
        [InlineData(@"dir\..\.\", "", "", @"dir\..\")]
        [InlineData(@"dir\..\.\.\", "", "", @"dir\..\")]
        [InlineData(@"dir\...\", "", "", @"dir\...\")]
        [InlineData(@"dir\....\", "", "", @"dir\....\")]
        [InlineData(@"dir1\dir2\..\..\", "", "", @"dir1\dir2\..\..\")]
        public void Canonicalize(string v, string r, string f, string p)
        {
            Assert.Equal(p, FilePath.CanonicalizePath(v, out var driveOrHost, out var rootFolder));
            Assert.Equal(f, rootFolder);
            Assert.Equal(r, driveOrHost);
        }

        [Theory]
        [InlineData("..")]
        [InlineData("../.")]
        [InlineData("../..")]
        [InlineData("../f")]
        [InlineData("../d/")]
        public void IsExternal_True(string p)
        {
            Assert.True(FilePath.Parse(p).IsExternal);
        }

        [Theory]
        [InlineData("")]
        [InlineData(".")]
        [InlineData("/..")]
        [InlineData("/../.")]
        [InlineData("/../..")]
        public void IsExternal_False(string p)
        {
            Assert.False(FilePath.Parse(p).IsExternal);
        }

        [Fact]
        public void FileExtension()
        {
            Assert.Equal("", FilePath.Parse("f").FileExtension);
            Assert.Equal(".e", FilePath.Parse("f.e").FileExtension);
            Assert.Equal(".e", FilePath.Parse("f.r.e").FileExtension);
            Assert.Equal("", FilePath.Parse("d/").FileExtension);
            Assert.Equal("", FilePath.Parse("d.e/").FileExtension);
            Assert.Equal("", FilePath.Parse("d.r.e/").FileExtension);
            Assert.Equal(".e", FilePath.Parse(".e").FileExtension);
            Assert.Equal("", FilePath.Parse(".e/").FileExtension);
            Assert.Equal("", FilePath.Parse("").FileExtension);
            Assert.Equal("", FilePath.Parse(".").FileExtension);
            Assert.Equal("", FilePath.Parse("..").FileExtension);
            Assert.Equal("", FilePath.Parse("../").FileExtension);
            Assert.Equal("", FilePath.Parse("../.").FileExtension);
            Assert.Equal("", FilePath.Parse("../..").FileExtension);
            Assert.Equal("", FilePath.Parse("/").FileExtension);
        }

        [Fact]
        public void Basename()
        {
            Assert.Equal("f", FilePath.Parse("f").Basename);
            Assert.Equal("f", FilePath.Parse("f.e").Basename);
            Assert.Equal("f.r", FilePath.Parse("f.r.e").Basename);
            Assert.Equal("d", FilePath.Parse("d/").Basename);
            Assert.Equal("d.e", FilePath.Parse("d.e/").Basename);
            Assert.Equal("d.r.e", FilePath.Parse("d.r.e/").Basename);
            Assert.Equal("", FilePath.Parse(".e").Basename);
            Assert.Equal(".e", FilePath.Parse(".e/").Basename);
            Assert.Equal("", FilePath.Parse("").Basename);
            Assert.Equal("", FilePath.Parse(".").Basename);
            Assert.Equal("..", FilePath.Parse("..").Basename);
            Assert.Equal("..", FilePath.Parse("../").Basename);
            Assert.Equal("..", FilePath.Parse("../.").Basename);
            Assert.Equal("..", FilePath.Parse("../..").Basename);
            Assert.Equal("", FilePath.Parse("/").Basename);
        }

        [Fact]
        public void DirectoryPath_Directory()
        {
            Assert.Equal(@"d\sd\", FilePath.Parse("c:/d/sd/").DirectoryPath);
            Assert.Equal(@"d\sd\", FilePath.Parse("/d/sd/").DirectoryPath);
            Assert.Equal(@"d\sd\", FilePath.Parse("d/sd/").DirectoryPath);
            Assert.Equal(@"sd\", FilePath.Parse("/sd/").DirectoryPath);
            Assert.Equal(@"sd\", FilePath.Parse("sd/").DirectoryPath);
            Assert.Equal(@"", FilePath.Parse("./").DirectoryPath);
            Assert.Equal(@"", FilePath.Parse(".").DirectoryPath);
            Assert.Equal(@"", FilePath.Parse("").DirectoryPath);
        }

        [Fact]
        public void DirectoryPath_File()
        {
            Assert.Equal(@"d\", FilePath.Parse("c:/d/f.e").DirectoryPath);
            Assert.Equal(@"d\", FilePath.Parse("/d/f.e").DirectoryPath);
            Assert.Equal(@"d\", FilePath.Parse("d/f.e").DirectoryPath);
            Assert.Equal(@"", FilePath.Parse("f.e").DirectoryPath);
        }

        [Fact]
        public void DriveLetter()
        {
            var fp1 = FilePath.Parse("c:\\d");

            Assert.True(fp1.HasDriveOrHost);
            Assert.Equal("c:", fp1.Drive);
            Assert.Equal('c', fp1.DriveLetter);

            var fp2 = FilePath.Parse("\\d");

            Assert.False(fp2.HasDriveOrHost);
            Assert.Empty(fp2.Drive);
            Assert.Null(fp2.DriveLetter);
        }

        [Fact]
        public void Extension()
        {
            Assert.Equal("", FilePath.Parse("f").Extension);
            Assert.Equal(".e", FilePath.Parse("f.e").Extension);
            Assert.Equal(".e", FilePath.Parse("f.r.e").Extension);
            Assert.Equal("", FilePath.Parse("d/").Extension);
            Assert.Equal(".e", FilePath.Parse("d.e/").Extension);
            Assert.Equal(".e", FilePath.Parse("d.r.e/").Extension);
            Assert.Equal(".e", FilePath.Parse(".e").Extension);
            Assert.Equal(".e", FilePath.Parse(".e/").Extension);
            Assert.Equal("", FilePath.Parse("").Extension);
            Assert.Equal("", FilePath.Parse(".").Extension);
            Assert.Equal("", FilePath.Parse("..").Extension);
            Assert.Equal("", FilePath.Parse("../").Extension);
            Assert.Equal("", FilePath.Parse("../.").Extension);
            Assert.Equal("", FilePath.Parse("../..").Extension);
            Assert.Equal("", FilePath.Parse("/").Extension);
        }

        [Fact]
        public void IsAbsolute()
        {
            Assert.True(FilePath.Parse("c:/").IsAbsolute);
            Assert.True(FilePath.Parse("//s/s/").IsAbsolute);

            Assert.False(FilePath.Parse("").IsAbsolute);
            Assert.False(FilePath.Parse(".").IsAbsolute);
            Assert.False(FilePath.Parse("..").IsAbsolute);
            Assert.False(FilePath.Parse("/").IsAbsolute);
            Assert.False(FilePath.Parse("f").IsAbsolute);
            Assert.False(FilePath.Parse("d/").IsAbsolute);
            Assert.False(FilePath.Parse("c:").IsAbsolute);
        }

        [Fact]
        public void IsCurrent()
        {
            Assert.True(FilePath.Parse("").IsCurrent);
            Assert.True(FilePath.Parse(".").IsCurrent);
            Assert.True(FilePath.Parse("d/..").IsCurrent);
            Assert.True(FilePath.Parse("c:").IsCurrent);

            Assert.False(FilePath.Parse("..").IsCurrent);
            Assert.False(FilePath.Parse("../.").IsCurrent);
            Assert.False(FilePath.Parse("../..").IsCurrent);
            Assert.False(FilePath.Parse("/").IsCurrent);
            Assert.False(FilePath.Parse("f").IsCurrent);
            Assert.False(FilePath.Parse("d/").IsCurrent);
            Assert.False(FilePath.Parse("c:/").IsCurrent);
            Assert.False(FilePath.Parse("c:..").IsCurrent);
            Assert.False(FilePath.Parse("//s/s/").IsCurrent);
        }

        [Fact]
        public void IsDirectory()
        {
            Assert.True(FilePath.Parse("").IsDirectory);
            Assert.True(FilePath.Parse(".").IsDirectory);
            Assert.True(FilePath.Parse("..").IsDirectory);
            Assert.True(FilePath.Parse("../.").IsDirectory);
            Assert.True(FilePath.Parse("../..").IsDirectory);
            Assert.True(FilePath.Parse("/").IsDirectory);
            Assert.True(FilePath.Parse("/.").IsDirectory);
            Assert.True(FilePath.Parse("/..").IsDirectory);
            Assert.True(FilePath.Parse("/../").IsDirectory);
            Assert.True(FilePath.Parse("d/").IsDirectory);
            Assert.True(FilePath.Parse("/d/").IsDirectory);
            Assert.True(FilePath.Parse("/d/sd/").IsDirectory);
            Assert.True(FilePath.Parse("c:/").IsDirectory);
            Assert.True(FilePath.Parse("c:/d/").IsDirectory);
            Assert.True(FilePath.Parse("c:/d/sd/").IsDirectory);
            Assert.True(FilePath.Parse("//s/s/").IsDirectory);
            Assert.True(FilePath.Parse("//s/s/d/").IsDirectory);
            Assert.True(FilePath.Parse("//s/s/d/sd/").IsDirectory);

            Assert.False(FilePath.Parse("f").IsDirectory);
            Assert.False(FilePath.Parse("/f").IsDirectory);
            Assert.False(FilePath.Parse("./f").IsDirectory);
            Assert.False(FilePath.Parse("../f").IsDirectory);
            Assert.False(FilePath.Parse("d/f").IsDirectory);
            Assert.False(FilePath.Parse("/d/f").IsDirectory);
            Assert.False(FilePath.Parse("c:/d/f").IsDirectory);
            Assert.False(FilePath.Parse("//s/s/f").IsDirectory);
            Assert.False(FilePath.Parse("//s/s/d/f").IsDirectory);
        }

        [Fact]
        public void IsExternal()
        {
            var fp = FilePath.Parse("../../..");

            Assert.NotNull(fp.Parent);
            Assert.NotNull(fp.Parent.Parent);
            Assert.Null(fp.Parent.Parent.Parent);
            Assert.True(fp.IsExternal);
            Assert.True(fp.Parent.IsExternal);
            Assert.True(fp.Parent.Parent.IsExternal);
        }

        [Fact]
        public void IsLocal()
        {
            Assert.True(FilePath.Parse("c:").IsLocal);
            Assert.True(FilePath.Parse("c:/").IsLocal);
            Assert.False(FilePath.Parse("//s/s/").IsLocal);
            Assert.False(FilePath.Parse("").IsLocal);
            Assert.False(FilePath.Parse(".").IsLocal);
            Assert.False(FilePath.Parse("..").IsLocal);
            Assert.False(FilePath.Parse("/").IsLocal);
        }

        [Fact]
        public void IsNetwork()
        {
            Assert.True(FilePath.Parse("//s/s/").IsNetwork);
            Assert.False(FilePath.Parse("c:/").IsNetwork);
            Assert.False(FilePath.Parse("").IsNetwork);
            Assert.False(FilePath.Parse(".").IsNetwork);
            Assert.False(FilePath.Parse("..").IsNetwork);
            Assert.False(FilePath.Parse("/").IsNetwork);
        }

        [Fact]
        public void NameWithoutExtension()
        {
            Assert.Equal("f", FilePath.Parse("f").NameWithoutExtension);
            Assert.Equal("f", FilePath.Parse("f.e").NameWithoutExtension);
            Assert.Equal("f.r", FilePath.Parse("f.r.e").NameWithoutExtension);
            Assert.Equal("d", FilePath.Parse("d/").NameWithoutExtension);
            Assert.Equal("d", FilePath.Parse("d.e/").NameWithoutExtension);
            Assert.Equal("d.r", FilePath.Parse("d.r.e/").NameWithoutExtension);
            Assert.Equal("", FilePath.Parse(".e").NameWithoutExtension);
            Assert.Equal("", FilePath.Parse(".e/").NameWithoutExtension);
            Assert.Equal("", FilePath.Parse("").NameWithoutExtension);
            Assert.Equal("", FilePath.Parse(".").NameWithoutExtension);
            Assert.Equal("..", FilePath.Parse("..").NameWithoutExtension);
            Assert.Equal("..", FilePath.Parse("../").NameWithoutExtension);
            Assert.Equal("..", FilePath.Parse("../.").NameWithoutExtension);
            Assert.Equal("..", FilePath.Parse("../..").NameWithoutExtension);
            Assert.Equal("", FilePath.Parse("/").NameWithoutExtension);
        }

        [Fact]
        public void NameWithoutExtensions()
        {
            var p = FilePath.Parse("f.r.e");

            Assert.Equal("f", p.NameWithoutExtensions(".r.e"));
            Assert.Equal("f", p.NameWithoutExtensions("r.e"));
            Assert.Equal("f.r", p.NameWithoutExtensions(".e"));
            Assert.Equal("f.r", p.NameWithoutExtensions("e"));
            Assert.Equal("f.r.e", p.NameWithoutExtensions(".x"));
            Assert.Equal("f.r.e", p.NameWithoutExtensions("x"));
            Assert.Equal("f.r.e", p.NameWithoutExtensions(""));
            Assert.Equal("f.r.e", p.NameWithoutExtensions());

            Assert.Equal("f.r", p.NameWithoutExtensions(".e", ".r"));
        }
    }
}
