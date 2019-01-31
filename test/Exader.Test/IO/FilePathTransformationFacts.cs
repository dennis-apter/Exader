using System;
using System.Linq;
using Xunit;

namespace Exader.IO
{
    public class FilePathTransformationFacts
    {
        [Theory]
        [InlineData("/d")]
        [InlineData("c:/d")]
        public void WithRoot(string filePath)
        {
            var fp = FilePath.Parse(filePath);

            Assert.Equal(@"a:\d", fp.WithRoot("a").ToWindowsString());
            Assert.Equal(@"a:\d", fp.WithRoot("a:/").ToWindowsString());
            Assert.Equal(@"a:\d", fp.WithRoot(@"a:\").ToWindowsString());
            Assert.Equal(@"a:\d", fp.WithRoot(@"\\?\a:\").ToWindowsString());

            var result = fp.WithRoot("a");
            Assert.Equal("a:", result.DriveOrHostWindows);
            Assert.Equal(@"\", result.RootFolderWindows);

            Assert.Equal(@"\d", fp.WithRoot("").ToWindowsString());
            Assert.Equal(@"\d", fp.WithRoot(null).ToWindowsString());

            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\\\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\\\\\\s\s\").ToWindowsString());
            Assert.Equal(@"\\s\s\d", fp.WithRoot(@"\\?\UNC\s\s\").ToWindowsString());

            result = fp.WithRoot(@"\\s\s\");
            Assert.Equal(@"\\s", result.DriveOrHostWindows);
            Assert.Equal(@"\s\", result.RootFolderWindows);
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
            Assert.Equal(r, FilePath.Parse(p).SubpathBefore(sp).ToWindowsString());
        }

        [Theory(Skip = "TODO")]
        [InlineData("a/b/c/d/e/f", "f", @"")]
        [InlineData("a/b/c/d/e/f", "e", @"f\")]
        [InlineData("a/b/c/d/e/f", "c", @"d\e\f")]
        [InlineData("a/b/c/d/e/f", "b", @"c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "a", @"b\c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "c/d", @"e\f")]
        [InlineData("a/b/c/d/e/f", "c/d/e", @"f")]
        [InlineData("a/b/c/d/e/f", "c/d/e/f", @"")]
        [InlineData("a/b/c/d/e/f", "/f/", @"")]
        [InlineData("a/b/c/d/e/f", "/e/", @"f")]
        [InlineData("a/b/c/d/e/f", "/c/", @"d\e\f")]
        [InlineData("a/b/c/d/e/f", "/b/", @"c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "/a/", @"b\c\d\e\f")]
        [InlineData("a/b/c/d/e/f", "/c/d/", @"e\f")]
        [InlineData("a/b/c/d/e/f", "/c/d/e/", @"f")]
        [InlineData("a/b/c/d/e/f", "/c/d/e/f/", @"")]
        public void SubpathAfter(string p, string sp, string r)
        {
            // TODO Assert.Equal(r, FilePath.Parse(p).SubpathAfter(sp));
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
            Assert.Equal(r, fp.SubpathBefore(sp, true).ToWindowsString());
        }

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
            Assert.Equal(@"a\b\c\", FilePath.Parse("a/b/c").AsDirectory().ToWindowsString());
            Assert.Equal(@"a\b\c\", FilePath.Parse("a/b/c/").AsDirectory().ToWindowsString());
        }

        [Fact]
        public void AsRooted()
        {
            Assert.Equal(@"\a\b\c", FilePath.Parse("a/b/c").AsRooted().ToWindowsString());
            Assert.Equal(@"c:\a\b\c", FilePath.Parse("c:a/b/c").AsRooted().ToWindowsString());
            Assert.Equal(@"\a\b\c", FilePath.Parse("/a/b/c").AsRooted().ToWindowsString());
            Assert.Equal(@"c:\a\b\c", FilePath.Parse("c:/a/b/c").AsRooted().ToWindowsString());
            Assert.Equal(@"\\h\a\b\c", FilePath.Parse("//h/a/b/c").AsRooted().ToWindowsString());
        }

        [Fact]
        public void AsLocal()
        {
            Assert.Equal(@"a\b\c", FilePath.Parse("/a/b/c").AsLocal().ToWindowsString());
            Assert.Equal(@"a\b\c", FilePath.Parse("c:/a/b/c").AsLocal().ToWindowsString());
            Assert.Equal(@"a\b\c", FilePath.Parse("c:a/b/c").AsLocal().ToWindowsString());
            Assert.Equal(@"a\b\c", FilePath.Parse("a/b/c").AsLocal().ToWindowsString());
            Assert.Equal(@"b\c", FilePath.Parse("//h/a/b/c").AsLocal().ToWindowsString());
        }

        [Fact]
        public void Item()
        {
            Assert.Equal(@"C:\dir\file.e", FilePath.Parse("C:/dir/").File("file.e").ToWindowsString());
            Assert.Equal(@"C:\dir\file.e", FilePath.Parse("C:/dir/f.e").File("file.e").ToWindowsString());
            Assert.Equal(@"C:\dir\dir.e\", FilePath.Parse("C:/dir/").Directory("dir.e").ToWindowsString());
            Assert.Equal(@"C:\dir\dir.e\", FilePath.Parse("C:/dir/f.e").Directory("dir.e").ToWindowsString());
        }

        [Fact]
        public void Parent()
        {
            Assert.Equal(@"\", FilePath.Parse(@"\f").Parent.ToWindowsString());
            Assert.Equal(@"\", FilePath.Parse(@"\d\").Parent.ToWindowsString());
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\f").Parent.ToWindowsString());
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\f.e").Parent.ToWindowsString());
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\.e").Parent.ToWindowsString());
            Assert.Equal(@"\d\", FilePath.Parse(@"\d\sd\").Parent.ToWindowsString());

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
        public void WithBasename()
        {
            var fp = FilePath.Parse("C:/dir/f.e");
            Assert.Equal(@"C:\dir\b.e", fp.WithBasename("b").ToWindowsString());
            Assert.Equal(@"C:\dir\.f.e", fp.WithBasename(".f").ToWindowsString());
            Assert.Equal(@"C:\dir\_._.e", fp.WithBasename("_._").ToWindowsString());

            Assert.Throws<ArgumentException>(() => fp.WithBasename(null));
            Assert.Throws<ArgumentException>(() => fp.WithBasename(""));
            Assert.Throws<ArgumentException>(() => fp.WithBasename("."));
            Assert.Throws<ArgumentException>(() => fp.WithBasename(".."));
            Assert.Throws<ArgumentException>(() => fp.WithBasename("..."));

            var dp = FilePath.Parse("C:/dir/subdir.e/");
            Assert.Equal(@"C:\dir\b\", dp.WithBasename("b").ToWindowsString());
            Assert.Equal(@"C:\dir\.f\", dp.WithBasename(".f").ToWindowsString());
            Assert.Equal(@"C:\dir\_._\", dp.WithBasename("_._").ToWindowsString());

            Assert.Throws<ArgumentException>(() => dp.WithBasename(null));
            Assert.Throws<ArgumentException>(() => dp.WithBasename(""));
            Assert.Throws<ArgumentException>(() => dp.WithBasename("."));
            Assert.Throws<ArgumentException>(() => dp.WithBasename(".."));
            Assert.Throws<ArgumentException>(() => dp.WithBasename("..."));
        }

        [Fact]
        public void WithExtension()
        {
            Assert.Equal("foo.x", FilePath.Parse("foo").WithExtension("x"));
            Assert.Equal("foo.x\\", FilePath.Parse("foo/").WithExtension("x").ToWindowsString());
            Assert.Equal("foo.x", FilePath.Parse("foo.bar").WithExtension("x"));
            Assert.Equal(".x", FilePath.Parse("foo.bar").WithExtension("x").Extension);
            Assert.Equal(".x", FilePath.Parse("foo.bar").WithExtension(".x").Extension);
            Assert.Equal(string.Empty, FilePath.Parse("foo.bar").WithExtension(string.Empty).Extension);
        }

        [Fact]
        public void WithExtensionPrefix()
        {
            var p = FilePath.Parse("foo.bar");
            Assert.Equal("foo.x.bar", p.WithExtensionPrefix("x"));
            Assert.Equal("foo.x.bar", p.WithExtensionPrefix(".x"));
        }

        [Fact]
        public void WithExtensionSuffix()
        {
            var p = FilePath.Parse("foo.bar");
            Assert.Equal("foo.bar.x", p.WithExtensionSuffix("x"));
            Assert.Equal("foo.bar.x", p.WithExtensionSuffix(".x"));
            Assert.Equal(".x", p.WithExtensionSuffix("x").Extension);
        }

        [Fact]
        public void WithName()
        {
            var fp = FilePath.Parse("C:/dir/f.e");
            Assert.Equal(@"C:\dir\b", fp.WithName("b").ToWindowsString());
            Assert.Equal(@"C:\dir\.f", fp.WithName(".f").ToWindowsString());
            Assert.Equal(@"C:\dir\_._", fp.WithName("_._").ToWindowsString());

            Assert.Throws<ArgumentException>(() => fp.WithName(null));
            Assert.Throws<ArgumentException>(() => fp.WithName(""));
            Assert.Throws<ArgumentException>(() => fp.WithName("."));
            Assert.Throws<ArgumentException>(() => fp.WithName(".."));
            Assert.Throws<ArgumentException>(() => fp.WithName("..."));

            var dp = FilePath.Parse("C:/dir/d.e/");
            Assert.Equal(@"C:\dir\b\", dp.WithName("b").ToWindowsString());
            Assert.Equal(@"C:\dir\.f\", dp.WithName(".f").ToWindowsString());
            Assert.Equal(@"C:\dir\_._\", dp.WithName("_._").ToWindowsString());

            Assert.Throws<ArgumentException>(() => dp.WithName(null));
            Assert.Throws<ArgumentException>(() => dp.WithName(""));
            Assert.Throws<ArgumentException>(() => dp.WithName("."));
            Assert.Throws<ArgumentException>(() => dp.WithName(".."));
            Assert.Throws<ArgumentException>(() => dp.WithName("..."));
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
            var p = FilePath.Parse("foo.bar");
            Assert.Equal("foo.barx", p.WithNameSuffix("x").ToString());
            Assert.Equal("foo.bar.x", p.WithNameSuffix(".x").ToString());
        }

        [Fact]
        public void Without_Directory()
        {
            var p = FilePath.Parse("c:/d/sd/");

            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHost().ToWindowsString());
            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHostAsWindowsString());

            Assert.Equal(@"\d\sd", p.WithoutDriveOrHostAndExtension().ToWindowsString());
            Assert.Equal(@"\d\sd", p.WithoutDriveOrHostAndExtensionAsWindowsString());

            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndName().ToWindowsString());
            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndNameAsWindowsString());

            Assert.Equal(@"d\sd\", p.WithoutRootFolder().ToWindowsString());
            Assert.Equal(@"d\sd\", p.WithoutRootFolderAsWindowsString());

            Assert.Equal(@"d\sd", p.WithoutRootFolderAndExtension().ToWindowsString());
            Assert.Equal(@"d\sd", p.WithoutRootFolderAndExtensionAsWindowsString());

            Assert.Equal(@"d\", p.WithoutRootFolderAndName().ToWindowsString());
            Assert.Equal(@"d\", p.WithoutRootFolderAndNameAsWindowsString());

            Assert.Equal(@"c:\d\sd", p.WithoutExtension().ToWindowsString());
            Assert.Equal(@"c:\d\sd", p.WithoutExtensionAsWindowsString());

            Assert.Equal(@"c:\d\", p.WithoutName().ToWindowsString());
            Assert.Equal(@"c:\d\", p.WithoutNameAsWindowsString());

            Assert.Equal(@"c:\d\sd\", p.WithoutFileName().ToWindowsString());
            Assert.Equal(@"c:\d\sd\", p.WithoutFileNameAsWindowsString());

            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHostAndFileName().ToWindowsString());
            Assert.Equal(@"\d\sd\", p.WithoutDriveOrHostAndFileNameAsWindowsString());

            Assert.Equal(@"d\sd\", p.WithoutRootFolderAndFileName().ToWindowsString());
            Assert.Equal(@"d\sd\", p.WithoutRootFolderAndFileNameAsWindowsString());
        }

        [Fact]
        public void Without_File()
        {
            var p = FilePath.Parse("c:/d/f.e");

            Assert.Equal(@"\d\f.e", p.WithoutDriveOrHost().ToWindowsString());
            Assert.Equal(@"\d\f.e", p.WithoutDriveOrHostAsWindowsString());

            Assert.Equal(@"\d\f", p.WithoutDriveOrHostAndExtension().ToWindowsString());
            Assert.Equal(@"\d\f", p.WithoutDriveOrHostAndExtensionAsWindowsString());

            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndName().ToWindowsString());
            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndNameAsWindowsString());
            
            Assert.Equal(@"d\f.e", p.WithoutRootFolder().ToWindowsString());
            Assert.Equal(@"d\f.e", p.WithoutRootFolderAsWindowsString());

            Assert.Equal(@"d\f", p.WithoutRootFolderAndExtension().ToWindowsString());
            Assert.Equal(@"d\f", p.WithoutRootFolderAndExtensionAsWindowsString());

            Assert.Equal(@"d\", p.WithoutRootFolderAndName().ToWindowsString());
            Assert.Equal(@"d\", p.WithoutRootFolderAndNameAsWindowsString());

            Assert.Equal(@"c:\d\f", p.WithoutExtension().ToWindowsString());
            Assert.Equal(@"c:\d\f", p.WithoutExtensionAsWindowsString());

            Assert.Equal(@"c:\d\", p.WithoutName().ToWindowsString());
            Assert.Equal(@"c:\d\", p.WithoutNameAsWindowsString());

            Assert.Equal(@"c:\d\", p.WithoutFileName().ToWindowsString());
            Assert.Equal(@"c:\d\", p.WithoutFileNameAsWindowsString());

            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndFileName().ToWindowsString());
            Assert.Equal(@"\d\", p.WithoutDriveOrHostAndFileNameAsWindowsString());

            Assert.Equal(@"d\", p.WithoutRootFolderAndFileName().ToWindowsString());
            Assert.Equal(@"d\", p.WithoutRootFolderAndFileNameAsWindowsString());
        }

        [Fact]
        public void WithoutAnscestors()
        {
            var p = FilePath.Parse("c:/foo/bar/baz");
            Assert.Equal(@"bar\baz", p.WithoutAnscestors().ToWindowsString());
            Assert.Equal(@"baz", p.WithoutAnscestors(2));
            Assert.Throws<ArgumentOutOfRangeException>(() => p.WithoutAnscestors(3));
        }

        [Fact]
        public void WithoutDriveOrHostAndExtension()
        {
            var p1 = FilePath.Parse("c:/d/a.b.c");
            Assert.Equal(@"\d\a.b", p1.WithoutDriveOrHostAndExtensionAsWindowsString());

            var p2 = p1.WithoutDriveOrHostAndExtension();
            Assert.Equal(@"\d\a.b", p2.ToWindowsString());
            Assert.Equal(@"\d\a", p2.WithoutDriveOrHostAndExtensionAsWindowsString());
            Assert.Equal(@".b", p2.Extension);

            var p3 = p2.WithoutDriveOrHostAndExtension();
            Assert.Equal(@"\d\a", p3.ToWindowsString());
            Assert.Equal(@"\d\a", p3.WithoutDriveOrHostAndExtensionAsWindowsString());
            Assert.Equal(@"", p3.Extension);
            Assert.Equal(@"\d\a", p3.WithoutDriveOrHostAndExtension().ToWindowsString());

            var p4 = FilePath.Parse("c:/d/a.b.c/");
            Assert.Equal(@"\d\a.b", p4.WithoutDriveOrHostAndExtensionAsWindowsString());
            Assert.True(p4.IsDirectory);

            var p5 = p4.WithoutDriveOrHostAndExtension();
            Assert.Equal(@"\d\a.b", p5.ToWindowsString());
            Assert.False(p5.IsDirectory);
            Assert.Equal(@".b", p5.Extension);
        }

        [Fact]
        public void WithoutExtension()
        {
            var p1 = FilePath.Parse("c:/d/a.b.c");
            Assert.Equal(@"c:\d\a.b", p1.WithoutExtensionAsWindowsString());

            var p2 = p1.WithoutExtension();
            Assert.Equal(@"c:\d\a.b", p2.ToWindowsString());
            Assert.Equal(@"c:\d\a", p2.WithoutExtensionAsWindowsString());
            Assert.Equal(@".b", p2.Extension);

            var p3 = p2.WithoutExtension();
            Assert.Equal(@"c:\d\a", p3.ToWindowsString());
            Assert.Equal(@"c:\d\a", p3.WithoutExtensionAsWindowsString());
            Assert.Equal(@"", p3.Extension);
            Assert.Equal(@"c:\d\a", p3.WithoutExtension().ToWindowsString());

            var p4 = FilePath.Parse("c:/d/a.b.c/");
            Assert.Equal(@"c:\d\a.b", p4.WithoutExtensionAsWindowsString());
            Assert.True(p4.IsDirectory);

            var p5 = p4.WithoutExtension();
            Assert.Equal(@"c:\d\a.b", p5.ToWindowsString());
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

        [Fact]
        public void WithoutParents()
        {
            var p = FilePath.Parse("c:/foo/bar/baz");
            Assert.Equal(@"c:\foo\baz", p.WithoutParents().ToWindowsString());
            Assert.Equal(@"c:\baz", p.WithoutParents(2).ToWindowsString());
            Assert.Throws<ArgumentOutOfRangeException>(() => p.WithoutParents(3));
        }

        [Fact]
        public void WithRoot_InvalidDriveLetter()
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
        public void WithRoot_InvalidNetworkShare()
        {
            var ex = Assert.Throws<ArgumentException>(() => FilePath.Parse("d").WithRoot("//s"));
            Assert.Contains("//s", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => FilePath.Parse("d").WithRoot("//s/s/s"));
            Assert.Contains("//s/s/s", ex.Message);
        }

        [Fact]
        public void WithRoot_Rootless()
        {
            var fp = FilePath.Parse("d");

            var r1 = fp.WithRoot("a:");
            Assert.Equal("a:d", r1);
            Assert.Equal("a:", r1.Drive);
            Assert.Equal('a', r1.DriveLetter);
            Assert.Empty(r1.Host);
            Assert.Empty(r1.RootFolder);

            var r2 = fp.WithRoot(@"\\s\s\");
            Assert.Equal(@"\\s\s\d", r2.ToWindowsString());
            Assert.Empty(r2.Drive);
            Assert.Null(r2.DriveLetter);
            Assert.Equal(@"\\s", r2.HostWindows);
            Assert.Equal(@"\s\", r2.RootFolderWindows);
        }
    }
}