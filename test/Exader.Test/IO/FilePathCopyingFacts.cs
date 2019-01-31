using System;
using System.Linq;
using Xunit;

namespace Exader.IO
{
    public class FilePathCopyingFacts : IClassFixture<FilePathCopyingFacts.Fixture>
    {
        private readonly Fixture _fixture;

        public FilePathCopyingFacts(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DirectoryToExistsDirectory()
        {
            var source = _fixture.Root / "A";
            var target = _fixture.Root / "DirectoryToExistsDirectory";

            var newTarget = source.CopyTo(target, new CopyOptions { Recursive = true });
            Assert.Equal(target, newTarget);

            var result = string.Join(";", target.Descendants().Select(d => (d % target).ToWindowsString()));
            Assert.Equal(@"B;B\b.txt;a.txt", result);
        }

        [Fact]
        public void DirectoryToNewDirectory()
        {
            var source = _fixture.Root / "A";
            var target = _fixture.Root / "DirectoryToNewDirectory";

            var newTarget = source.CopyTo(target, new CopyOptions { Recursive = true });
            Assert.Equal(target, newTarget);

            var result = string.Join(";", target.Descendants().Select(d => (d % target).ToWindowsString()));
            Assert.Equal(@"B;B\b.txt;a.txt", result);
        }

        [Fact]
        public void FileToExistsDirectory()
        {
            var source = _fixture.Root / "A/a.txt";
            var target = _fixture.Root / "FileToExistsDirectory";

            var newTarget = source.CopyTo(target);
            Assert.Equal(target, newTarget.Parent);
            Assert.Equal(source.Name, newTarget.Name);

            var result = string.Join(";", target.Descendants().Select(d => d % target));
            Assert.Equal("a.txt", result);
        }

        [Fact]
        public void FileToNewDirectory()
        {
            var source = _fixture.Root / "A/a.txt";
            var target = _fixture.Root / "FileToNewDirectory";

            var newTarget = source.CopyTo(target);
            Assert.Equal(target, newTarget.Parent);
            Assert.Equal(source.Name, newTarget.Name);

            var result = string.Join(";", target.Descendants().Select(d => d % target));
            Assert.Equal("a.txt", result);
        }

        public class Fixture : IDisposable
        {
            public Fixture()
            {
                Root = TestData.Path.Directory(Guid.NewGuid().ToString());
                Root.Combine("A").CreateAsDirectory()
                    .Combine("a.txt").WriteAllText("Test").Parent
                    .Combine("B").CreateAsDirectory()
                    .Combine("b.txt").WriteAllText("Test");
            }

            public FilePath Root { get; }

            public void Dispose()
            {
                Root.Delete(true);
            }
        }
    }
}
