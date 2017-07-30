using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Exader.IO
{
    public partial class FilePath
    {
        public bool IsDirectoryExists => System.IO.Directory.Exists(ToAbsoluteString());

        public bool IsEmptyFileOrDirectory
        {
            get
            {
                if (IsFileExists)
                {
                    return 0 == new FileInfo(ToString()).Length;
                }

                if (IsDirectoryExists)
                {
                    return 0 == System.IO.Directory.GetFileSystemEntries(ToString()).Length;
                }

                return true;
            }
        }

        public bool IsFileExists => System.IO.File.Exists(ToAbsoluteString());

        public bool IsParentExists => IsRoot && System.IO.Directory.Exists(WithoutNameAsString());

        public static void Copy(string sourcePath, string destPath)
        {
            Copy(sourcePath, destPath, CopyOptions.Default);
        }

        public static void Copy(string sourcePath, string destPath, CopyOptions options)
        {
            options = options ?? CopyOptions.Default;

            if (System.IO.Directory.Exists(sourcePath))
            {
                CopyDirectory(sourcePath, destPath, options);
            }
            else if (System.IO.File.Exists(sourcePath))
            {
                if (System.IO.Directory.Exists(destPath))
                {
                    var name = Path.GetFileName(sourcePath);

                    destPath = Path.Combine(destPath, name);
                }

                CopyFile(sourcePath, destPath, options);
            }
        }

        public static string Temporary(string fileName = null, bool sanitize = false)
        {
            if (null == fileName)
            {
                fileName = Path.GetTempFileName();
            }
            else if (sanitize)
            {
                fileName = SanitizeFileName(fileName);
            }

            return Path.Combine(Path.GetTempPath(), fileName);
        }

        private static void ClearRecursive(string dir, bool includeReadOnly)
        {
            if (includeReadOnly)
            {
                var dirInfo = new DirectoryInfo(dir);
                foreach (var file in dirInfo.EnumerateFiles())
                {
                    file.IsReadOnly = false;
                    file.Delete();
                }
            }
            else
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(dir))
                    System.IO.File.Delete(file);
            }

            foreach (var subdir in System.IO.Directory.EnumerateDirectories(dir))
                ClearRecursive(subdir, includeReadOnly);
        }

        private static void CopyDirectory(string sourceDir, string destDir, CopyOptions options)
        {
            // If the source directory does not exist, throw an exception.
            if (!System.IO.Directory.Exists(sourceDir))
            {
                if (options.ContinueOnError)
                {
                    return;
                }

                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " +
                                                     sourceDir);
            }

            // If the destination directory does not exist, create it.
            if (!System.IO.Directory.Exists(destDir))
            {
                System.IO.Directory.CreateDirectory(destDir);
            }
            else if (ClearCondition.None != options.Clear)
            {
                var existsFiles = System.IO.Directory.EnumerateFiles(destDir);
                foreach (var existsFile in existsFiles)
                    if (options.IsAccept(existsFile, true))
                    {
                        try
                        {
                            if (ClearCondition.IfOlderThenBaseTime == options.Clear)
                            {
                                var lastWriteTime = System.IO.File.GetLastWriteTime(existsFile);
                                if (lastWriteTime < options.BaseTime)
                                {
                                    continue;
                                }
                            }

                            System.IO.File.Delete(existsFile);
                        }
                        catch (Exception)
                        {
                            if (!options.ContinueOnError)
                            {
                                throw;
                            }
                        }
                    }
            }

            // Get the file contents of the directory to copy.
            var sourceFiles = System.IO.Directory.EnumerateFiles(sourceDir);
            foreach (var sourceFile in sourceFiles)
                if (options.IsAccept(sourceFile, true))
                {
                    // Create the path to the new copy of the file.
                    var destFile = Path.Combine(destDir, Path.GetFileName(sourceFile));

                    // Copy the file.
                    CopyFile(sourceFile, destFile, options);
                }

            // If copySubDirs is true, copy the subdirectories.
            if (!options.Recursive)
            {
                return;
            }

            var sourceSubdirs = System.IO.Directory.EnumerateDirectories(sourceDir);
            foreach (var sourceSubdir in sourceSubdirs)
                if (options.IsAccept(sourceSubdir, false))
                {
                    // Create the subdirectory.
                    var destSubdir = Path.Combine(destDir, Path.GetFileName(sourceSubdir));

                    // Copy the subdirectories.
                    CopyDirectory(sourceSubdir, destSubdir, options);

                    if (options.ExcludeEmptyDirectories)
                    {
                        try
                        {
                            if (!System.IO.Directory.EnumerateFileSystemEntries(destSubdir).Any())
                            {
                                System.IO.Directory.Delete(destSubdir);
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO Добавить опции по обработке ошибок
                        }
                    }
                }
        }

        private static void CopyFile(string sourceFile, string destFile, CopyOptions options)
        {
            try
            {
                if (OverwriteCondition.Always != options.Overwrite && System.IO.File.Exists(destFile))
                {
                    switch (options.Overwrite)
                    {
                        case OverwriteCondition.IfNewer:
                            var destLastWriteTime = System.IO.File.GetLastWriteTime(destFile);
                            var sourceLastWriteTime = System.IO.File.GetLastWriteTime(sourceFile);
                            if (sourceLastWriteTime < destLastWriteTime)
                            {
                                return;
                            }

                            break;
                    }
                }

                System.IO.File.Copy(sourceFile, destFile, true);
            }
            catch (Exception)
            {
                if (!options.ContinueOnError)
                {
                    throw;
                }
            }
        }

        public void AppendAllText(string contents)
        {
            System.IO.File.AppendAllText(ToString(), contents);
        }

        public StreamWriter AppendText(Encoding encoding = null)
        {
#if NET45
            return new StreamWriter(ToString(), true, encoding ?? Encoding.UTF8);
#else
            var stream = System.IO.File.OpenWrite(ToString());
            return new StreamWriter(stream, encoding ?? Encoding.UTF8);
#endif
        }

        /// <summary>
        ///     Удаляет содержимое файла или директории,
        ///     сохраняя атрибуты и настройки безопастности.
        /// </summary>
        public FilePath Clear(bool includeReadOnly = false)
        {
            if (IsFileExists)
            {
                if (includeReadOnly)
                {
                    ToFileInfo().IsReadOnly = false;
                }

                using (var fs = System.IO.File.Open(ToString(), FileMode.Open, FileAccess.Write))
                {
                    fs.SetLength(0);
                }
            }
            else if (IsDirectoryExists)
            {
                ClearRecursive(this, includeReadOnly);
            }

            return this;
        }

        public FilePath CopyTo(string destination, CopyOptions options = null)
        {
            return CopyTo(Parse(destination));
        }

        public FilePath CopyTo(FilePath destination, CopyOptions options = null)
        {
            options = options ?? CopyOptions.Default;
            if (IsDirectoryExists)
            {
                CopyDirectory(this, destination, options);
            }
            else if (IsFileExists)
            {
                if (destination.IsDirectoryExists)
                {
                    destination = destination / Name;
                }
                else if (Extension != string.Empty &&
                         destination.Extension == string.Empty)
                {
                    destination.CreateAsDirectory();
                    destination = destination / Name;
                }

                CopyFile(this, destination, options);
            }

            return destination;
        }

        public FilePath CreateAsDirectory()
        {
            System.IO.Directory.CreateDirectory(ToString());
            return this;
        }

        public FileStream CreateAsFileStream()
        {
            return System.IO.File.Open(ToString(), FileMode.Create);
        }

        /// <summary>
        ///     Удаляет файл или директорию.
        /// </summary>
        public bool Delete(bool recursive = false)
        {
            if (IsFileExists)
            {
                System.IO.File.Delete(this);
                return true;
            }

            if (IsDirectoryExists)
            {
                var di = ToDirectoryInfo();
                di.Delete(recursive);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     If the directory does not exist, create it.
        /// </summary>
        /// <returns></returns>
        public FilePath EnsureDirectoryExists()
        {
            if (!IsDirectoryExists)
            {
                System.IO.Directory.CreateDirectory(ToString());
            }

            return this;
        }

        /// <summary>
        ///     If the parent directory does not exist, create it.
        /// </summary>
        /// <returns></returns>
        public FilePath EnsureParentExists()
        {
            if (!IsParentExists)
            {
                System.IO.Directory.CreateDirectory(WithoutNameAsString());
            }

            return this;
        }

        public FilePath MoveTo(string destination)
        {
            return MoveTo((FilePath) destination);
        }

        public FilePath MoveTo(FilePath destination)
        {
            if (destination.IsDirectory)
            {
                destination = destination.EnsureDirectoryExists().File(Name);
            }

            System.IO.File.Move(this, destination);
            return destination;
        }

        [Obsolete("Replaced to " + nameof(ReadText), true)]
        public StreamReader OpenText(Encoding encoding = null)
        {
            return ReadText(encoding);
        }

        [Obsolete("Replaced to " + nameof(WriteText), true)]
        public StreamWriter OverwriteText(Encoding encoding = null)
        {
            return WriteText(encoding);
        }

        public string[] ReadAllLines(Encoding encoding = null)
        {
            return System.IO.File.ReadAllLines(this, encoding ?? Encoding.UTF8);
        }

        public string ReadAllText(Encoding encoding = null)
        {
            return System.IO.File.ReadAllText(ToString(), encoding ?? Encoding.UTF8);
        }

        public string ReadAllTextWithDecoding(int maxFileSize = 64 * 1024, Encoding defaultEncoding = null)
        {
            if (defaultEncoding == null)
            {
                defaultEncoding = Encoding.UTF8;
            }

            using (var stream = System.IO.File.Open(ToString(), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (maxFileSize < stream.Length)
                {
                    throw new ArgumentException(string.Format("Размер файла “{0}” превышает {1} КБ.", ToString(),
                        maxFileSize / 1024));
                }

                var encoding = Encoding.UTF8;
                while (stream.CanRead)
                    if (!stream.IsUtf8())
                    {
                        encoding = defaultEncoding;
                        break;
                    }

                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, encoding, false, 4096, true))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public IEnumerable<string> ReadLines(Encoding encoding = null)
        {
            return System.IO.File.ReadLines(this, encoding ?? Encoding.UTF8);
        }

        public StreamReader ReadText(Encoding encoding = null)
        {
#if NET45
            return new StreamReader(ToString(), encoding ?? Encoding.UTF8);
#else
            var stream = System.IO.File.Open(ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
            return new StreamReader(stream, encoding ?? Encoding.UTF8);
#endif
        }

        public DirectoryInfo ToDirectoryInfo()
        {
            return new DirectoryInfo(this);
        }

        public FileInfo ToFileInfo()
        {
            return new FileInfo(this);
        }

        [CanBeNull]
        public FileSystemInfo ToFileSystemInfo()
        {
            if (IsDirectoryExists)
            {
                return new DirectoryInfo(this);
            }
            if (IsFileExists)
            {
                return new FileInfo(this);
            }

            return null;
        }

        public bool TryRecodeToUtf8(int maxFileSize = 64 * 1024, Encoding fromEncoding = null)
        {
            if (fromEncoding == null)
            {
                fromEncoding = Encoding.UTF8;
            }
            else if (Encoding.UTF8.Equals(fromEncoding))
            {
                return false;
            }

            FilePath copy;
            using (var stream = System.IO.File.Open(ToString(), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (maxFileSize < stream.Length)
                {
                    throw new ArgumentException(string.Format("Размер файла “{0}” превышает {1} КБ.", ToString(),
                        maxFileSize / 1024));
                }

                if (stream.IsUtf8())
                {
                    return false;
                }

                copy = WithExtension(Extension + "__fixed");
                using (var fixStream = System.IO.File.Create(copy))
                {
                    stream.ConvertTo(fixStream, fromEncoding, Encoding.UTF8);
                    fixStream.Flush();
                }
            }

            if (copy != null)
            {
                System.IO.File.Delete(this);
                System.IO.File.Move(copy, this);
            }

            return true;
        }

        public FilePath WriteAllLines(string[] contents, Encoding encoding = null)
        {
            System.IO.File.WriteAllLines(this, contents, encoding ?? Encoding.UTF8);
            return this;
        }

        public FilePath WriteAllLines(IEnumerable<string> contents, Encoding encoding = null)
        {
            System.IO.File.WriteAllLines(this, contents, encoding ?? Encoding.UTF8);
            return this;
        }

        public FilePath WriteAllText(string contents, Encoding encoding = null)
        {
            System.IO.File.WriteAllText(this, contents, encoding ?? Encoding.UTF8);
            return this;
        }

        public StreamWriter WriteText(Encoding encoding = null)
        {
#if NET45
            return new StreamWriter(ToString(), false, encoding ?? Encoding.UTF8);
#else
            var stream = System.IO.File.OpenWrite(ToString());
            stream.SetLength(0);
            return new StreamWriter(stream, encoding ?? Encoding.UTF8);
#endif
        }

        #region Travelsal

        private static IEnumerable<FilePath> GetSiblingDirectories(
            string basePath, string selfPath, string searchPattern, SearchOption searchOption)
        {
            if (basePath == "")
            {
                basePath = ".";
            }

            foreach (var directory in System.IO.Directory.EnumerateDirectories(basePath, searchPattern, searchOption))
            {
                if (!string.IsNullOrEmpty(selfPath) && selfPath.Equals(directory, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return Parse(directory);
            }
        }

        private static IEnumerable<FilePath> GetSiblingFiles(
            string basePath, string selfPath, string searchPattern, SearchOption searchOption)
        {
            if (basePath == "")
            {
                basePath = ".";
            }

            foreach (var file in System.IO.Directory.EnumerateFiles(basePath, searchPattern, searchOption))
            {
                if (!string.IsNullOrEmpty(selfPath) && selfPath.Equals(file, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return Parse(file);
            }
        }

        public FilePath Ancestor(int offset)
        {
            if (offset < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var ancestor = this;
            do
            {
                ancestor = ancestor.Parent;
                if (ancestor == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset));
                }
            } while (0 < --offset);

            return ancestor;
        }

        public IEnumerable<FilePath> Ancestors()
        {
            var ancestor = Parent;
            while (null != ancestor)
            {
                yield return ancestor;

                ancestor = ancestor.Parent;
            }
        }

        public IEnumerable<FilePath> AncestorsAndSelf()
        {
            yield return this;

            var ancestor = Parent;
            while (null != ancestor)
            {
                yield return ancestor;

                ancestor = ancestor.Parent;
            }
        }

        public IEnumerable<FilePath> DescendantDirectories()
        {
            foreach (var directory in Directories("*", SearchOption.AllDirectories))
                yield return directory;
        }

        public IEnumerable<FilePath> DescendantDirectories(string searchPattern)
        {
            foreach (var directory in Directories(searchPattern, SearchOption.AllDirectories))
                yield return directory;
        }

        public IEnumerable<FilePath> DescendantFiles()
        {
            foreach (var directory in Directories())
            foreach (var descendant in directory.DescendantFiles())
                yield return descendant;

            foreach (var file in Files())
                yield return file;
        }

        public IEnumerable<FilePath> DescendantFiles(string searchPattern)
        {
            foreach (var directory in Directories())
            foreach (var descendant in directory.DescendantFiles(searchPattern))
                yield return descendant;

            foreach (var file in Files(searchPattern))
                yield return file;
        }

        public IEnumerable<FilePath> Descendants(string searchPattern)
        {
            foreach (var directory in Directories(searchPattern))
            foreach (var descendant in directory.Descendants(searchPattern))
                yield return descendant;

            foreach (var file in Files(searchPattern))
                yield return file;
        }

        public IEnumerable<FilePath> Descendants()
        {
            foreach (var directory in Directories())
            {
                yield return directory;

                foreach (var descendant in directory.Descendants())
                    yield return descendant;
            }

            foreach (var file in Files())
                yield return file;
        }

        /// <summary>
        ///     Enumerate all directories in the directory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FilePath> Directories()
        {
            return Directories(AllItemsPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<FilePath> Directories(string searchPattern)
        {
            return Directories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        ///     Enumerate files in the directories matching the
        ///     given search pattern (ie, "*.txt") and search option.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public IEnumerable<FilePath> Directories(string searchPattern, SearchOption searchOption)
        {
            return IsDirectoryExists
                ? GetSiblingDirectories(ToString(), null, searchPattern, searchOption)
                : Enumerable.Empty<FilePath>();
        }

        /// <summary>
        ///     Enumerate all files in the directory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FilePath> Files()
        {
            return Files(AllItemsPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<FilePath> Files(string searchPattern)
        {
            return Files(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        ///     Enumerate files in the directory matching the
        ///     given search pattern (ie, "*.txt") and search option.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public IEnumerable<FilePath> Files(string searchPattern, SearchOption searchOption)
        {
            return IsDirectoryExists
                ? GetSiblingFiles(ToString(), null, searchPattern, searchOption)
                : Enumerable.Empty<FilePath>();
        }

        public IEnumerable<FilePath> Items()
        {
            return Items(AllItemsPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<FilePath> Items(string searchPattern)
        {
            return Items(searchPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<FilePath> Items(string searchPattern, SearchOption searchOption)
        {
            return IsDirectoryExists
                ? GetSiblingDirectories(ToString(), null, searchPattern, searchOption)
                    .Union(GetSiblingFiles(ToString(), null, searchPattern, searchOption))
                : Enumerable.Empty<FilePath>();
        }

        public FilePath Sibling(string relativePath)
        {
            if (null == Parent)
            {
                throw new ArgumentException(
                    $"Невозможно получить смежный путь {relativePath} на базе путя без основания {ToString()}.",
                    nameof(relativePath)
                );
            }

            return Parent.Combine(relativePath);
        }

        public IEnumerable<FilePath> SiblingDirectories()
        {
            return SiblingDirectories(AllItemsPattern);
        }

        public IEnumerable<FilePath> SiblingDirectories(string searchPattern)
        {
            return IsRoot
                ? GetSiblingDirectories(WithoutNameAsString(), ToString(), searchPattern, SearchOption.TopDirectoryOnly)
                : Enumerable.Empty<FilePath>();
        }

        public IEnumerable<FilePath> SiblingFiles()
        {
            return SiblingFiles(AllItemsPattern);
        }

        public IEnumerable<FilePath> SiblingFiles(string searchPattern)
        {
            return IsRoot
                ? GetSiblingFiles(WithoutNameAsString(), ToString(), searchPattern, SearchOption.TopDirectoryOnly)
                : Enumerable.Empty<FilePath>();
        }

        public IEnumerable<FilePath> Siblings(string searchPattern)
        {
            return IsRoot
                ? GetSiblingDirectories(ToString(), ToString(), searchPattern, SearchOption.TopDirectoryOnly)
                    .Union(GetSiblingFiles(ToString(), ToString(), searchPattern, SearchOption.TopDirectoryOnly))
                : Enumerable.Empty<FilePath>();
        }

        public IEnumerable<FilePath> Siblings()
        {
            return Siblings(AllItemsPattern);
        }

#endregion
    }
}
