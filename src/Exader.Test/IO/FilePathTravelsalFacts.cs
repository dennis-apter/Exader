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
            var list = FilePath.Empty.DescendantDirectories().ToList();
            foreach (var descendant in list)
            {
                Trace.WriteLine(descendant);
            }

            Assert.True(FilePath.Empty.DescendantDirectories().Any());
        }

        [Fact]
        public void Descendants()
        {
            var list = FilePath.Empty.Descendants().ToList();
            foreach (var descendant in list)
            {
                Trace.WriteLine(descendant);
            }

            Assert.True(FilePath.Empty.Descendants().Any());
        }

        [Fact]
        public void DescendantDirectories_ByPattern()
        {
            var list = FilePath.Empty.DescendantDirectories("*ar*").ToList();
            foreach (var descendant in list)
            {
                Trace.WriteLine(descendant);
            }

            Assert.True(list.Any());
        }

        [Fact]
        public void DescendantFiles_ByPattern()
        {
            var list = FilePath.Empty.DescendantFiles("*.dll").ToList();
            foreach (var descendant in list)
            {
                Trace.WriteLine(descendant);
            }

            Assert.True(list.Any());
        }
    }
}
