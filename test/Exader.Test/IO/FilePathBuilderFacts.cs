using System;
using Xunit;

namespace Exader.IO
{
    public class FilePathBuilderFacts
    {
        [Fact]
        public void CannotAddRootDirAsChild()
        {
            Assert.Throws<InvalidOperationException>(() => new FilePath.Builder("foo").Add(new FilePath.Builder()));
        }

        [Theory]
        [InlineData(".e")]
        [InlineData("f.e")]
        [InlineData("d/f.e")]
        public void CompactLongFileNameToShortestInvalidForm(string path)
        {
            Assert.Throws<InvalidOperationException>(() => 
                FilePath.Builder.ParseUnix(path).Root().Trim(path.Length - 1, 1));
        }

        [Theory]
        [InlineData("file.e", "fi….e")]
        [InlineData("file.e", "f….e")]
        [InlineData("file.e", "….e")]
        [InlineData("d/file.e", "d/fi….e")]
        [InlineData("d/file.e", "d/f….e")]
        [InlineData("d/file.e", "d/….e")]
        [InlineData("dir/file.e", "dir/fi….e")]
        [InlineData("dir/file.e", "dir/f….e")]
        [InlineData("dir/file.e", "dir/….e")]
        [InlineData("dir/file.e", "d…/….e")]
        [InlineData("dir/file.e", "…/….e")]
        [InlineData("/dir/file.e", "/dir/fi….e")]
        [InlineData("/dir/file.e", "/dir/f….e")]
        [InlineData("/dir/file.e", "/dir/….e")]
        [InlineData("/dir/file.e", "/d…/….e")]
        [InlineData("/dir/file.e", "/…/….e")]
        public void CompactLongFileNameToShortestValidForm(string path, string result)
        {
            var file = FilePath.Builder.ParseUnix(path);
            file.Root().Trim(result.Length, 1);

            Assert.Equal(result, file.ToUnixString());
        }

        [Fact]
        public void DirectoryMerge()
        {
            var root = new FilePath.Builder();
            var x = FilePath.Builder.ParseUnix("etc/x");
            var y = FilePath.Builder.ParseUnix("etc/y");

            root.Add(x.Root());
            Assert.Equal("\\etc\\x", x.ToWindowsString());

            root.Add(y.Root());
            Assert.Equal("\\etc\\x", x.ToWindowsString());
            Assert.Equal("\\etc\\y", y.ToWindowsString());
            Assert.True(ReferenceEquals(x.Parent, y.Parent));
        }

        [Fact]
        public void DoublesDirectoryMerge()
        {
            var root = new FilePath.Builder();
            var x1 = FilePath.Builder.ParseUnix("etc/x");
            var x2 = FilePath.Builder.ParseUnix("etc/x");

            root.Add(x1.Root());
            Assert.Equal("\\etc\\x", x1.ToWindowsString());

            root.Add(x2.Root());
            Assert.Equal("\\etc\\x(1)", x1.ToWindowsString());
            Assert.Equal("\\etc\\x(2)", x2.ToWindowsString());
            Assert.True(ReferenceEquals(x1.Parent, x2.Parent));
        }

        [Fact]
        public void FileDoubles()
        {
            var root = new FilePath.Builder();
            var x1 = new FilePath.Builder("x.doc");
            var x2 = new FilePath.Builder("x.doc");

            root.Add(x1);
            Assert.Equal("\\x.doc", x1.ToWindowsString());

            root.Add(x2);
            Assert.Equal("\\x(1).doc", x1.ToWindowsString());
            Assert.Equal("\\x(2).doc", x2.ToWindowsString());
        }

        [Fact]
        public void FolderDoubles()
        {
            var root = new FilePath.Builder();
            var x1 = new FilePath.Builder("x");
            var x2 = new FilePath.Builder("x");

            root.Add(x1);
            Assert.Equal("\\x", x1.ToWindowsString());

            root.Add(x2);
            Assert.Equal("\\x(1)", x1.ToWindowsString());
            Assert.Equal("\\x(2)", x2.ToWindowsString());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("d")]
        [InlineData("d/f.e")]
        [InlineData("d/.e")]
        [InlineData(".e")]
        [InlineData("dir/1234567890.ext")]
        public void Length(string path)
        {
            var p = FilePath.Builder.ParseUnix(path);
            Assert.Equal(path.Length, p.GetLength());
        }

        [Theory]
        [InlineData("f", "", "f", "")]
        [InlineData(".e", "", "", ".e")]
        [InlineData("f.e", "", "f", ".e")]
        [InlineData("d/f.e", @"d\", "f", ".e")]
        [InlineData("/d/f.e", @"\d\", "f", ".e")]
        [InlineData("c:/d/f.e", @"\d\", "f", ".e")]
        public void Parse(string value, string parentPath, string name, string ext)
        {
            var path = FilePath.Builder.ParseUnix(value);
            
            Assert.Equal(parentPath, path.ParentWindowsPath);
            Assert.Equal(name, path.NameWithoutExtension);
            Assert.Equal(ext, path.Extension);
        }

        [Fact]
        public void Relative()
        {
            Assert.Equal("foo", new FilePath.Builder("foo").ToUnixString());
        }

        [Fact]
        public void Root()
        {
            Assert.Equal("/", new FilePath.Builder().ToUnixString());
        }

        [Fact]
        public void Rooted()
        {
            Assert.Equal("/foo", new FilePath.Builder().Add("foo").ToUnixString());
        }

        [Fact]
        public void ToFilePath()
        {
            var path = FilePath.Builder.ParseUnix("c:/devdir/sqlton/src/Core/Core.csproj");

            Assert.Equal(@"\devdir\sqlton\src\Core\Core.csproj", path.ToFilePath().ToWindowsString());
        }
    }
}
