using System.IO;
using Xunit;

namespace Exader.IO
{
	public class StreamExtensions_IsUtf8
	{
		private static readonly FilePath TestData = FilePaths.CurrentDirectory.SubpathBefore("src", true) / "TestData";

		[Theory]
		[InlineData("cyr.txt")]
		[InlineData("UTF-8-test-illegal-312.txt")]
		[InlineData("UTF-8-test-illegal-311.txt")]
		public static void ShouldConvertToUtf8(string fileName)
		{
			var filePath = TestData / fileName;
			var copy = TestData / "PleaseIgnore" / fileName;
			copy.EnsureParentExists();

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
			var filePath = TestData / fileName;
			using (Stream stream = filePath.Open())
			{
				return stream.IsUtf8();
			}
		}
	}
}
