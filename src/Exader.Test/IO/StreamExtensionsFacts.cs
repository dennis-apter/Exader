using System.IO;
using Xunit;

namespace Exader.IO
{
    public class StreamExtensionsFacts
    {
        [Theory]
        [InlineData("cyr.txt")]
        [InlineData("UTF-8-test-illegal-312.txt")]
        [InlineData("UTF-8-test-illegal-311.txt")]
        public static void ShouldConvertToUtf8(string fileName)
        {
            var filePath = TestData.Get(fileName);
            var copy = TestData.Temp() / fileName;

            filePath.CopyTo(copy, CopyOptions.ForceOverwrite);

            Assert.True(copy.TryRecodeToUtf8());

            using (Stream stream = copy.Open())
            {
                Assert.True(stream.IsUtf8());
            }
        }

        [Theory]
        [InlineData("cyr.txt")]
        [InlineData("UTF-8-test-illegal-312.txt")]
        [InlineData("UTF-8-test-illegal-311.txt")]
        public static void ShouldDetectInvalidUtf8Encoding(string fileName)
        {
            Assert.False(Test(fileName));
        }

        [Theory]
        [InlineData("utf8.html")]
        [InlineData("ru.sql")]
        [InlineData("UTF-8-demo.txt")]
        [InlineData("utf8BOM.txt")]
        [InlineData("UTF-8-test.txt")]
        public static void ShouldDetectValidUtf8Encoding(string fileName)
        {
            Assert.True(Test(fileName));
        }

        private static bool Test(string fileName)
        {
            var filePath = TestData.Get(fileName);
            using (Stream stream = filePath.Open())
            {
                return stream.IsUtf8();
            }
        }
    }
}
