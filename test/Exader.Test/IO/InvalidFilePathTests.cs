using System.IO;
using Xunit;

namespace Exader.IO
{
    public class InvalidFilePathTests
    {
        [Fact]
        public void File_Relative()
        {
            var name = new string(Path.GetInvalidFileNameChars()).RemoveCharacter('\\');

            var filePath = "F" + name + ".E" + name;
            var fpb = FilePath.Builder.Parse(filePath);
            var fp = fpb.ToFilePath();

            Assert.Equal("F_.E_", fp.ToString());
        }

        [Fact]
        public void File_Rooted()
        {
            var name = new string(Path.GetInvalidFileNameChars()).RemoveCharacter('\\');

            var filePath = "\\F" + name + ".E" + name;
            var fpb = FilePath.Builder.Parse(filePath);
            var fp = fpb.ToFilePath();

            Assert.Equal("\\F_.E_", fp.ToString());
        }

        [Fact]
        public void FileWithFolder()
        {
            var name = new string(Path.GetInvalidFileNameChars()).RemoveCharacter('\\');

            var filePath = "\\D" + name + "Y\\F" + name + ".E" + name;
            var fpb = FilePath.Builder.Parse(filePath);
            var fp = fpb.ToFilePath();

            Assert.Equal("\\D_Y\\F_.E_", fp.ToString());
        }

        [Fact]
        public void Folder_Relative()
        {
            var name = new string(Path.GetInvalidFileNameChars()).RemoveCharacter('\\');

            var filePath = "D" + name + "Y\\";
            var fpb = FilePath.Builder.Parse(filePath);
            var fp = fpb.ToFilePath();

            Assert.Equal("D_Y", fp.ToString());
        }

        [Fact]
        public void Folder_Rooted()
        {
            var name = new string(Path.GetInvalidFileNameChars()).RemoveCharacter('\\');

            var filePath = "\\D" + name + "Y\\";
            var fpb = FilePath.Builder.Parse(filePath);
            var fp = fpb.ToFilePath();

            Assert.Equal("\\D_Y", fp.ToString());
        }
    }
}
