using System;
using Xunit;

namespace Exader.IO
{
    public class FilePathModificationFacts
    {
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
        public void SubpathAfter()
        {
            Assert.Equal(FilePath.Parse("bar/baz"), FilePath.Parse("c:/foo/bar/baz").SubpathAfter());
            Assert.Equal(FilePath.Parse("baz"), FilePath.Parse("c:/foo/bar/baz").SubpathAfter(2));

            Assert.Throws<ArgumentOutOfRangeException>(() => FilePath.Parse("c:/foo/bar/baz").SubpathAfter(3));
        }

        [Fact]
        public void WithExtension()
        {
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
        public void WithName()
        {
            Assert.Equal(@"C:\dir\b.e", FilePath.Parse("C:/dir/file.e").WithName("b"));
            Assert.Equal(@"C:\dir\.e", FilePath.Parse("C:/dir/file.e").WithName(null));
            Assert.Equal(@"C:\dir\.f", FilePath.Parse("C:/dir/file.e").WithName(".f"));
            Assert.Equal(@"C:\dir\_._", FilePath.Parse("C:/dir/file.e").WithName("_._"));
        }

        [Fact]
        public void WithNameSuffix()
        {
            Assert.Equal("foo.bar.x", FilePath.Parse("foo.bar").WithNameSuffix("x").ToString());
            Assert.Equal("foo.bar.x", FilePath.Parse("foo.bar").WithNameSuffix(".x").ToString());
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
        public void WithoutExtensions()
        {
            var path = FilePath.Parse("foo.bar.baz");

            Assert.Equal("foo.bar", path.WithoutExtensions());
            Assert.Equal("foo", path.WithoutExtensions(2));

            Assert.Throws<ArgumentOutOfRangeException>(() => path.WithoutExtensions(3));
        }
    }
}