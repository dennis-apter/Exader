using System;
using Xunit;

namespace Exader.IO
{
    public class FilePathParsingFacts
    {
        [Theory]
        [InlineData(@".\f", "f")]
        [InlineData(@".\..\f", @"..\f")]
        [InlineData(@".\f.e", "f.e")]
        [InlineData(@".\..\f.e", @"..\f.e")]
        [InlineData(@".\.e", ".e")]
        [InlineData(@".\..\.e", @"..\.e")]
        [InlineData(@".", "")]
        [InlineData(@".\.", "")]
        [InlineData(@"..\.", @"..\")]
        [InlineData(@".\..\.", @"..\")]
        [InlineData(@".\d\", @"d\")]
        [InlineData(@".\..\d\", @"..\d\")]
        [InlineData(@".\..\d\.", @"..\d\")]
        [InlineData(@".\..\d\..", @"..\")]
        [InlineData(@"d\.", @"d\")]
        [InlineData(@"d\.\", @"d\")]
        [InlineData(@"d\.\.", @"d\")]
        [InlineData(@"d\.\.\", @"d\")]
        [InlineData(@"d\..", "")]
        [InlineData(@"d\..\", "")]
        [InlineData(@"d\..\.\", "")]
        [InlineData(@"d\..\..", "..\\")]
        [InlineData(@"d\..\..\", "..\\")]
        [InlineData(@"d\..\.\.\", "")]
        [InlineData(@"d\...\", "")]
        [InlineData(@"d\....\", "")]
        [InlineData(@"d1\d2\..\..", "")]
        [InlineData(@"d1\d2\..\..\", "")]
        public void CollapseRelativePath(string value, string result)
        {
            var fp = FilePath.Parse(value);
            Assert.Equal(result, fp);
            Assert.Empty(fp.RootFolder);

            fp = FilePath.Parse("C:" + value);
            // relative path to the current directory on the drive
            Assert.Equal("C:" + result, fp);
            Assert.Empty(fp.RootFolder);
        }

        [Theory]
        [InlineData(@"\..", @"\")]
        [InlineData(@"c:\..", @"c:\")]
        [InlineData(@"c:\..\", @"c:\")]
        [InlineData(@"c:\..\..\", @"c:\")]
        [InlineData(@"\\h\r\..", @"\\h\r\")]
        [InlineData(@"\\h\r\..\", @"\\h\r\")]
        [InlineData(@"\\h\r\..\..\", @"\\h\r\")]
        public void CollapseRelativePathOutOfRootFolder(string value, string result)
        {
            var fp = FilePath.Parse(value);

            Assert.Equal(result, fp);
            Assert.True(fp.IsRoot);
        }

        [Theory]
        [InlineData(@"")]
        [InlineData(@".")]
        [InlineData(@".\")]
        [InlineData(@"d\..")]
        public void CurrentDirectory(string value)
        {
            var fp = FilePath.Parse(value);

            Assert.Empty(fp.Name);
            Assert.Empty(fp.Extension);
            Assert.True(fp.IsDirectory);
            Assert.True(fp.IsCurrent);
            Assert.False(fp.IsRoot);
        }

        [Theory]
        [InlineData(@"\.", @"")]
        [InlineData(@"..\", @"..")]
        [InlineData(@"..\.", @"..")]
        [InlineData(@"d\", @"d")]
        [InlineData(@"\d\", @"d")]
        [InlineData(@"\d1\d2\", @"d2")]
        [InlineData(@"d\.", @"d")]
        [InlineData(@".\d\", @"d")]
        [InlineData(@"\d1\d2\.", @"d2")]
        [InlineData(@"\d1\.\d2\.", @"d2")]
        [InlineData(@"\d1\..\d2\.", @"d2")]
        [InlineData(@"\d1\..\..\d2\.", @"d2")]
        [InlineData(@"c:d\", @"d")]
        [InlineData(@"c:\d\", @"d")]
        [InlineData(@"c:\d\", @"d")]
        [InlineData(@"c:\d\.", @"d")]
        [InlineData(@"c:\d\.\", @"d")]
        [InlineData(@"c:\d\..\", @"")]
        [InlineData(@"\\h\r\d\", @"d")]
        public void Directory(string value, string name)
        {
            var fp = FilePath.Parse(value);

            Assert.Equal(name, fp.Name);
            Assert.True(fp.IsDirectory);
            Assert.False(fp.IsCurrent);
        }

        [Theory]
        [InlineData(".\\", "")]
        [InlineData("..\\", "..")]
        [InlineData("...\\", "..")]
        [InlineData("....\\", "..")]
        [InlineData("..e\\", ".")]
        [InlineData("...e\\", "..")]
        [InlineData("....e\\", "...")]
        public void DirectoryNameWithDots(string value, string result)
        {
            var fp = FilePath.Parse(value);
            Assert.Equal(result, fp.NameWithoutExtension);
        }

        [Theory]
        [InlineData(@"file://c|\Program%20Files\", @"c:\Program Files\")]
        public void EncodedFileUri(string value, string result)
        {
            Assert.Equal(result, FilePath.Parse(value));
        }

        [Theory]
        [InlineData("..e", ".")]
        [InlineData("...e", "..")]
        [InlineData("....e", "...")]
        public void FileNameWithDots(string value, string result)
        {
            var fp = FilePath.Parse(value);
            Assert.Equal(result, fp.NameWithoutExtension);
        }

        [Theory]
        [InlineData(">f\ri\nl\ve\t")]
        [InlineData(">\r\n\v\tfile")]
        [InlineData("file>\r\n\v\t")]
        [InlineData("file>%0A%0D\v\t")]
        [InlineData("file>%0a%0d\v\t")]
        public void InvalidCharacters(string s)
        {
            var ex = Assert.Throws<ArgumentException>(() => FilePath.Parse(s));
            Assert.Equal("Invalid file path character '>'.", ex.Message);

            Assert.False(FilePath.TryParse(s, out var result, out var error));
            Assert.Equal(FilePathParseErrorType.InvalidCharacter, error.ErrorType);
        }

        [Theory]
        [InlineData(@"xyz:\")]
        [InlineData(@"file://xyz:\")]
        [InlineData(@"file://xyz|\")]
        [InlineData(@"file://localhost/xyz:\")]
        [InlineData(@"file://localhost/xyz|\")]
        public void InvalidDriveLetter(string s)
        {
            var ex = Assert.Throws<ArgumentException>(() => FilePath.Parse(s));
            Assert.Contains("xyz", ex.Message);

            Assert.False(FilePath.TryParse(s, out var result, out var error));
            Assert.Equal(FilePathParseErrorType.InvalidDriveLetter, error.ErrorType);
        }

        [Theory]
        [InlineData(@"\\?\xz:\")]
        [InlineData(@"\\?\file:\")]
        [InlineData(@"\\?\UNCx\")]
        public void InvalidLongPathPrefix(string s)
        {
            Assert.Throws<ArgumentException>(() => FilePath.Parse(s));

            Assert.False(FilePath.TryParse(s, out var result, out var error));
            Assert.Equal(FilePathParseErrorType.InvalidLongPathPrefix, error.ErrorType);
        }

        [Theory]
        [InlineData(@"file://localhost/c:\Windows\")]
        [InlineData(@"file://localhost/c|\Windows\")]
        [InlineData(@"file:///localhost/c|\Windows\")]
        [InlineData(@"file:////localhost/c|\Windows\")]
        [InlineData(@"file://////localhost/c|\Windows\")]
        [InlineData(@"file:\localhost\c|\Windows\")]
        [InlineData(@"file:\\localhost\c|\Windows\")]
        [InlineData(@"file:\\\localhost\c|\Windows\")]
        [InlineData(@"file:\\\\localhost\c|\Windows\")]
        [InlineData(@"file:\\\\\localhost\c|\Windows\")]
        [InlineData(@"file:\\\\\\localhost\c|\Windows\")]
        [InlineData(@"file:\/\localhost\c|\Windows\")]
        [InlineData(@"file:/\/\localhost\c|\Windows\")]
        [InlineData(@"file:\/\/\localhost\c|\Windows\")]
        [InlineData(@"file:\////\localhost\c|\Windows\")]
        public void LocalFileUri(string value)
        {
            foreach (var fp in new[]
            {
                FilePath.Parse(value),
                FilePath.Parse(value.Replace("localhost\\", string.Empty))
            })
            {
                Assert.Equal(@"c:\Windows\", fp);
                Assert.Equal("Windows", fp.Name);
                Assert.Equal("\\", fp.RootFolder);
                Assert.Equal("c:", fp.Drive);
                Assert.Empty(fp.Host);
                Assert.Empty(fp.Extension);
                Assert.Empty(fp.ParentPath);
                Assert.True(fp.IsDirectory);
                Assert.True(fp.IsLocal);
                Assert.False(fp.IsNetwork);
            }
        }

        [Theory(Skip = "TODO")]
        [InlineData(@"\\?\c:\d\", @"c:\d\")]
        [InlineData(@"\\?\UNC\h\r\", @"\\h\r\")]
        public void LongPath(string value, string result)
        {
            Assert.Equal(result, FilePath.Parse(value));
        }

        [Theory]
        [InlineData(@"file://h/r/d/")]
        [InlineData(@"file:///h/r/d/")]
        [InlineData(@"file:////h/r/d/")]
        [InlineData(@"file://///h/r/d/")]
        [InlineData(@"file://////h/r/d/")]
        [InlineData(@"file:///////h/r/d/")]
        [InlineData(@"file:////////h/r/d/")]
        [InlineData(@"file://///////h/r/d/")]
        [InlineData(@"file://////////h/r/d/")]
        [InlineData(@"file:/\/\/\/\/\/h/r/d/")]
        [InlineData(@"file:///\\\///\\\h/r/d/")]
        public void NetworkFileUri(string value)
        {
            var fp = FilePath.Parse(value);
            {
                Assert.Equal(@"\\h\r\d\", fp);
                Assert.Equal("d", fp.Name);
                Assert.Equal("\\r\\", fp.RootFolder);
                Assert.Equal("\\\\h", fp.Host);
                Assert.Empty(fp.Drive);
                Assert.Empty(fp.Extension);
                Assert.Empty(fp.ParentPath);
                Assert.True(fp.IsDirectory);
                Assert.False(fp.IsLocal);
                Assert.True(fp.IsNetwork);
            }
        }

        [Theory]
        [InlineData(@"\\h", @"\\h")]
        [InlineData(@"\\h\", @"\\h\")]
        [InlineData(@"\\h\r\", @"\\h\r\")]
        [InlineData(@"\\h\r\f", @"\\h\r\f")]
        [InlineData(@"\\h\r\f.e", @"\\h\r\f.e")]
        [InlineData(@"\\h\r\d\", @"\\h\r\d\")]
        [InlineData(@"\\h\r\d\..", @"\\h\r\")]
        public void NetworkShare(string value, string result)
        {
            Assert.Equal(result, FilePath.Parse(value));
        }

        [Theory]
        [InlineData(@"..\")]
        [InlineData(@"..\..\")]
        [InlineData(@"..\..\..\")]
        [InlineData(@"..\d\")]
        [InlineData(@"..\f")]
        [InlineData(@"..\f.e")]
        [InlineData(@"..\.e")]
        public void NonCollapseRelativePath(string value)
        {
            var fp = FilePath.Parse(value);
            Assert.Equal(value, fp);
            Assert.Empty(fp.RootFolder);
            Assert.True(fp.IsExternal);

            fp = FilePath.Parse("C:" + value);
            Assert.Equal("C:" + value, fp);
            Assert.Empty(fp.RootFolder);
            Assert.True(fp.IsExternal);
        }

        [Theory]
        [InlineData(@"c:", @"c:")]
        [InlineData(@"c:\", @"c:\")]
        [InlineData(@"c:\f", @"c:\f")]
        [InlineData(@"c:\d\", @"c:\d\")]
        [InlineData(@"f", "f")]
        [InlineData(@"f.e", "f.e")]
        [InlineData(@".e", ".e")]
        [InlineData(@"d\", @"d\")]
        public void Regural(string value, string result)
        {
            Assert.Equal(result, FilePath.Parse(value));
        }

        [Theory]
        [InlineData(@"\")]
        [InlineData(@"\.")]
        [InlineData(@"\d\..")]
        [InlineData(@"c:\")]
        [InlineData(@"c:\d\..")]
        [InlineData(@"\\h\r\")]
        [InlineData(@"\\h\r\..")]
        [InlineData(@"\\h\r\d\..")]
        public void RootFolder(string value)
        {
            var fp = FilePath.Parse(value);

            Assert.Empty(fp.Name);
            Assert.Empty(fp.Extension);
            Assert.True(fp.IsDirectory);
            Assert.False(fp.IsCurrent);
            Assert.True(fp.IsRoot);
            Assert.True(fp.HasRootFolder);
        }

        [Theory]
        [InlineData("f. ", "f")]
        [InlineData(@"d. \", @"d\")]
        [InlineData(@"d.\f.", @"d\f")]
        [InlineData(@"d1 . \d2 . \", @"d1\d2\")]
        [InlineData(@"c:\f.", @"c:\f")]
        [InlineData(@"c:\d . \", @"c:\d\")]
        [InlineData(@"c:\d . \f.", @"c:\d\f")]
        [InlineData(@"c:\d1 . \d2 . \", @"c:\d1\d2\")]
        public void TrimEnds(string value, string result)
        {
            Assert.Equal(result, FilePath.Parse(value));
        }

        [Theory]
        [InlineData(@"\\?\UNC\h\r\", @"\\h\r\")]
        [InlineData(@"\\?\c:\", @"c:\")]
        public void Unc(string value, string result)
        {
            var fp = FilePath.Parse(value);
            Assert.Equal(result, fp);
        }

        [Fact]
        public void Extension()
        {
            var fp = FilePath.Parse("foo.bar");
            Assert.Equal(".bar", fp.Extension);
            Assert.Equal("foo.bar", fp.Name);
            Assert.Equal("foo", fp.NameWithoutExtension);

            fp = FilePath.Parse("foo.bar/");
            Assert.True(fp.IsDirectory);
            Assert.Equal(".bar", fp.Extension);
            Assert.Equal("foo.bar", fp.Name);
            Assert.Equal("foo", fp.NameWithoutExtension);
        }
    }
}