using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Exader.IO
{
    /// <summary>
    /// TODO Use fixture
    /// </summary>
    public class FilePathTravelsalFacts
    {
        [Fact]
        public void DescendantDirectories()
        {
            Assert.True(FilePath.Empty.DescendantDirectories().Any());
        }

        [Fact]
        public void Descendants()
        {
            Assert.True(FilePath.Empty.Descendants().Any());
        }

        [Fact]
        public void DescendantDirectories_ByPattern()
        {
            var list = FilePath.Empty.DescendantDirectories("*Data*").ToList();
            Assert.True(list.Any());
        }

        [Fact]
        public void DescendantFiles_ByPattern()
        {
            var list = FilePath.Empty.DescendantFiles("*.dll").ToList();
            Assert.True(list.Any());
        }
    }
}
