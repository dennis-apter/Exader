#if !SILVERLIGHT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace Exader.IO
{
    public partial class FilePath
    {
        public bool IsFileExists => File.Exists(ToAbsoluteString());

        public bool IsDirectoryExists => Directory.Exists(ToAbsoluteString());

        public bool IsParentExists => IsRoot && Directory.Exists(WithoutNameAsString());

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
                    return 0 == Directory.GetFileSystemEntries(ToString()).Length;
                }

                return true;
            }
        }

        #region Travelsal

        private static IEnumerable<FilePath> GetSiblingDirectories(
            string basePath, string selfPath, string searchPattern, SearchOption searchOption)
        {
            if (basePath == "")
                basePath = ".";
#if NET35
            foreach (string directory in Directory.GetDirectories(basePath, searchPattern, searchOption))
#else
            foreach (string directory in Directory.EnumerateDirectories(basePath, searchPattern, searchOption))
#endif
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
                basePath = ".";
#if NET35
            foreach (string file in Directory.GetFiles(basePath, searchPattern, searchOption))
#else
            foreach (string file in Directory.EnumerateFiles(basePath, searchPattern, searchOption))
#endif
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
            }
            while (0 < --offset);

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
            foreach (FilePath directory in Directories("*", SearchOption.AllDirectories))
            {
                yield return directory;
            }
        }

        public IEnumerable<FilePath> DescendantDirectories(string searchPattern)
        {
            foreach (FilePath directory in Directories(searchPattern, SearchOption.AllDirectories))
            {
                yield return directory;
            }
        }

        public IEnumerable<FilePath> DescendantFiles()
        {
            foreach (FilePath directory in Directories())
            {
                foreach (FilePath descendant in directory.DescendantFiles())
                {
                    yield return descendant;
                }
            }

            foreach (FilePath file in Files())
            {
                yield return file;
            }
        }

        public IEnumerable<FilePath> DescendantFiles(string searchPattern)
        {
            foreach (FilePath directory in Directories())
            {
                foreach (FilePath descendant in directory.DescendantFiles(searchPattern))
                {
                    yield return descendant;
                }
            }

            foreach (FilePath file in Files(searchPattern))
            {
                yield return file;
            }
        }

        public IEnumerable<FilePath> Descendants(string searchPattern)
        {
            foreach (FilePath directory in Directories(searchPattern))
            {
                foreach (FilePath descendant in directory.Descendants(searchPattern))
                {
                    yield return descendant;
                }
            }

            foreach (FilePath file in Files(searchPattern))
            {
                yield return file;
            }
        }

        public IEnumerable<FilePath> Descendants()
        {
            foreach (FilePath directory in Directories())
            {
                yield return directory;

                foreach (FilePath descendant in directory.Descendants())
                {
                    yield return descendant;
                }
            }

            foreach (FilePath file in Files())
            {
                yield return file;
            }
        }

        /// <summary>
        ///  Enumerate all directories in the directory.
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
        ///  Enumerate files in the directories matching the 
        ///  given search pattern (ie, "*.txt") and search option.
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
        ///  Enumerate all files in the directory.
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
        ///  Enumerate files in the directory matching the 
        ///  given search pattern (ie, "*.txt") and search option.
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

        private static void ClearRecursive(string dir, bool includeReadOnly)
        {
            if (includeReadOnly)
            {
                var dirInfo = new DirectoryInfo(dir);
#if NET35
                foreach (var file in dirInfo.GetFiles())
#else
                foreach (var file in dirInfo.EnumerateFiles())
#endif
                {
                    file.IsReadOnly = false;
                    file.Delete();
                }
            }
            else
            {
#if NET35
                var files = Directory.GetFiles(dir);
#else
                var files = Directory.EnumerateFiles(dir);
#endif
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

#if NET35
            var subdirs = Directory.GetDirectories(dir);
#else
            var subdirs = Directory.EnumerateDirectories(dir);
#endif
            foreach (string subdir in subdirs)
            {
                ClearRecursive(subdir, includeReadOnly);
            }
        }

        /// <summary>
        /// Удаляет содержимое файла или директории,
        /// сохраняя атрибуты и настройки безопастности.
        /// </summary>
        public FilePath Clear(bool includeReadOnly = false)
        {
            if (IsFileExists)
            {
                if (includeReadOnly)
                {
                    ToFileInfo().IsReadOnly = false;
                }

                using (var fs = Open())
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

        public static void Copy(string sourcePath, string destPath)
        {
            Copy(sourcePath, destPath, CopyOptions.Default);
        }

        public static void Copy(string sourcePath, string destPath, CopyOptions options)
        {
            options = options ?? CopyOptions.Default;

            if (Directory.Exists(sourcePath))
            {
                CopyDirectory(sourcePath, destPath, options);
            }
            else if (File.Exists(sourcePath))
            {
                if (Directory.Exists(destPath))
                {
                    string name = Path.GetFileName(sourcePath);

                    destPath = Path.Combine(destPath, name);
                }

                CopyFile(sourcePath, destPath, options);
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir, CopyOptions options)
        {
            // If the source directory does not exist, throw an exception.
            if (!Directory.Exists(sourceDir))
            {
                if (options.ContinueOnError)
                {
                    return;
                }

                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDir);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            else if (ClearCondition.None != options.Clear)
            {
#if NET35
                var existsFiles = Directory.GetFiles(destDir);
#else
                var existsFiles = Directory.EnumerateFiles(destDir);
#endif
                foreach (string existsFile in existsFiles)
                {
                    if (options.IsAccept(existsFile, true))
                    {
                        try
                        {
                            if (ClearCondition.IfOlderThenBaseTime == options.Clear)
                            {
                                DateTime lastWriteTime = File.GetLastWriteTime(existsFile);
                                if (lastWriteTime < options.BaseTime)
                                {
                                    continue;
                                }
                            }

                            if (options.IsTraceEnabled)
                            {
                                Trace.WriteLine("Delete " + existsFile);
                            }

                            File.Delete(existsFile);
                        }
                        catch (Exception)
                        {
                            if (!options.ContinueOnError)
                            {
                                throw;
                            }
                        }
                    }
                    else if (options.IsTraceEnabled)
                    {
                        Trace.WriteLine("Ignore for clear " + existsFile);
                    }
                }
            }

            // Get the file contents of the directory to copy.
#if NET35
            var sourceFiles = Directory.GetFiles(sourceDir);
#else
            var sourceFiles = Directory.EnumerateFiles(sourceDir);
#endif
            foreach (string sourceFile in sourceFiles)
            {
                if (options.IsAccept(sourceFile, true))
                {
                    // Create the path to the new copy of the file.
                    string destFile = Path.Combine(destDir, Path.GetFileName(sourceFile));

                    // Copy the file.
                    CopyFile(sourceFile, destFile, options);
                }
                else if (options.IsTraceEnabled)
                {
                    Trace.WriteLine("Ignore for copy " + sourceFile);
                }
            }

            // If copySubDirs is true, copy the subdirectories.
            if (!options.Recursive)
                return;

#if NET35
            var sourceSubdirs = Directory.GetDirectories(sourceDir);
#else
            var sourceSubdirs = Directory.EnumerateDirectories(sourceDir);
#endif
            foreach (string sourceSubdir in sourceSubdirs)
            {
                if (options.IsAccept(sourceSubdir, false))
                {
                    // Create the subdirectory.
                    string destSubdir = Path.Combine(destDir, Path.GetFileName(sourceSubdir));

                    // Copy the subdirectories.
                    CopyDirectory(sourceSubdir, destSubdir, options);

                    if (options.ExcludeEmptyDirectories)
                    {
                        try
                        {
#if NET35
                            if (!Directory.GetFileSystemEntries(destSubdir).Any())
#else
                            if (!Directory.EnumerateFileSystemEntries(destSubdir).Any())
#endif
                            {
                                if (options.IsTraceEnabled)
                                {
                                    Trace.WriteLine("Clear empty directory " + destSubdir);
                                }

                                Directory.Delete(destSubdir);
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO Добавить опции по обработке ошибок
                            if (options.IsTraceEnabled)
                            {
                                Trace.WriteLine("Error: " + ex);
                            }
                        }
                    }
                }
                else if (options.IsTraceEnabled)
                {
                    Trace.WriteLine("Ignore for recursive " + sourceSubdir);
                }
            }
        }

        private static void CopyFile(string sourceFile, string destFile, CopyOptions options)
        {
            try
            {
                if (OverwriteCondition.Always != options.Overwrite && File.Exists(destFile))
                {
                    switch (options.Overwrite)
                    {
                        case OverwriteCondition.None:
                            if (options.IsTraceEnabled)
                            {
                                Trace.WriteLine("Skip when allready exists " + destFile);
                            }
                            return;
                        case OverwriteCondition.IfNewer:
                            DateTime destLastWriteTime = File.GetLastWriteTime(destFile);
                            DateTime sourceLastWriteTime = File.GetLastWriteTime(sourceFile);
                            if (sourceLastWriteTime < destLastWriteTime)
                            {
                                if (options.IsTraceEnabled)
                                {
                                    Trace.WriteLine("Skip when is up-to-date " + destFile);
                                }

                                return;
                            }
                            break;
                    }
                }

                if (options.IsTraceEnabled)
                {
                    Trace.WriteLine(
                        string.Format(
                            "Copy {0} | {1} --> {2}",
                            Path.GetFileName(sourceFile),
                            Path.GetDirectoryName(sourceFile),
                            Path.GetDirectoryName(destFile)
                            )
                        );
                }

                File.Copy(sourceFile, destFile, true);
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
            File.AppendAllText(ToString(), contents);
        }

        public StreamWriter AppendText()
        {
            return new StreamWriter(this, true);
        }

        public StreamWriter AppendText(Encoding encoding)
        {
            return new StreamWriter(this, true, encoding);
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
            Directory.CreateDirectory(ToString());
            return this;
        }

        public FileStream CreateAsFileStream()
        {
            return File.Open(ToString(), FileMode.Create);
        }

#if !SILVERLIGHT
        public XmlWriter CreateAsXml(bool omitXmlDeclaration = false, Encoding encoding = null, bool indent = false)
        {
            return XmlWriter.Create(
                ToString(),
                new XmlWriterSettings
                {
                    Indent = indent,
                    Encoding = encoding ?? Encoding.UTF8,
                    OmitXmlDeclaration = omitXmlDeclaration
                });
        }
#endif

        /// <summary>
        /// Удаляет файл или директорию.
        /// </summary>
        public bool Delete(bool recursive = false)
        {
            if (IsFileExists)
            {
                File.Delete(this);
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
        /// If the directory does not exist, create it.
        /// </summary>
        /// <returns></returns>
        public FilePath EnsureDirectoryExists()
        {
            if (!IsDirectoryExists)
                Directory.CreateDirectory(ToString());

            return this;
        }

        /// <summary>
        /// If the parent directory does not exist, create it.
        /// </summary>
        /// <returns></returns>
        public FilePath EnsureParentExists()
        {
            if (!IsParentExists)
                Directory.CreateDirectory(WithoutNameAsString());

            return this;
        }

        public FileStream Open()
        {
            return File.Open(ToString(), FileMode.Open);
        }

        public FileStream OpenOrCreate()
        {
            return File.Open(ToString(), FileMode.OpenOrCreate);
        }

        public StreamReader OpenText(Encoding encoding = null)
        {
            return new StreamReader(ToString(), encoding ?? Encoding.UTF8);
        }

        public XmlReader OpenXml()
        {
            return XmlReader.Create(ToString());
        }

        public FileStream Overwrite()
        {
            FileStream stream = File.OpenWrite(ToString());
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            return stream;
        }

        public StreamWriter OverwriteText(Encoding encoding = null)
        {
            return new StreamWriter(ToString(), false, encoding ?? Encoding.UTF8);
        }

#if !SILVERLIGHT
        public string[] ReadAllLines(Encoding encoding = null)
        {
            return File.ReadAllLines(this, encoding ?? Encoding.UTF8);
        }

        public XmlReader ReadXml()
        {
            return new XmlTextReader(ToString());
        }
#endif

        public string ReadAllText(Encoding encoding = null)
        {
            return File.ReadAllText(ToString(), encoding ?? Encoding.UTF8);
        }

        public string ReadAllTextWithDecoding(int maxFileSize = 64 * 1024, Encoding defaultEncoding = null)
        {
            if (defaultEncoding == null)
            {
                defaultEncoding = Encoding.Default;
            }

            using (var stream = File.Open(ToString(), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (maxFileSize < stream.Length)
                {
                    throw new ArgumentException(string.Format("Размер файла “{0}” превышает {1} КБ.", ToString(), maxFileSize / 1024));
                }

                var encoding = Encoding.UTF8;
                while (stream.CanRead)
                {
                    if (!stream.IsUtf8())
                    {
                        encoding = defaultEncoding;
                        break;
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);
#if NET35
                using (var reader = new StreamReader(stream, encoding, false, 4096))
#else
                using (var reader = new StreamReader(stream, encoding, false, 4096, true))
#endif
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public bool TryRecodeToUtf8(int maxFileSize = 64 * 1024, Encoding fromEncoding = null)
        {
            if (fromEncoding == null)
            {
                fromEncoding = Encoding.Default;
            }
            else if (Encoding.UTF8.Equals(fromEncoding))
            {
                return false;
            }

            FilePath copy;
            using (var stream = File.Open(ToString(), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (maxFileSize < stream.Length)
                {
                    throw new ArgumentException(string.Format("Размер файла “{0}” превышает {1} КБ.", ToString(), maxFileSize / 1024));
                }

                if (stream.IsUtf8())
                {
                    return false;
                }

                copy = WithExtension(Extension + "__fixed");
                using (var fixStream = File.Create(copy))
                {
                    stream.ConvertTo(fixStream, fromEncoding, Encoding.UTF8);
                    fixStream.Flush();
                }
            }

            if (copy != null)
            {
                File.Delete(this);
                File.Move(copy, this);
            }

            return true;
        }

        public IEnumerable<string> ReadLines(Encoding encoding = null)
        {
            return new LineEnumerator(this, encoding).AsEnumerable();
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

#if NET35
        public FilePath WriteAllLines(string[] contents, Encoding encoding = null)
#else
        public FilePath WriteAllLines(string[] contents, Encoding encoding = null)
        {
            File.WriteAllLines(this, contents, encoding ?? Encoding.UTF8);
            return this;
        }

        public FilePath WriteAllLines(IEnumerable<string> contents, Encoding encoding = null)
#endif
        {
            File.WriteAllLines(this, contents, encoding ?? Encoding.UTF8);
            return this;
        }

        public FilePath WriteAllText(string contents, Encoding encoding = null)
        {
            File.WriteAllText(this, contents, encoding ?? Encoding.UTF8);
            return this;
        }

        internal struct LineEnumerator : IEnumerator<string>
        {
            private readonly Encoding _encoding;
            private readonly FilePath _filePath;
            private string _line;
            private StreamReader _reader;

            public LineEnumerator(FilePath filePath, Encoding encoding)
            {
                _filePath = filePath;
                _encoding = encoding ?? Encoding.UTF8;
                _line = null;
                _reader = null;
            }

            public void Dispose()
            {
                _line = null;
                if (_reader != null)
                {
                    _reader.Close();
                    _reader = null;
                }
            }

            public bool MoveNext()
            {
                if (null == _reader)
                {
                    _reader = new StreamReader(_filePath, _encoding);
                }

                _line = _reader.ReadLine();
                return null != _line;
            }

            public void Reset()
            {
                Dispose();
            }

            object IEnumerator.Current => Current;

            public string Current => _line;
        }
    }
}
#endif
