using System.IO;

namespace Exader.IO
{
    public static class FilePathHelper
    {
        public static FilePath AsFilePath(this DirectoryInfo directory)
        {
            return FilePath.Parse(directory.ToString()).AsDirectory();
        }
        
        public static FilePath AsFilePath(this FileInfo file)
        {
            return FilePath.Parse(file.ToString());
        }
    }
}