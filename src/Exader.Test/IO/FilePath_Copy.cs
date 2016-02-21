using System.Linq;
using Xunit;

namespace Exader.IO
{
    public class FilePath_Copy
    {
        private readonly FilePath _root;

        public FilePath_Copy()
        {
            _root = (FilePath) "_TheCopyMethod";
            _root.Clear(true)
                .Combine("Foo").CreateAsDirectory()
                .Combine("baz.txt").WriteAllText("Test").Parent
                .Combine("Bar").CreateAsDirectory()
                .Combine("fubar.txt").WriteAllText("Test");
        }

        [Fact]
        public void DirectoryToExistsDirectory()
        {
            var source = _root / "Foo";
            var target = _root / "Foo3";

            target.Clear(true);

            var newTarget = source.CopyTo(target, new CopyOptions { Recursive = true });
            Assert.Equal(target, newTarget);

            var result = string.Join(";", target.Descendants().Select(d => d % target));
            Assert.Equal(@"Bar;Bar\fubar.txt;baz.txt", result);
        }

        [Fact]
        public void DirectoryToNewDirectory()
        {
            var source = _root / "Foo";
            var target = _root / "Foo2";

            target.Delete(true);

            var newTarget = source.CopyTo(target, new CopyOptions { Recursive = true });
            Assert.Equal(target, newTarget);

            var result = string.Join(";", target.Descendants().Select(d => d % target));
            Assert.Equal(@"Bar;Bar\fubar.txt;baz.txt", result);
        }

        [Fact]
        public void FileToExistsDirectory()
        {
            var source = _root / "Foo/baz.txt";
            var target = _root / "Foo5";

            target.Clear(true);

            var newTarget = source.CopyTo(target);
            Assert.Equal(target, newTarget.Parent);
            Assert.Equal(source.Name, newTarget.Name);

            var result = string.Join(";", target.Descendants().Select(d => d % target));
            Assert.Equal("baz.txt", result);
        }

        [Fact]
        public void FileToNewDirectory()
        {
            var source = _root / "Foo/baz.txt";
            var target = _root / "Foo4";

            target.Delete(true);

            var newTarget = source.CopyTo(target);
            Assert.Equal(target, newTarget.Parent);
            Assert.Equal(source.Name, newTarget.Name);

            var result = string.Join(";", target.Descendants().Select(d => d % target));
            Assert.Equal("baz.txt", result);
        }
    }
}