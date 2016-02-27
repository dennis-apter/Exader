#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Exader.IO
{
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed partial class FilePath : IEquatable<FilePath>
    {
        private const string AllItemsPattern = "*";
        private static readonly string DirectoryName = ".";

        public static readonly FilePath Empty = new FilePath();
        public static readonly FilePath RelativeRoot = new FilePath("", "\\", "", "", "", false);

        /// <summary>
        /// Storing drive letter or server computer name of shared folder.
        /// </summary>
        [NotNull]
        private readonly string _driveOrHost;

        [NotNull]
        private readonly string _extension;

        [NotNull]
        private readonly string _name;

        [NotNull]
        private readonly string _prefix;

        [NotNull]
        private readonly string _path;

        /// <summary>
        /// Storing name of shared folder or root directory.
        /// </summary>
        [NotNull]
        private readonly string _rootFolder;

        private bool _isDirectory;

        private FilePath _parent;


        /// <summary>
        /// Создает экземпляр <see cref="FilePath"/>
        /// для описания фалового пути к текущей директории.
        /// </summary>
        [DebuggerStepThrough]
        public FilePath()
        {
            _driveOrHost = "";
            _rootFolder = "";
            _prefix = "";
            _name = "";
            _extension = "";
            _path = "";
        }

        private FilePath(string driveOrHost, string rootFolder, string prefix, string name, string extension, bool isDirectory)
        {
            _driveOrHost = driveOrHost;
            _rootFolder = rootFolder;
            _prefix = prefix;
            _name = name;
            _extension = extension;
            _isDirectory = isDirectory;

            _path = CombinePath(driveOrHost, rootFolder, prefix, name, extension, isDirectory);
        }

        private FilePath(FilePath parent, string name, string extension, bool isDirectory)
        {
            if (parent != null)
            {
                _parent = parent;
                _driveOrHost = parent._driveOrHost;
                _rootFolder = parent._rootFolder;
                _prefix = parent.DirectoryPath;
            }
            else
            {
                _driveOrHost = "";
                _rootFolder = "";
                _prefix = "";
            }

            _name = name;
            _extension = extension;
            _isDirectory = isDirectory;

            _path = CombinePath(_driveOrHost, _rootFolder, _prefix, name, extension, isDirectory);
        }

        private static string CombinePath(string driveOrHost, string rootFolder, string prefix, string name, string extension, bool isDirectory)
        {
            var trailingSlash = isDirectory && (name != "" || extension != "") ? "\\" : "";
            var path = driveOrHost + rootFolder + prefix + name + extension + trailingSlash;
            return path;// == "" ? "." : path;
        }

        public string DirectoryPath
        {
            get
            {
                var rootLength = _driveOrHost.Length + _rootFolder.Length;
                return _isDirectory && _path.Length > rootLength
                    ? _prefix + _name + _extension + "\\"
                    : _prefix;
            }
        }

        public string Drive => IsLocal ? _driveOrHost : "";
        public char? DriveLetter => IsLocal ? (char?)_driveOrHost[0] : null;

        public string DriveOrHost => _driveOrHost;
        public string Extension => _extension;
        public bool HasDriveOrHost => _driveOrHost != "";
        public bool HasRootFolder => _rootFolder != "";
        public string Host => IsNetwork ? _driveOrHost : "";

        /// <summary>
        ///  Указывает на то, что путь является абсолютным и обладает именем диска.
        /// </summary>
        /// <example>\</example>
        public bool IsAbsolute => _driveOrHost != "" && _rootFolder != "";

        public bool IsCurrent => _rootFolder == "" && _prefix == "" && _name == "" && _extension == "";

        public bool IsDirectory => _isDirectory || (_name == "" && _extension == "");

        public bool IsLocal => _driveOrHost.Length == 2 && _driveOrHost[1] == ':';
        public bool IsNetwork => 2 < _driveOrHost.Length && _driveOrHost[0] == '\\';

        public bool IsExternal => _name == ".." || _prefix.StartsWith("..");

        /// <summary>
        /// Relative or Absolute
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return _rootFolder != ""
                    && _prefix == ""
                    && _name == ""
                    && _extension == "";
            }
        }

        public string Name => _name + _extension;
        public string NameWithoutExtension => _name;

        /// <summary>
        ///  Возвращает путь верхнего уровня.
        /// </summary>
        [CanBeNull]
        public FilePath Parent
        {
            get
            {
                if (_parent != null || (_name == "" && _extension == ""))
                    return _parent;

                if (_prefix != "")
                {
                    if (_prefix.Length < 2)
                        throw new InvalidOperationException($"Invalid file path.");

                    string prefix;
                    string fullName;
                    string name;
                    string ext;
                    var i = _prefix.LastIndexOf('\\', _prefix.Length - 2);
                    if (i == -1)
                    {
                        fullName = _prefix.Substring(0, _prefix.Length - 1);
                        prefix = "";
                    }
                    else
                    {
                        fullName = _prefix.Substring(i + 1, _prefix.Length - i - 2);
                        prefix = _prefix.Substring(0, i + 1);
                    }

                    if (fullName == "..")
                    {
                        name = fullName;
                        ext = "";
                    }
                    else
                    {
                        name = fullName.SubstringBeforeOrSelf('.');
                        ext = fullName.SubstringAfter(name.Length);
                    }

                    _parent = new FilePath(_driveOrHost, _rootFolder, prefix, name, ext, true);
                }
                else if (_rootFolder != "")
                {
                    _parent = new FilePath(_driveOrHost, _rootFolder, "", "", "", true);
                }

                return _parent;
            }
        }

        public string Prefix => _prefix;
        public string RootFolder => _rootFolder;

        public string NameWithoutExtensions(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                var result = NameWithoutExtensionsCore(ext);
                if (result != null)
                {
                    return result;
                }
            }

            return Name;
        }

        public string NameWithoutExtensions(params string[] extensions)
        {
            return NameWithoutExtensions((IEnumerable<string>)extensions);
        }

        public string NameWithoutExtensions(IEnumerable<string> extensions)
        {
            if (extensions != null)
            {
                foreach (var ext in extensions)
                {
                    var result = NameWithoutExtensionsCore(ext);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return Name;
        }

        private string NameWithoutExtensionsCore(string ext)
        {
            if (ext[0] != '.')
                ext = '.' + ext;

            if (ext.EndsWithIgnoreCase(_extension))
            {
                var subExt = ext.RemoveRight(_extension.Length);
                if (_name.EndsWithIgnoreCase(subExt))
                {
                    return _name.RemoveRight(subExt.Length);
                }
            }

            return null;
        }

        public static bool IsDriveLetter(char letter)
        {
            return ('A' <= letter && letter <= 'Z') || ('a' <= letter && letter <= 'z');
        }

#if !SILVERLIGHT && !NET35
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNullOrEmpty(FilePath filePath)
        {
            return ReferenceEquals(filePath, null) || filePath._path == "";
        }

        public static string SanitizeFileName(string fileName, char? replacement = null)
        {
            fileName = fileName.Trim();
            var sb = new StringBuilder();
            for (int index = 0; index < fileName.Length; index++)
            {
                var c = fileName[index];
                foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
                {
                    if (c.Equals(invalidFileNameChar))
                    {
                        c = replacement ?? char.MinValue;
                        break;
                    }
                }

                if (c != char.MinValue)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string SanitizeFilePath(string fileName, char? replacement = null)
        {
            fileName = fileName.Trim();
            var sb = new StringBuilder();
            var parts = fileName.Split(Path.DirectorySeparatorChar);
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                for (int index = 0; index < part.Length; index++)
                {
                    var c = part[index];
                    foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
                    {
                        if (c.Equals(invalidFileNameChar))
                        {
                            c = replacement ?? char.MinValue;
                            break;
                        }
                    }

                    if (c != char.MinValue)
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }

        public FilePath ToAbsolute()
        {
            return IsAbsolute ? this : Parse(ToAbsoluteString());
        }

        public string ToAbsoluteString()
        {
            if (IsAbsolute)
                return _path;

            return Path.GetFullPath(_path == "" ? "." : _path);
        }

        private bool TryToRelativeCore(string basePath, string baseRoot, string baseLocalPath, out FilePath relativePath)
        {
            if (_driveOrHost != baseRoot)
            {
                relativePath = null;
                return false;
            }

            var path = WithoutRootFolderAsString();
            var result = GetRelativePath(path, baseLocalPath);

            // TODO Make the result FilePath without parsing
            relativePath = Parse(result);
            return true;
        }

        private bool TryToRelativeStringCore(string basePath, string baseRoot, string baseLocalPath, out string relativePath)
        {
            if (_driveOrHost != baseRoot)
            {
                relativePath = null;
                return false;
            }

            var path = WithoutRootFolderAsString();
            relativePath = GetRelativePath(path, baseLocalPath);
            return true;
        }

        private static string GetRelativePath(string path, string basePath)
        {
            var basePathParts = basePath.SplitAndRemoveEmpties(Path.DirectorySeparatorChar);
            var pathParts = path.Split(Path.DirectorySeparatorChar);
            int partsCount = Math.Min(basePathParts.Length, pathParts.Length);
            int samePartCount = 0;
            for (int i = 0; i < partsCount; i++)
            {
                if (!basePathParts[i].EqualsIgnoreCase(pathParts[i]))
                {
                    break;
                }

                samePartCount++;
            }

            if (0 == samePartCount)
            {
                return "..\\".Repeat(basePathParts.Length) + path;
            }

            var relativePath = new StringBuilder();
            for (int i = samePartCount; i < basePathParts.Length; i++)
            {
                relativePath.Append("..");
                relativePath.Append(Path.DirectorySeparatorChar);
            }

            for (int i = samePartCount; i < pathParts.Length; i++)
            {
                if (samePartCount < i)
                {
                    relativePath.Append(Path.DirectorySeparatorChar);
                }

                relativePath.Append(pathParts[i]);
            }

            return relativePath.ToString();
        }

        public FilePath ToRelative(FilePath basePath)
        {
            FilePath result;
            if (TryToRelative(basePath, out result))
            {
                return result;
            }

            throw new ArgumentException("Invalid base path.", nameof(basePath));
        }

        public FilePath ToRelative(string basePath)
        {
            return ToRelative(Parse(basePath));
        }

        public string ToRelativeString(FilePath basePath)
        {
            string result;
            if (TryToRelativeString(basePath, out result))
            {
                return result;
            }

            throw new ArgumentException("Invalid base path.", nameof(basePath));
        }

        public string ToRelativeString(string basePath)
        {
            return ToRelativeString(Parse(basePath));
        }

        public bool TryToRelative(FilePath basePath, out FilePath relativePath)
        {
            var blp = basePath.WithoutRootFolderAsString();
            return TryToRelativeCore(basePath.ToString(), basePath._driveOrHost, blp, out relativePath);
        }

        public bool TryToRelative(string basePath, out FilePath relativePath)
        {
            string br;
            string bf;
            var blp = CanonicalizePath(basePath, out br, out bf);
            return TryToRelativeCore(basePath, br + bf, blp, out relativePath);
        }

        public bool TryToRelativeString(FilePath basePath, out string relativePath)
        {
            var blp = basePath.WithoutRootFolderAsString();
            return TryToRelativeStringCore(basePath.ToString(), basePath._driveOrHost, blp, out relativePath);
        }

        public bool TryToRelativeString(string basePath, out string relativePath)
        {
            string br;
            string bf;
            var blp = CanonicalizePath(basePath, out br, out bf);
            return TryToRelativeStringCore(basePath, br + bf, blp, out relativePath);
        }

        public static string Combine(string left, string rigth)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (rigth == null)
                throw new ArgumentNullException(nameof(rigth));

            var baseFilePath = Parse(left);
            var filePath = baseFilePath.Combine(rigth);
            return filePath.ToString();
        }

        public static bool TryCombine(string left, string right, out string result)
        {
            if (left != null && right != null)
            {
                var baseFilePath = Parse(left);
                FilePath filePath;
                if (baseFilePath.TryCombine(right, out filePath))
                {
                    result = filePath.ToString();
                    return true;
                }
            }

            result = null;
            return false;
        }

        public FilePath Combine(FilePath other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.DriveOrHost != "")
                throw new ArgumentException("Absolute path cannot be in right part of paths combination.", nameof(other));

            if (other.IsCurrent)
                return this;

            if (other.RootFolder != "")
                return new FilePath(_driveOrHost, other.RootFolder, other.Prefix, other.Name, other.Extension, other.IsDirectory);

            var basePath = WithoutRootFolderAsString();
            if (basePath != "" && !_isDirectory)
                basePath += "\\";

            var com = new Parser(basePath + other.Prefix, true, _rootFolder);
            return new FilePath(_driveOrHost, _rootFolder, com.DirectoryPath, other.NameWithoutExtension, other.Extension, other.IsDirectory);
        }

        public FilePath Combine(string other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other == "")
                return this;

            var rel = new Parser(other, false, "");
            if (rel.ErrorType != FilePathParseErrorType.None)
                throw new ArgumentException("Invalid file path.", nameof(other));

            if (rel.DriveOrHost != "")
                throw new ArgumentException("Absolute path cannot be in right part of paths combination.", nameof(other));

            if (rel.RootFolder != "")
                return new FilePath(_driveOrHost, rel.RootFolder, rel.Prefix, rel.Name, rel.Extension, rel.IsDirectory);

            var value = WithoutRootFolderAsString();
            if (value == "")
            {
                value = rel.DirectoryPath;
            }
            else if (_isDirectory)
            {
                value = value + rel.Prefix;
            }
            else
            {
                value = value + "\\" + rel.Prefix;
            }

            var com = new Parser(value, true, _rootFolder);
            return new FilePath(_driveOrHost, _rootFolder, com.DirectoryPath, rel.Name, rel.Extension, rel.IsDirectory);
        }

        public FilePath CombineOrNull(FilePath other)
        {
            FilePath result;
            return TryCombine(other, out result) ? result : null;
        }

        public FilePath CombineOrOther(FilePath other)
        {
            FilePath result;
            return TryCombine(other, out result) ? result : other;
        }

        public FilePath CombineOrSelf(FilePath other)
        {
            FilePath result;
            return TryCombine(other, out result) ? result : this;
        }

        public bool TryCombine(string other, out FilePath result)
        {
            if (string.IsNullOrEmpty(other))
            {
                result = this;
                return true;
            }

            var rel = new Parser(other, false, "");
            if (rel.DriveOrHost == "" && rel.RootFolder == "")
            {
                var com = new Parser(_prefix + rel.Prefix, true, _rootFolder);
                if (com.ErrorType == FilePathParseErrorType.None)
                {
                    result = new FilePath(_driveOrHost, _rootFolder, com.Prefix, rel.Name, rel.Extension, rel.IsDirectory);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public bool TryCombine(FilePath other, out FilePath result)
        {
            if (IsNullOrEmpty(other))
            {
                result = this;
                return true;
            }

            if (other.DriveOrHost == "" && other.RootFolder == "")
            {
                var com = new Parser(DirectoryPath + other.Prefix, true, _rootFolder);
                if (com.ErrorType == FilePathParseErrorType.None)
                {
                    result = new FilePath(_driveOrHost, _rootFolder, com.DirectoryPath, other.NameWithoutExtension, other.Extension, other.IsDirectory);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static bool TryRelate(string a, string b, out FilePathRelation relation)
        {
            return Parse(a).TryRelateTo(Parse(b), out relation);
        }

        public bool IsDescendantOf(FilePath other)
        {
            FilePathRelation rel;
            return TryRelateTo(other, out rel)
                   && (rel == FilePathRelation.Child || rel == FilePathRelation.ImplicitChild);
        }

        public bool IsAncestorOf(params FilePath[] paths)
        {
            return IsAncestorOf((IEnumerable<FilePath>)paths);
        }

        public bool IsAncestorOf(IEnumerable<FilePath> paths)
        {
            if (paths == null)
            {
                return false;
            }

            foreach (var path in paths)
            {
                if (!IsAncestorOf(path))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAncestorOf(FilePath other)
        {
            FilePathRelation rel;
            return TryRelateTo(other, out rel)
                   && (rel == FilePathRelation.Parent || rel == FilePathRelation.ImplicitParent);
        }

        public bool TryRelateTo(FilePath other, out FilePathRelation relation)
        {
            if (other == null)
            {
                relation = FilePathRelation.None;
                return false;
            }

            bool sameRoot = _driveOrHost.EqualsIgnoreCase(other._driveOrHost)
                            && _rootFolder.EqualsIgnoreCase(other._rootFolder);

            int start = 0;
            int end = 0;

            var self = DirectoryPath.GetEnumerator();
            var target = other.DirectoryPath.GetEnumerator();

            bool hasSelf = self.MoveNext();
            bool hasTarget = target.MoveNext();

            if (hasSelf && hasTarget)
            {
                bool hasSeparator = false;

                while (hasSelf && hasTarget)
                {
                    if (CharExtensions.Equals(self.Current, target.Current,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Stretch common range
                        // 1bsd+
                        // abcdef
                        //  ^^
                        //  ^^^

                        end++;

                        if (self.Current == '\\')
                        {
                            hasSeparator = true;
                        }
                    }
                    else if (end < start)
                    {
                        // Find common start
                        // 1bsd+
                        // abcdef
                        //  ^
                        //   ^

                        start++;
                        end = start;
                    }
                    else
                    {
                        // Common range end
                        // 1bsd+
                        // abcdef
                        //  ^^^
                        //  ^^^|

                        break;
                    }

                    hasSelf = self.MoveNext();
                    hasTarget = target.MoveNext();
                }

                if (end <= start)
                {
                    relation = FilePathRelation.Sibling;
                    return sameRoot;
                }

                if (hasSelf)
                {
                    if (hasTarget)
                    {
                        relation = hasSeparator
                            // d1/c/
                            // d1/d2/
                            //    |
                            ? FilePathRelation.CommonAncestor
                            // d1/
                            // dA/dB/
                            //  |
                            : FilePathRelation.Sibling;
                    }
                    else
                    {
                        if (hasSeparator)
                        {
                            // d/c/
                            // d/
                            //   |
                            relation = FilePathRelation.Child;
                        }
                        else
                        {
                            // d/
                            // 
                            // |
                            relation = FilePathRelation.ImplicitChild;
                            Debug.Fail("RelativelyChild");
                        }
                    }
                }
                else if (hasTarget)
                {
                    if (hasSeparator)
                    {
                        // d1/
                        // d1/d2/
                        //    |
                        relation = FilePathRelation.Parent;
                    }
                    else
                    {
                        // 
                        // d/
                        // |
                        relation = FilePathRelation.ImplicitChild;
                        Debug.Fail("RelativelyChild");
                    }
                }
                else if (IsDirectory == other.IsDirectory)
                {
                    relation = FilePathRelation.Equal;
                }
                else if (IsDirectory)
                {
                    relation = FilePathRelation.Parent;
                }
                else if (other.IsDirectory)
                {
                    relation = FilePathRelation.Child;
                }
                else
                {
                    relation = FilePathRelation.Equal;
                }
            }
            else if (hasSelf)
            {
                // d/
                // 
                // |
                relation = FilePathRelation.ImplicitChild;
            }
            else
            {
                // 
                // d/
                // |
                relation = FilePathRelation.ImplicitParent;
            }

            return sameRoot;
        }

        public FilePath WithoutDriveOrHost() => new FilePath("", _rootFolder, _prefix, _name, _extension, _isDirectory);
        public string WithoutDriveOrHostAsString() => _rootFolder + _prefix + _name + _extension + (_isDirectory ? "\\" : "");

        public FilePath WithoutDriveOrHostAndExtension() => new FilePath("", _rootFolder, _prefix, _name, "", false);
        public string WithoutDriveOrHostAndExtensionAsString() => _rootFolder + _prefix + _name;

        public FilePath WithoutExtension() => new FilePath(_driveOrHost, _rootFolder, _prefix, _name, "", false);
        public string WithoutExtensionAsString() => _driveOrHost + _rootFolder + _prefix + _name;

        [CanBeNull]
        public FilePath WithoutDriveOrHostAndName() => Parent?.WithoutDriveOrHost();
        public string WithoutDriveOrHostAndNameAsString() => _rootFolder + _prefix;

        [CanBeNull]
        public FilePath WithoutDriveOrHostAndFileName() => _isDirectory ? WithoutDriveOrHost() : Parent?.WithoutDriveOrHost();
        public string WithoutDriveOrHostAndFileNameAsString() => _isDirectory ? _rootFolder + _prefix + _name + _extension + "\\" : _rootFolder + _prefix;

        [CanBeNull]
        public FilePath WithoutName() => Parent;
        public string WithoutNameAsString() => _driveOrHost + _rootFolder + _prefix;

        [CanBeNull]
        public FilePath WithoutFileName() => _isDirectory ? this : Parent;
        public string WithoutFileNameAsString() => _isDirectory ? _path : _driveOrHost + _rootFolder + _prefix;

        public FilePath WithoutRootFolder() => new FilePath("", "", _prefix, _name, _extension, _isDirectory);
        public string WithoutRootFolderAsString() => _prefix + _name + _extension + (_isDirectory ? "\\" : "");

        public FilePath WithoutRootFolderAndExtension() => new FilePath("", "", _prefix, _name, "", false);
        public string WithoutRootFolderAndExtensionAsString() => _prefix + _name;

        [CanBeNull]
        public FilePath WithoutRootFolderAndName() => Parent?.WithoutRootFolder();
        public string WithoutRootFolderAndNameAsString() => _prefix;

        [CanBeNull]
        public FilePath WithoutRootFolderAndFileName() => _isDirectory ? WithoutRootFolder() : Parent?.WithoutRootFolder();
        public string WithoutRootFolderAndFileNameAsString() => _isDirectory ? _prefix + _name + _extension + "\\" : _prefix;

        /// <summary>
        /// Добавляет расширение файла.
        /// </summary>
        /// <param name="suffix">Может начинаться или оканчиваться с точки.</param>
        /// <returns></returns>
        public FilePath WithNameSuffix(string suffix)
        {
            return WithExtension(_extension + "." + suffix.Trim('.'));
        }

        public FilePath WithNamePrefix(string prefix)
        {
            return WithName(prefix + Name);
        }

        /// <summary>
        /// Добавляет префикс перед расширением файла.
        /// </summary>
        /// <param name="prefix">Может начинаться или оканчиваться с точки.</param>
        /// <returns></returns>
        public FilePath WithExtensionPrefix(string prefix)
        {
            return WithExtension("." + prefix.Trim('.') + _extension);
        }

        /// <summary>
        /// Возвращяет новый путь с заданным расширением.
        /// </summary>
        /// <param name="newExtension">Расширение имени файла начинающееся с точки (например: .ext).</param>
        /// <returns></returns>
        public FilePath WithExtension(string newExtension)
        {
            if (string.IsNullOrEmpty(newExtension))
            {
                newExtension = "";
            }
            else if (!newExtension.StartsWith("."))
            {
                newExtension = '.' + newExtension;
            }

            var suffix = newExtension.SubstringBeforeLast('.');
            newExtension = newExtension.Substring(suffix.Length);
            return new FilePath(_driveOrHost, _rootFolder, _prefix, _name + suffix, newExtension, _isDirectory);
        }

        public FilePath WithName(string newName)
        {
            string ext = _extension;
            if (newName == null)
            {
                newName = "";
            }
            else
            {
                int pos = newName.LastIndexOf('.');
                if (0 == pos)
                {
                    ext = newName;
                    newName = "";
                }
                else if (0 < pos)
                {
                    ext = newName.Substring(pos);
                    newName = newName.Substring(0, pos);
                }
            }

            return new FilePath(_driveOrHost, _rootFolder, _prefix, newName, ext, _isDirectory);
        }

        public FilePath WithRoot(string newRoot)
        {
            var newRootFolder = _rootFolder;
            if (!string.IsNullOrEmpty(newRoot))
            {
                var root = newRoot.Trim().Replace('/', '\\');

                if (root.StartsWith(@"\\?\UNC\"))
                {
                    root = root.Substring(7);
                }
                else if (root.StartsWith(@"\\?\"))
                {
                    root = root.Substring(4);
                }

                if (root.Length == 1 || root.EndsWith(':'))
                {
                    if (!IsDriveLetter(root[0]))
                    {
                        throw new ArgumentOutOfRangeException(nameof(newRoot), $"Invalid drive letter '{newRoot}'.");
                    }

                    if (root.Length == 1)
                    {
                        root += ":";
                    }
                }
                else if (root.Length == 3 && root[1] == ':' && root[2] == '\\')
                {
                    if (!IsDriveLetter(root[0]))
                    {
                        throw new ArgumentOutOfRangeException(nameof(newRoot), $"Invalid drive letter '{newRoot}'.");
                    }

                    newRootFolder = "\\";
                    root = root[0] + ":";
                }
                else if (root.StartsWith('\\'))
                {
                    root = root.TrimStart('\\');
                    var share = root.SubstringAfter('\\');
                    if (share == "" || share.JoinsWith('\\'))
                    {
                        throw new ArgumentException($"Invalid network share root '{newRoot}'.");
                    }

                    root = root.RemoveRight(share.Length + 1);
                    newRootFolder = '\\' + share.EnsureEndsWith(Path.DirectorySeparatorChar);

                    root = @"\\" + root;
                }
                else
                {
                    throw new ArgumentException($"Invalid file path root '{newRoot}'.", nameof(newRoot));
                }

                newRoot = root;
            }
            else
            {
                newRoot = "";
            }

            return new FilePath(newRoot, newRootFolder, _prefix, _name, _extension, _isDirectory);
        }

        public FilePath SubpathAfter(int count = 1)
        {
            string[] parents = _prefix.SplitAndRemoveEmpties(Path.DirectorySeparatorChar);
            if (parents.Length < count) throw new ArgumentOutOfRangeException(nameof(count));

            var newParentPath = "";
            if (count < parents.Length)
            {
                newParentPath = string.Join(Path.DirectorySeparatorChar.ToString(), parents.Subarray(count)) + Path.DirectorySeparatorChar;
            }

            return new FilePath("", "", newParentPath, _name, _extension, _isDirectory);
        }

        public FilePath WithoutExtensions(int count = 1)
        {
            var parts = _name.Split('.');
            if (parts.Length < count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (parts.Length == count)
            {
                return new FilePath(_driveOrHost, _rootFolder, _prefix, parts[0], "", _isDirectory);
            }

            var ext = "." + parts[parts.Length - 1];
            parts = parts.Shrink(count);
            var newName = string.Join(".", parts);
            return new FilePath(_driveOrHost, _rootFolder, _prefix, newName, ext, _isDirectory);
        }

        public FilePath SubpathBefore(string subpath, bool include = false)
        {
            if (subpath == null)
                throw new ArgumentNullException(nameof(subpath));

            subpath = subpath.Replace('/', '\\').Trim('\\');

            var s = _prefix.GetIndexedEnumerator();
            var t = subpath.GetIndexedEnumerator();

            if (!t.MoveNext())
                return include ? this : null;

            var start = s.MoveWith(ref t);
            bool matched = t.IsFinished;
            bool matchedByPrefix = matched;
            if (!matched)
            {
                if (_name != "")
                {
                    s = _name.GetIndexedEnumerator();
                    s.MoveWith(ref t);
                    matched = s.IsFinished && t.IsFinished;
                }

                if (_extension != "")
                {
                    matched = false;
                    if (!t.IsFinished)
                    {
                        s = _extension.GetIndexedEnumerator();
                        s.MoveWith(ref t);
                        matched = s.IsFinished && t.IsFinished;
                    }
                }
            }

            if (!matched)
                throw new ArgumentOutOfRangeException(
                    nameof(subpath), subpath, "A path does not contain a subpath.");

            if (include)
            {
                if (s.IsFinished)
                {
                    return matchedByPrefix ? Parent : this;
                }

                start = s.Index + 1;
            }
            else if (start < 0)
            {
                return Parent;
            }

            if (start == 0)
                return new FilePath(_driveOrHost, _rootFolder, "", "", "", true);

            string rest;
            var newPrefix = _prefix.Substring(0, start - 1);
            var newName = newPrefix.SubstringAfterLast('\\', out rest);
            if (newName == "")
            {
                newName = newPrefix;
                newPrefix = "";
            }
            else
            {
                newPrefix = rest + "\\";
            }

            rest = newName.SubstringAfterLast('.', out newName, true);
            return new FilePath(_driveOrHost, _rootFolder, newPrefix, newName, rest, true);
        }

        /// <summary>
        /// Возвращает результат сравнения с заданным путем.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is FilePath && Equals((FilePath)obj);
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public override string ToString()
        {
            return _path;
        }

        #region IEquatable<FilePath> Members

        /// <summary>
        /// Возвращает результат сравнения с заданным путем.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        /// <returns></returns>
        public bool Equals(FilePath other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            else if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (_isDirectory != other._isDirectory)
            {
                var delta = _path.Length - other._path.Length;
                if (delta == 1)
                {
                    return _path.StartsWith(other._path, StringComparison.OrdinalIgnoreCase) && _isDirectory;
                }
                else if (delta == -1)
                {
                    return other._path.StartsWith(_path, StringComparison.OrdinalIgnoreCase) && other._isDirectory;
                }

                return false;
            }

            return _path.Equals(other._path, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        /// <summary>
        /// Комбинирует два пути — базовый и относительный.
        /// </summary>
        /// <param name="left">Базовый путь</param>
        /// <param name="right">Относительный путь</param>
        /// <returns>Комбинированный путь</returns>
        public static FilePath operator /(FilePath left, FilePath right)
        {
            if (ReferenceEquals(left, null))
                return right;

            return left.Combine(right);
        }

        /// <summary>
        /// Комбинирует два пути — базовый и относительный.
        /// </summary>
        /// <param name="left">Базовый путь</param>
        /// <param name="right">Относительный путь</param>
        /// <returns>Комбинированный путь</returns>
        public static FilePath operator /(FilePath left, string right)
        {
            if (ReferenceEquals(left, null))
                return Parse(right);

            return left.Combine(right);
        }

        /// <summary>
        /// Комбинирует два пути — базовый и относительный.
        /// </summary>
        /// <param name="left">Базовый путь</param>
        /// <param name="right">Относительный путь</param>
        /// <returns>Комбинированный путь</returns>
        public static FilePath operator /(string left, FilePath right)
        {
            if (ReferenceEquals(left, null))
                return Parse(right);

            return Parse(left).Combine(right);
        }

        /// <summary>
        /// Возвращает результат сравнения с двух путей.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator ==(FilePath left, FilePath right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Возвращает результат сравнения с двух путей.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator ==(FilePath left, string right)
        {
            if (ReferenceEquals(left, null))
                return right == null;

            return Equals(left.ToString(), right);
        }

        /// <summary>
        /// Возвращает результат сравнения с двух путей.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator ==(string left, FilePath right)
        {
            if (ReferenceEquals(right, null))
                return left == null;

            return Equals(left, right.ToString());
        }

        public static explicit operator FilePath(string path)
        {
            return Parse(path);
        }

        public static implicit operator string(FilePath path)
        {
            return path.ToString();
        }

        /// <summary>
        /// Возвращает результат сравнения двух путей.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator !=(FilePath left, FilePath right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Возвращает результат сравнения двух путей.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator !=(FilePath left, string right)
        {
            if (ReferenceEquals(left, null))
                return right != null;

            return !Equals(left.ToString(), right);
        }

        /// <summary>
        /// Возвращает результат сравнения двух путей.
        /// Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator !=(string left, FilePath right)
        {
            if (ReferenceEquals(right, null))
                return left != null;

            return !Equals(left, right.ToString());
        }

        /// <summary>
        /// Создает относительный путь на основе базового из полного.
        /// </summary>
        /// <param name="absolutePath">Полный путь</param>
        /// <param name="basePath">Базовый путь</param>
        /// <returns>Относительный путь</returns>
        public static FilePath operator %(FilePath absolutePath, FilePath basePath)
        {
            if (ReferenceEquals(absolutePath, null))
                return basePath;

            return absolutePath.ToRelative(basePath);
        }

        /// <summary>
        /// Создает относительный путь на основе базового из полного.
        /// </summary>
        /// <param name="absolutePath">Полный путь</param>
        /// <param name="basePath">Базовый путь</param>
        /// <returns>Относительный путь</returns>
        public static FilePath operator %(FilePath absolutePath, string basePath)
        {
            if (ReferenceEquals(absolutePath, null))
                return Parse(basePath);

            return absolutePath.ToRelative(Parse(basePath));
        }

        /// <summary>
        /// Создает относительный путь на основе базового из полного.
        /// </summary>
        /// <param name="absolutePath">Полный путь</param>
        /// <param name="basePath">Базовый путь</param>
        /// <returns>Относительный путь</returns>
        public static FilePath operator %(string absolutePath, FilePath basePath)
        {
            if (absolutePath == null)
                return basePath;

            return Parse(absolutePath).ToRelative(basePath);
        }
    }

    public enum FilePathRelation
    {
        None,
        /// <summary>
        /// Равенство
        /// </summary>
        Equal,
        /// <summary>
        /// Вложенность
        /// </summary>
        Child,
        /// <summary>
        /// Включение
        /// </summary>
        Parent,
        /// <summary>
        /// Неявная вложенность
        /// </summary>
        ImplicitChild,
        /// <summary>
        /// Неявное включение
        /// </summary>
        ImplicitParent,
        /// <summary>
        /// Общая корневая папка
        /// </summary>
        Sibling,
        /// <summary>
        /// Общая папка
        /// </summary>
        CommonAncestor,
    }
}
#endif