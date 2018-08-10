using System;
using System.IO;
using Xunit;

namespace Exader.IO
{
    public class StreamExtensionsFacts : IClassFixture<StreamExtensionsFacts.Fixture>
    {
        private readonly Fixture _fixture;

        public StreamExtensionsFacts(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("cyr.txt")]
        [InlineData("UTF-8-test-illegal-312.txt")]
        [InlineData("UTF-8-test-illegal-311.txt")]
        public void ShouldConvertToUtf8(string fileName)
        {
            var filePath = TestData.Get(fileName);
            var copy = _fixture.Temp / fileName;

            filePath.CopyTo(copy, CopyOptions.ForceOverwrite);

            Assert.True(copy.TryRecodeToUtf8());

            using (Stream stream = File.Open(copy, FileMode.Open, FileAccess.Read))
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
            using (Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                return stream.IsUtf8();
            }
        }
        
        public class Fixture : IDisposable
        {
            public Fixture()
            {
                Temp = TestData.Path.Directory(Guid.NewGuid().ToString()).Ensure();
            }

            public FilePath Temp { get; }
            
            public void Dispose()
            {
                Temp.Delete(true);
            }
        }
    }
}
