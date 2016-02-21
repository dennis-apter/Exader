using System;
using System.Linq;
using Xunit;

namespace Exader.IO
{
    public class FilePathTransformationFacts
    {
        [Fact]
        public void Ancestor()
        {
            var p = FilePath.Parse("a/b/c");

            Assert.Equal(p.Parent, p.Ancestor(1));
            Assert.Equal(p.Parent.Parent, p.Ancestor(2));
        }

        [Fact]
        public void Ancestor_OutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FilePath.Parse("f").Ancestor(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => FilePath.Parse("f").Ancestor(1));
        }

        [Fact]
        public void Ancestors()
        {
            Assert.Equal(2, FilePath.Parse("a/b/c").Ancestors().Count());
        }

        [Fact]
        public void AncestorsAndSelf()
        {
            Assert.Equal(3, FilePath.Parse("a/b/c").AncestorsAndSelf().Count());
        }

        [Theory]
        [InlineData("a/b/c/d/e/f", "f", @"a\b\c\d\e\")]
        [InlineData("a/b/c/d/e/f", "e", @"a\b\c\d\")]
        [InlineData("a/b/c/d/e/f", "c", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "b", @"a\")]
        [InlineData("a/b/c/d/e/f", "a", @"")]
        [InlineData("a/b/c/d/e/f", "c/d", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "c/d/e", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "c/d/e/f", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "/f/", @"a\b\c\d\e\")]
        [InlineData("a/b/c/d/e/f", "/e/", @"a\b\c\d\")]
        [InlineData("a/b/c/d/e/f", "/c/", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "/b/", @"a\")]
        [InlineData("a/b/c/d/e/f", "/a/", @"")]
        [InlineData("a/b/c/d/e/f", "/c/d/", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "/c/d/e/", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "/c/d/e/f/", @"a\b\")]
        public void SubpathBefore(string p, string sp, string r)
        {
            Assert.Equal(r, FilePath.Parse(p).SubpathBefore(sp));
        }

        [Theory]
        [InlineData("a/b/c/d/e/f", "f", @"a\b\c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "e", @"a\b\c\d\e\")]
        [InlineData("a/b/c/d/e/f", "c", @"a\b\c\")]
        [InlineData("a/b/c/d/e/f", "b", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "a", @"a\")]
        [InlineData("a/b/c/d/e/f", "c/d", @"a\b\c\d\")]
        [InlineData("a/b/c/d/e/f", "c/d/e", @"a\b\c\d\e\")]
        [InlineData("a/b/c/d/e/f", "c/d/e/f", @"a\b\c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "/f/", @"a\b\c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "/e/", @"a\b\c\d\e\")]
        [InlineData("a/b/c/d/e/f", "/c/", @"a\b\c\")]
        [InlineData("a/b/c/d/e/f", "/b/", @"a\b\")]
        [InlineData("a/b/c/d/e/f", "/a/", @"a\")]
        [InlineData("a/b/c/d/e/f", "/c/d/", @"a\b\c\d\")]
        [InlineData("a/b/c/d/e/f", "/c/d/e/", @"a\b\c\d\e\")]
        [InlineData("a/b/c/d/e/f", "/c/d/e/f/", @"a\b\c\d\e\f")]
        public void SubpathBefore_Inclusive(string p, string sp, string r)
        {
            var fp = FilePath.Parse(p);
            Assert.Equal(r, fp.SubpathBefore(sp, true).ToString());
        }

        [Fact]
        public void Parent()
        {
            Assert.Equal(@"\", FilePath.Parse(@"\f").Parent);
            Assert.Equal(@"\", FilePath.Parse(@"\d\").Parent);
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\f").Parent);
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\f.e").Parent);
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\.e").Parent);
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\sd\").Parent);

            Assert.Null(FilePath.Parse("").Parent);
            Assert.Null(FilePath.Parse(".").Parent);
            Assert.Null(FilePath.Parse(".\\").Parent);
            Assert.Null(FilePath.Parse("..\\").Parent);
            Assert.Null(FilePath.Parse("c:").Parent);
            Assert.Null(FilePath.Parse("c:.\\").Parent);
            Assert.Null(FilePath.Parse("c:..\\").Parent);
            Assert.Null(FilePath.Parse("c:f").Parent);
            Assert.Null(FilePath.Parse("c:d\\").Parent);

            Assert.Null(FilePath.Parse(@"\").Parent);
            Assert.Null(FilePath.Parse(@"\..").Parent);
            Assert.Null(FilePath.Parse(@"c:\").Parent);
            Assert.Null(FilePath.Parse(@"\\server\folder\").Parent);
        }

        [Fact]
        public void Without_Directory()
        {
            var p = FilePath.Parse("c:/d/sd/");

            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHost());
            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHostAsString());

            Assert.Equal(@"\d\sd", p.WithoutDriveOrHostAndExtension());
            Assert.Equal(@"\d\sd", p.WithoutDriveOrHostAndExtensionAsString());

            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndName());
            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndNameAsString());

            Assert.Equal(@"d\sd\", p.WithoutRootFolder());
            Assert.Equal(@"d\sd\", p.WithoutRootFolderAsString());

            Assert.Equal(@"d\sd", p.WithoutRootFolderAndExtension());
            Assert.Equal(@"d\sd", p.WithoutRootFolderAndExtensionAsString());

            Assert.Equal(@"d\", p.WithoutRootFolderAndName());
            Assert.Equal(@"d\", p.WithoutRootFolderAndNameAsString());

            Assert.Equal(@"c:\d\sd", p.WithoutExtension());
            Assert.Equal(@"c:\d\sd", p.WithoutExtensionAsString());

            Assert.Equal(@"c:\d\", p.WithoutName());
            Assert.Equal(@"c:\d\", p.WithoutNameAsString());

            Assert.Equal(@"c:\d\sd\", p.WithoutFileName());
            Assert.Equal(@"c:\d\sd\", p.WithoutFileNameAsString());

            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHostAndFileName());
            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHostAndFileNameAsString());

            Assert.Equal(@"d\sd\", p.WithoutRootFolderAndFileName());
            Assert.Equal(@"d\sd\", p.WithoutRootFolderAndFileNameAsString());
        }

        [Fact]
        public void Without_File()
        {
            var p = FilePath.Parse("c:/d/f.e");

            Assert.Equal(@"\d\f.e", p.WithoutDriveOrHost());
            Assert.Equal(@"\d\f.e", p.WithoutDriveOrHostAsString());

            Assert.Equal(@"\d\f", p.WithoutDriveOrHostAndExtension());
            Assert.Equal(@"\d\f", p.WithoutDriveOrHostAndExtensionAsString());

            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndName());
            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndNameAsString());

            Assert.Equal(@"d\f.e", p.WithoutRootFolder());
            Assert.Equal(@"d\f.e", p.WithoutRootFolderAsString());

            Assert.Equal(@"d\f", p.WithoutRootFolderAndExtension());
            Assert.Equal(@"d\f", p.WithoutRootFolderAndExtensionAsString());

            Assert.Equal(@"d\", p.WithoutRootFolderAndName());
            Assert.Equal(@"d\", p.WithoutRootFolderAndNameAsString());

            Assert.Equal(@"c:\d\f", p.WithoutExtension());
            Assert.Equal(@"c:\d\f", p.WithoutExtensionAsString());

            Assert.Equal(@"c:\d\", p.WithoutName());
            Assert.Equal(@"c:\d\", p.WithoutNameAsString());

            Assert.Equal(@"c:\d\", p.WithoutFileName());
            Assert.Equal(@"c:\d\", p.WithoutFileNameAsString());

            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndFileName());
            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndFileNameAsString());

            Assert.Equal(@"d\", p.WithoutRootFolderAndFileName());
            Assert.Equal(@"d\", p.WithoutRootFolderAndFileNameAsString());
        }
    }
}
