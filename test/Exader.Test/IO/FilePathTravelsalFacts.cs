using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Exader.IO
{
    public class FilePathTravelsalFacts : IClassFixture<FilePathTravelsalFacts.Fixture>
    {
        private readonly Fixture _fixture;

        public FilePathTravelsalFacts(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DescendantDirectories()
        {
            Assert.True(_fixture.Root.DescendantDirectories().Any());
        }

        [Fact]
        public void Descendants()
        {
            Assert.True(_fixture.Root.Descendants().Any());
        }

        [Fact]
        public void DescendantDirectories_ByPattern()
        {
            var list = _fixture.Root.DescendantDirectories("*Data*").ToList();
            Assert.True(list.Any());
        }

        [Fact]
        public void DescendantFiles_ByPattern()
        {
            var list = _fixture.Root.DescendantFiles("*.dll").ToList();
            Assert.True(list.Any());
        }
        
        public class Fixture : IDisposable
        {
            public Fixture()
            {
                Root = TestData.Path.Directory(Guid.NewGuid().ToString()).Ensure();
                var baz = (Root / "foo/bar/baz/").Ensure();
                baz.File("dummy.dll").WriteAllText("test");
                baz.Directory("Data").Ensure();
            }
            
            public FilePath Root { get; }

            public void Dispose()
            {
                Root.Delete(true);
            }
        }
    }
}
