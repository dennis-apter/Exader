using Exader.IO;
using Xunit;

namespace Exader
{
    public static class TestData
    {
        public static readonly FilePath Path = FilePaths.CurrentDirectory.SubpathBefore("test", true) / "TestData";

        public static FilePath Temp()
        {
            return (Path / "PleaseIgnore/").Ensure();
        }

        public static FilePath Get(string fileName)
        {
            var path = Path / fileName;
            Assert.True(path.IsFileExists);
            return path;
        }
    }
}
