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

        [Fact]
        public void AsDirectory()
        {
            Assert.Equal(@"a\b\c\", FilePath.Parse("a/b/c").AsDirectory());
            Assert.Equal(@"a\b\c\", FilePath.Parse("a/b/c/").AsDirectory());
        }

        [Fact]
        public void AsRooted()
        {
            Assert.Equal(@"\a\b\c", FilePath.Parse("a/b/c").AsRooted());
            Assert.Equal(@"c:\a\b\c", FilePath.Parse("c:a/b/c").AsRooted());
            Assert.Equal(@"\a\b\c", FilePath.Parse("/a/b/c").AsRooted());
            Assert.Equal(@"c:\a\b\c", FilePath.Parse("c:/a/b/c").AsRooted());
            Assert.Equal(@"\\h\a\b\c", FilePath.Parse("//h/a/b/c").AsRooted());
        }

        [Theory]
        [InlineData("/d")]
        [InlineData("c:/d")]
        public void ChangeRoot(string filePath)
        {
            var fp = FilePath.Parse(filePath);

            Assert.Equal(@"a:\d", fp.WithRoot("a"));
            Assert.Equal(@"a:\d", fp.WithRoot("a:/"));
            Assert.Equal(@"a:\d", fp.WithRoot(@"a:\"));
            Assert.Equal(@"a:\d", fp.WithRoot(@"\\?\a:\"));

            var result = fp.WithRoot("a");
            Assert.Equal("a:", result.DriveOrHost);
            Assert.Equal(@"\", result.RootFolder);

            Assert.Equal(@"\d", fp.WithRoot(""));
            Assert.Equal(@"\d", fp.WithRoot(null));

            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\\\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\\\\s\s\"));
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\?\UNC\s\s\"));

            result = fp.WithRoot(@"\\s\s\");
            Assert.Equal(@"\\s", result.DriveOrHost);
            Assert.Equal(@"\s\", result.RootFolder);
        }

        [Fact]
        public void ChangeRoot_InvalidDriveLetter()
        {
            var fp = FilePath.Parse("c:/d");

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => fp.WithRoot("1"));
            Assert.Contains("1", ex.Message);

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => fp.WithRoot("1:/"));
            Assert.Contains("1:/", ex.Message);

            var ex2 = Assert.Throws<ArgumentException>(() => fp.WithRoot("foo"));
            Assert.Contains("foo", ex2.Message);
        }

        [Fact]
        public void ChangeRoot_InvalidNetworkShare()
        {
            var ex = Assert.Throws<ArgumentException>(() => FilePath.Parse("d").WithRoot("//s"));
            Assert.Contains("//s", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => FilePath.Parse("d").WithRoot("//s/s/s"));
            Assert.Contains("//s/s/s", ex.Message);
        }

        [Fact]
        public void ChangeRoot_Rootless()
        {
            var fp = FilePath.Parse("d");

            var r1 = fp.WithRoot("a:");
            Assert.Equal("a:d", r1);
            Assert.Equal("a:", r1.Drive);
            Assert.Equal('a', r1.DriveLetter);
            Assert.Empty(r1.Host);
            Assert.Empty(r1.RootFolder);

            var r2 = fp.WithRoot(@"\\s\s\");
            Assert.Equal(@"\\s\s\d", r2);
            Assert.Empty(r2.Drive);
            Assert.Null(r2.DriveLetter);
            Assert.Equal(@"\\s", r2.Host);
            Assert.Equal(@"\s\", r2.RootFolder);
        }

        [Fact]
        public void Item()
        {
            Assert.Equal(@"C:\dir\file.e", FilePath.Parse("C:/dir/").File("file.e"));
            Assert.Equal(@"C:\dir\file.e", FilePath.Parse("C:/dir/f.e").File("file.e"));
            Assert.Equal(@"C:\dir\dir.e\", FilePath.Parse("C:/dir/").Directory("dir.e"));
            Assert.Equal(@"C:\dir\dir.e\", FilePath.Parse("C:/dir/f.e").Directory("dir.e"));
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
        public void SubpathAfter()
        {
            Assert.Equal(FilePath.Parse("bar/baz"), FilePath.Parse("c:/foo/bar/baz").SubpathAfter());
            Assert.Equal(FilePath.Parse("baz"), FilePath.Parse("c:/foo/bar/baz").SubpathAfter(2));

            Assert.Throws<ArgumentOutOfRangeException>(() => FilePath.Parse("c:/foo/bar/baz").SubpathAfter(3));
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
        public void WithExtension()
        {
            Assert.Equal("foo.x", FilePath.Parse("foo").WithExtension("x"));
            Assert.Equal("foo.x\\", FilePath.Parse("foo/").WithExtension("x"));
            Assert.Equal("foo.x", FilePath.Parse("foo.bar").WithExtension("x"));
            Assert.Equal(".x", FilePath.Parse("foo.bar").WithExtension("x").Extension);
            Assert.Equal(".x", FilePath.Parse("foo.bar").WithExtension(".x").Extension);
            Assert.Equal(string.Empty, FilePath.Parse("foo.bar").WithExtension(string.Empty).Extension);
        }

        [Fact]
        public void WithExtensionPrefix()
        {
            Assert.Equal("foo.x.bar", FilePath.Parse("foo.bar").WithExtensionPrefix("x").ToString());
            Assert.Equal("foo.x.bar", FilePath.Parse("foo.bar").WithExtensionPrefix(".x").ToString());
        }

        [Fact]
        public void WithExtensionSuffix()
        {
            Assert.Equal("foo.bar.x", FilePath.Parse("foo.bar").WithExtensionSuffix("x"));
        }

        [Fact]
        public void WithName()
        {
            Assert.Equal(@"C:\dir\b.e", FilePath.Parse("C:/dir/file.e").WithName("b"));
            Assert.Equal(@"C:\dir\.e", FilePath.Parse("C:/dir/file.e").WithName(null));
            Assert.Equal(@"C:\dir\.f", FilePath.Parse("C:/dir/file.e").WithName(".f"));
            Assert.Equal(@"C:\dir\_._", FilePath.Parse("C:/dir/file.e").WithName("_._"));

            Assert.Equal(@"C:\dir\b\", FilePath.Parse("C:/dir/subdir.e/").WithName("b"));
            Assert.Throws<ArgumentException>(() => FilePath.Parse("C:/dir/subdir.e/").WithName(null));
            Assert.Equal(@"C:\dir\.f\", FilePath.Parse("C:/dir/subdir.e/").WithName(".f"));
            Assert.Equal(@"C:\dir\_._\", FilePath.Parse("C:/dir/subdir.e/").WithName("_._"));
        }

        [Fact]
        public void WithNamePrefix()
        {
            var fp = FilePath.Parse("foo.bar");

            Assert.Equal("gfoo.bar", fp.WithNamePrefix("g").ToString());
            Assert.Equal(".gfoo.bar", fp.WithNamePrefix(".g").ToString());
            Assert.Equal("g.foo.bar", fp.WithNamePrefix("g.").ToString());
            Assert.Equal(".g.foo.bar", fp.WithNamePrefix(".g.").ToString());
        }

        [Fact]
        public void WithNameSuffix()
        {
            Assert.Equal("foo.bar.x", FilePath.Parse("foo.bar").WithNameSuffix("x").ToString());
            Assert.Equal("foo.bar.x", FilePath.Parse("foo.bar").WithNameSuffix(".x").ToString());
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

        [Fact]
        public void WithoutDriveOrHostAndExtension()
        {
            var p1 = FilePath.Parse("c:/d/a.b.c");
            Assert.Equal(@"\d\a.b", p1.WithoutDriveOrHostAndExtensionAsString());

            var p2 = p1.WithoutDriveOrHostAndExtension();
            Assert.Equal(@"\d\a.b", p2);
            Assert.Equal(@"\d\a", p2.WithoutDriveOrHostAndExtensionAsString());
            Assert.Equal(@".b", p2.Extension);

            var p3 = p2.WithoutDriveOrHostAndExtension();
            Assert.Equal(@"\d\a", p3);
            Assert.Equal(@"\d\a", p3.WithoutDriveOrHostAndExtensionAsString());
            Assert.Equal(@"", p3.Extension);
            Assert.Equal(@"\d\a", p3.WithoutDriveOrHostAndExtension());

            var p4 = FilePath.Parse("c:/d/a.b.c/");
            Assert.Equal(@"\d\a.b", p4.WithoutDriveOrHostAndExtensionAsString());
            Assert.True(p4.IsDirectory);

            var p5 = p4.WithoutDriveOrHostAndExtension();
            Assert.Equal(@"\d\a.b", p5);
            Assert.False(p5.IsDirectory);
            Assert.Equal(@".b", p5.Extension);
        }

        [Fact]
        public void WithoutExtension()
        {
            var p1 = FilePath.Parse("c:/d/a.b.c");
            Assert.Equal(@"c:\d\a.b", p1.WithoutExtensionAsString());

            var p2 = p1.WithoutExtension();
            Assert.Equal(@"c:\d\a.b", p2);
            Assert.Equal(@"c:\d\a", p2.WithoutExtensionAsString());
            Assert.Equal(@".b", p2.Extension);

            var p3 = p2.WithoutExtension();
            Assert.Equal(@"c:\d\a", p3);
            Assert.Equal(@"c:\d\a", p3.WithoutExtensionAsString());
            Assert.Equal(@"", p3.Extension);
            Assert.Equal(@"c:\d\a", p3.WithoutExtension());

            var p4 = FilePath.Parse("c:/d/a.b.c/");
            Assert.Equal(@"c:\d\a.b", p4.WithoutExtensionAsString());
            Assert.True(p4.IsDirectory);

            var p5 = p4.WithoutExtension();
            Assert.Equal(@"c:\d\a.b", p5);
            Assert.False(p5.IsDirectory);
            Assert.Equal(@".b", p5.Extension);
        }

        [Fact]
        public void WithoutExtensions()
        {
            var path = FilePath.Parse("foo.bar.baz");

            Assert.Equal("foo.bar", path.WithoutExtensions());
            Assert.Equal("foo", path.WithoutExtensions(2));

            Assert.Throws<ArgumentOutOfRangeException>(() => path.WithoutExtensions(3));
        }
    }
}
