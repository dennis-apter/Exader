using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Exader.IO
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed partial class FilePath : IEquatable<FilePath>
    {
        private const string AllItemsPattern = "*";
        private static readonly string DirectoryName = ".";

        public static readonly FilePath Empty = new FilePath();
        public static readonly FilePath RelativeRoot;

        /// <summary>
        ///     Storing drive letter or server computer name of shared folder.
        /// </summary>
        [NotNull] private readonly string _driveOrHost;

        [NotNull] private readonly string _path;

        /// <summary>
        ///     Storing name of shared folder or root directory.
        /// </summary>
        [NotNull] private readonly string _rootFolder;

        private bool _isDirectory = true;

        private FilePath _parent;


        /// <summary>
        ///     Создает экземпляр <see cref="FilePath" />
        ///     для описания фалового пути к текущей директории.
        /// </summary>
        [DebuggerStepThrough]
        public FilePath()
        {
            _driveOrHost = "";
            _rootFolder = "";
            ParentPath = "";
            NameWithoutExtension = "";
            Extension = "";
            _path = "";
        }

        private FilePath(string driveOrHost, string rootFolder, string parentPath, string name, string extension,
            bool isDirectory)
        {
            _driveOrHost = driveOrHost;
            _rootFolder = rootFolder;
            ParentPath = parentPath;
            NameWithoutExtension = name;
            Extension = extension;
            _isDirectory = isDirectory;

            _path = CombinePath(driveOrHost, rootFolder, parentPath, name, extension, isDirectory);
        }

        private FilePath(FilePath parent, string name, string extension, bool isDirectory)
        {
            if (parent != null)
            {
                _parent = parent;
                _driveOrHost = parent._driveOrHost;
                _rootFolder = parent._rootFolder;
                ParentPath = parent.DirectoryPath;
            }
            else
            {
                _driveOrHost = "";
                _rootFolder = "";
                ParentPath = "";
            }

            NameWithoutExtension = name;
            Extension = extension;
            _isDirectory = isDirectory;

            _path = CombinePath(_driveOrHost, _rootFolder, ParentPath, name, extension, isDirectory);
        }

        [Obsolete("Use ToAbsoluteString() or ToAbsolute() method instead", true)]
        public string AbsolutePath => ToAbsoluteString();

        /// <summary>
        ///     Возвращает себя, если является путем директории иначе путь родительской директории.
        /// </summary>
        public string DirectoryPath
        {
            get
            {
                var rootLength = _driveOrHost.Length + _rootFolder.Length;
                return _isDirectory && _path.Length > rootLength
                    ? ParentPath + NameWithoutExtension + Extension + Separator
                    : ParentPath;
            }
        }

        public string Drive => IsLocal ? _driveOrHost : "";
        public char? DriveLetter => IsLocal ? (char?) _driveOrHost[0] : null;

        public string DriveOrHost => _driveOrHost;

        [NotNull]
        public string Extension { get; }

        [Obsolete("Use ToString() method instead", true)]
        public string FullPath => ToString();

        public bool HasDriveOrHost => _driveOrHost != "";
        public bool HasRootFolder => _rootFolder != "";
        public string Host => IsNetwork ? _driveOrHost : "";

        /// <summary>
        ///     Указывает на то, что путь является абсолютным и обладает именем диска.
        /// </summary>
        /// <example>\</example>
        public bool IsAbsolute => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? _driveOrHost != "" && _rootFolder != "" : _rootFolder != "";

        public bool IsCurrent => _rootFolder == "" && ParentPath == "" && NameWithoutExtension == "" && Extension == "";

        public bool IsDirectory => _isDirectory || NameWithoutExtension == "" && Extension == "";

        public bool IsExternal => NameWithoutExtension == ".." || ParentPath.StartsWith("..");

        public bool IsLocal => _driveOrHost.Length == 2 && _driveOrHost[1] == ':';
        public bool IsNetwork => 2 < _driveOrHost.Length && _driveOrHost[0] == '\\';

        /// <summary>
        ///     Relative or Absolute
        /// </summary>
        public bool IsRoot => _rootFolder != ""
                              && ParentPath == ""
                              && NameWithoutExtension == ""
                              && Extension == "";

        public string Name => NameWithoutExtension + Extension;

        [NotNull]
        public string NameWithoutExtension { get; }

        [NotNull]
        public string Basename => IsDirectory ? Name : NameWithoutExtension;

        [NotNull]
        public string FileExtension => IsDirectory ? string.Empty : Extension;

        /// <summary>
        ///     Возвращает путь верхнего уровня.
        /// </summary>
        [CanBeNull]
        public FilePath Parent
        {
            get
            {
                if (_parent != null || NameWithoutExtension == "" && Extension == "")
                    return _parent;

                if (ParentPath != "")
                {
                    if (ParentPath.Length < 2)
                        throw new InvalidOperationException("Invalid file path.");

                    string prefix;
                    string fullName;
                    string name;
                    string ext;
                    var i = ParentPath.LastIndexOf('\\', ParentPath.Length - 2);
                    if (i == -1)
                    {
                        fullName = ParentPath.Substring(0, ParentPath.Length - 1);
                        prefix = "";
                    }
                    else
                    {
                        fullName = ParentPath.Substring(i + 1, ParentPath.Length - i - 2);
                        prefix = ParentPath.Substring(0, i + 1);
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

        [NotNull]
        public string ParentPath { get; }

        [NotNull]
        [Obsolete("Use " + nameof(ParentPath) + " instead", true)]
        public string Prefix => ParentPath;

        public string RootFolder => _rootFolder;

        #region IEquatable<FilePath> Members

        /// <inheritdoc />
        /// <summary>
        ///     Возвращает результат сравнения с заданным путем.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        /// <returns></returns>
        public bool Equals(FilePath other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (_isDirectory != other._isDirectory)
            {
                var delta = _path.Length - other._path.Length;
                if (delta == 1)
                    return _path.StartsWith(other._path, StringComparison.OrdinalIgnoreCase) && _isDirectory;

                if (delta == -1)
                    return other._path.StartsWith(_path, StringComparison.OrdinalIgnoreCase) && other._isDirectory;

                return false;
            }

            return _path.Equals(other._path, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

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

        public static bool IsDriveLetter(char letter)
        {
            return 'A' <= letter && letter <= 'Z' || 'a' <= letter && letter <= 'z';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(FilePath filePath)
        {
            return ReferenceEquals(filePath, null) || filePath._path == "";
        }

        /// <summary>
        ///     Комбинирует два пути — базовый и относительный.
        /// </summary>
        /// <param name="left">Базовый путь</param>
        /// <param name="right">Относительный путь</param>
        /// <returns>Комбинированный путь</returns>
        public static FilePath operator |(FilePath left, string right)
        {
            if (ReferenceEquals(left, null))
                return Parse(right);

            return left.Combine(right);
        }

        /// <summary>
        ///     Комбинирует два пути — базовый и относительный.
        /// </summary>
        /// <param name="left">Базовый путь</param>
        /// <param name="right">Относительный путь</param>
        /// <returns>Комбинированный путь</returns>
        public static FilePath operator |(string left, FilePath right)
        {
            if (ReferenceEquals(left, null))
                return Parse(right);

            return Parse(left).Combine(right);
        }

        /// <summary>
        ///     Комбинирует два пути — базовый и относительный.
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
        ///     Комбинирует два пути — базовый и относительный.
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
        ///     Комбинирует два пути — базовый и относительный.
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
        ///     Возвращает результат сравнения с двух путей.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator ==(FilePath left, FilePath right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Возвращает результат сравнения с двух путей.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator ==(FilePath left, string right)
        {
            if (ReferenceEquals(left, null))
                return right == null;

            return Equals(left.ToString(), right);
        }

        /// <summary>
        ///     Возвращает результат сравнения с двух путей.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator ==(string left, FilePath right)
        {
            if (ReferenceEquals(right, null))
                return left == null;

            return Equals(left, right.ToString());
        }

        public static explicit operator FilePath(string path)
        {
            return path != null ? Parse(path) : null;
        }

        public static implicit operator string(FilePath path)
        {
            return path?.ToString();
        }

        /// <summary>
        ///     Возвращает результат сравнения двух путей.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator !=(FilePath left, FilePath right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Возвращает результат сравнения двух путей.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator !=(FilePath left, string right)
        {
            if (ReferenceEquals(left, null))
                return right != null;

            return !Equals(left.ToString(), right);
        }

        /// <summary>
        ///     Возвращает результат сравнения двух путей.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        public static bool operator !=(string left, FilePath right)
        {
            if (ReferenceEquals(right, null))
                return left != null;

            return !Equals(left, right.ToString());
        }

        /// <summary>
        ///     Создает относительный путь на основе базового из полного.
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
        ///     Создает относительный путь на основе базового из полного.
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
        ///     Создает относительный путь на основе базового из полного.
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

        public static string SanitizeFileName(string fileName, char? replacement = null)
        {
            fileName = fileName.Trim();
            var sb = new StringBuilder();
            for (var index = 0; index < fileName.Length; index++)
            {
                var c = fileName[index];
                foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
                    if (c.Equals(invalidFileNameChar))
                    {
                        c = replacement ?? char.MinValue;
                        break;
                    }

                if (c != char.MinValue)
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string SanitizeFilePath(string fileName, char? replacement = null)
        {
            fileName = fileName.Trim();
            var sb = new StringBuilder();
            var parts = fileName.Split(Sep);
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                for (var index = 0; index < part.Length; index++)
                {
                    var c = part[index];
                    foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
                        if (c.Equals(invalidFileNameChar))
                        {
                            c = replacement ?? char.MinValue;
                            break;
                        }

                    if (c != char.MinValue)
                        sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static bool TryCombine(string left, string right, out string result)
        {
            if (left != null && right != null)
            {
                var baseFilePath = Parse(left);
                if (baseFilePath.TryCombine(right, out var filePath))
                {
                    result = filePath.ToString();
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

        private static string CombinePath(string driveOrHost, string rootFolder, string prefix, string name,
            string extension, bool isDirectory)
        {
            var trailingSlash = isDirectory && (name != "" || extension != "") ? Separator : "";
            var path = driveOrHost + rootFolder + prefix + name + extension + trailingSlash;
            return path; // == "" ? "." : path;
        }

        private static string GetRelativePath(string path, string basePath)
        {
            var basePathParts = basePath.SplitAndRemoveEmpties(Sep);
            var pathParts = path.Split(Sep);
            var partsCount = Math.Min(basePathParts.Length, pathParts.Length);
            var samePartCount = 0;
            for (var i = 0; i < partsCount; i++)
            {
                if (!basePathParts[i].EqualsIgnoreCase(pathParts[i]))
                    break;

                samePartCount++;
            }

            if (0 == samePartCount)
                return "..\\".Repeat(basePathParts.Length) + path;

            var relativePath = new StringBuilder();
            for (var i = samePartCount; i < basePathParts.Length; i++)
            {
                relativePath.Append("..");
                relativePath.Append(Separator);
            }

            for (var i = samePartCount; i < pathParts.Length; i++)
            {
                if (samePartCount < i)
                    relativePath.Append(Separator);

                relativePath.Append(pathParts[i]);
            }

            return relativePath.ToString();
        }

        public FilePath AsDirectory()
        {
            if (IsDirectory)
                return this;

            return new FilePath(_driveOrHost, _rootFolder, ParentPath, NameWithoutExtension, Extension, true);
        }

        public FilePath AsRooted()
        {
            if (HasRootFolder)
                return this;

            return new FilePath(_driveOrHost, Separator, ParentPath, NameWithoutExtension, Extension, _isDirectory);
        }

        public FilePath AsLocal()
        {
            if (HasRootFolder || HasDriveOrHost)
                return WithoutRootFolder();

            return this;
        }

        public FilePath Combine(FilePath other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.DriveOrHost != "")
                throw new ArgumentException("Absolute path cannot be in right part of paths combination.",
                    nameof(other));

            if (other.IsCurrent)
                return this;

            if (other.RootFolder != "")
                return new FilePath(_driveOrHost, other.RootFolder, other.ParentPath, other.Name, other.Extension,
                    other.IsDirectory);

            var basePath = WithoutRootFolderAsString();
            if (basePath != "" && !_isDirectory)
                basePath += Separator;

            var com = new Parser(basePath + other.ParentPath, true, _rootFolder);
            return new FilePath(_driveOrHost, _rootFolder, com.DirectoryPath, other.NameWithoutExtension,
                other.Extension, other.IsDirectory);
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
                throw new ArgumentException("Absolute path cannot be in right part of paths combination.",
                    nameof(other));

            if (rel.RootFolder != "")
                return new FilePath(_driveOrHost, rel.RootFolder, rel.Prefix, rel.Name, rel.Extension, rel.IsDirectory);

            var value = WithoutRootFolderAsString();
            if (value == "")
                value = rel.DirectoryPath;
            else if (_isDirectory)
                value = value + rel.Prefix;
            else
                value = value + Separator + rel.Prefix;

            var com = new Parser(value, true, _rootFolder);
            return new FilePath(_driveOrHost, _rootFolder, com.DirectoryPath, rel.Name, rel.Extension, rel.IsDirectory);
        }

        public FilePath CombineOrNull(FilePath other)
        {
            return TryCombine(other, out var result) ? result : null;
        }

        public FilePath CombineOrOther(FilePath other)
        {
            return TryCombine(other, out var result) ? result : other;
        }

        public FilePath CombineOrSelf(FilePath other)
        {
            return TryCombine(other, out var result) ? result : this;
        }

        public FilePath Directory()
        {
            return IsDirectory ? this : Parent;
        }

        public FilePath Directory(string name)
        {
            return Item(name, true);
        }

        /// <summary>
        ///     Возвращает результат сравнения с заданным путем.
        ///     Сравнение произодится без учёта регистра и с инвариантной культурой.
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            var a = obj as FilePath;
            return a != null && Equals(a);
        }

        public FilePath File(string name)
        {
            return Item(name, false);
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public bool IsAncestorOf(params FilePath[] paths)
        {
            return IsAncestorOf((IEnumerable<FilePath>) paths);
        }

        public bool IsAncestorOf(IEnumerable<FilePath> paths)
        {
            if (paths == null)
                return false;

            foreach (var path in paths)
                if (!IsAncestorOf(path))
                    return false;

            return true;
        }

        public bool IsAncestorOf(FilePath other)
        {
            return TryRelateTo(other, out var rel) &&
                   (rel == FilePathRelation.Parent || rel == FilePathRelation.ImplicitParent);
        }

        public bool IsDescendantOf(FilePath other)
        {
            return TryRelateTo(other, out var rel) &&
                   (rel == FilePathRelation.Child || rel == FilePathRelation.ImplicitChild);
        }

        public string NameWithoutExtensions(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                var result = NameWithoutExtensionsCore(ext);
                if (result != null)
                    return result;
            }

            return Name;
        }

        public string NameWithoutExtensions(params string[] extensions)
        {
            return NameWithoutExtensions((IEnumerable<string>) extensions);
        }

        public string NameWithoutExtensions(IEnumerable<string> extensions)
        {
            if (extensions != null)
                foreach (var ext in extensions)
                {
                    var result = NameWithoutExtensionsCore(ext);
                    if (result != null)
                        return result;
                }

            return Name;
        }

        public FilePath WithoutAnscestors(int count = 1)
        {
            var parents = ParentPath.SplitAndRemoveEmpties(Sep);
            if (parents.Length < count)
                throw Guard.FilePathParentsOutOfRange(this, count);

            var newParentPath = "";
            if (count < parents.Length)
                newParentPath = string.Join(Separator, parents.Subarray(count)) + Separator;

            return new FilePath("", "", newParentPath, NameWithoutExtension, Extension, _isDirectory);
        }

        public FilePath WithoutParents(int count = 1)
        {
            var parent = Parent;
            while (count > 0 && parent != null)
            {
                parent = parent.Parent;
                count--;
            }

            if (parent == null)
                throw Guard.FilePathParentsOutOfRange(this, count);

            return new FilePath(parent, NameWithoutExtension, Extension, _isDirectory);
        }

        [Obsolete("Use " + nameof(WithoutAnscestors) + " instead", true)]
        public FilePath SubpathAfter(int offset = 1)
        {
            return WithoutAnscestors(offset);
        }

        [Obsolete("Use " + nameof(SubpathBefore) + " instead", true)]
        public FilePath Upon(string subpath, bool include = false)
        {
            return SubpathBefore(subpath, include);
        }

        public FilePath SubpathBefore(string subpath, bool include = false)
        {
            if (subpath == null)
                throw new ArgumentNullException(nameof(subpath));

            subpath = subpath.Replace('/', '\\').Trim('\\');

            var s = ParentPath.GetIndexedEnumerator();
            var t = subpath.GetIndexedEnumerator();

            if (!t.MoveNext())
                return include ? this : null;

            var start = s.MoveWith(ref t);
            var matched = t.IsFinished;
            var matchedByPrefix = matched;
            if (!matched)
            {
                if (NameWithoutExtension != "")
                {
                    s = NameWithoutExtension.GetIndexedEnumerator();
                    s.MoveWith(ref t);
                    matched = s.IsFinished && t.IsFinished;
                }

                if (Extension != "")
                {
                    matched = false;
                    if (!t.IsFinished)
                    {
                        s = Extension.GetIndexedEnumerator();
                        s.MoveWith(ref t);
                        matched = s.IsFinished && t.IsFinished;
                    }
                }
            }

            if (!matched)
                throw new ArgumentOutOfRangeException(
                    nameof(subpath), subpath,
                    $"Path {this} does not contain a subpath {subpath}.");

            if (include)
            {
                if (s.IsFinished)
                    return matchedByPrefix ? Parent : this;

                start = s.Index + 1;
            }
            else if (start < 0)
            {
                return Parent;
            }

            if (start == 0)
                return new FilePath(_driveOrHost, _rootFolder, "", "", "", true);

            var newPrefix = ParentPath.Substring(0, start - 1);
            var newName = newPrefix.SubstringAfterLast('\\', out var rest);
            if (newName == "")
            {
                newName = newPrefix;
                newPrefix = "";
            }
            else
            {
                newPrefix = rest + Separator;
            }

            rest = newName.SubstringAfterLast('.', out newName, true);
            return new FilePath(_driveOrHost, _rootFolder, newPrefix, newName, rest, true);
        }

        public FilePath ToAbsolute()
        {
            return IsAbsolute ? this : Parse(ToAbsoluteString());
        }

        public string ToAbsoluteString()
        {
            return IsAbsolute ? _path : Path.GetFullPath(_path == "" ? "." : _path);
        }

        public FilePath ToRelative(FilePath basePath)
        {
            return TryToRelative(basePath, out var result)
                ? result
                : throw new ArgumentException("Invalid base path.", nameof(basePath));
        }

        public FilePath ToRelative(string basePath)
        {
            return ToRelative(Parse(basePath));
        }

        public string ToRelativeString(FilePath basePath)
        {
            return TryToRelativeString(basePath, out var result)
                ? result
                : throw new ArgumentException("Invalid base path.", nameof(basePath));
        }

        public string ToRelativeString(string basePath)
        {
            return ToRelativeString(Parse(basePath));
        }

        public override string ToString()
        {
            return _path;
        }

        [Obsolete("Use " + nameof(WithoutAnscestors) + " instead", true)]
        public FilePath TrimAnscestors(int offset = 1)
        {
            return WithoutAnscestors(offset);
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
                var com = new Parser(ParentPath + rel.Prefix, true, _rootFolder);
                if (com.ErrorType == FilePathParseErrorType.None)
                {
                    result = new FilePath(_driveOrHost, _rootFolder, com.Prefix, rel.Name, rel.Extension,
                        rel.IsDirectory);
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
                var com = new Parser(DirectoryPath + other.ParentPath, true, _rootFolder);
                if (com.ErrorType == FilePathParseErrorType.None)
                {
                    result = new FilePath(_driveOrHost, _rootFolder, com.DirectoryPath, other.NameWithoutExtension,
                        other.Extension, other.IsDirectory);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public bool TryRelateTo(FilePath other, out FilePathRelation relation)
        {
            if (other == null)
            {
                relation = FilePathRelation.None;
                return false;
            }

            var sameRoot = _driveOrHost.EqualsIgnoreCase(other._driveOrHost)
                           && _rootFolder.EqualsIgnoreCase(other._rootFolder);

            var start = 0;
            var end = 0;

            var self = ((IEnumerable<char>) DirectoryPath).GetEnumerator();
            var target = ((IEnumerable<char>) other.DirectoryPath).GetEnumerator();
            try
            {
                var hasSelf = self.MoveNext();
                var hasTarget = target.MoveNext();

                if (hasSelf && hasTarget)
                {
                    var hasSeparator = false;

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
                                hasSeparator = true;
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
                                Debug.Assert(false, "RelativelyChild");
                            }
                        }
                    else if (hasTarget)
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
                            Debug.Assert(false, "RelativelyChild");
                        }
                    else if (IsDirectory == other.IsDirectory)
                        relation = FilePathRelation.Equal;
                    else if (IsDirectory)
                        relation = FilePathRelation.Parent;
                    else if (other.IsDirectory)
                        relation = FilePathRelation.Child;
                    else
                        relation = FilePathRelation.Equal;
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
            }
            finally
            {
                self.Dispose();
                target.Dispose();
            }

            return sameRoot;
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
            var blp = CanonicalizePath(basePath, out var br, out var bf);
            return TryToRelativeStringCore(basePath, br + bf, blp, out relativePath);
        }

        /// <summary>
        ///     Возвращяет новый путь с заданным расширением.
        /// </summary>
        /// <param name="newExtension">Расширение имени файла начинающееся с точки (например: .ext).</param>
        /// <returns></returns>
        public FilePath WithExtension(string newExtension)
        {
            if (string.IsNullOrEmpty(newExtension))
                newExtension = "";
            else if (!newExtension.StartsWith("."))
                newExtension = '.' + newExtension;

            var suffix = newExtension.SubstringBeforeLast('.');
            newExtension = newExtension.Substring(suffix.Length);
            return new FilePath(_driveOrHost, _rootFolder, ParentPath, NameWithoutExtension + suffix, newExtension,
                _isDirectory);
        }

        public FilePath WithNameWithoutExtension(string newName)
        {
            string newExtension;
            if (newName == null)
            {
                newName = "";
                newExtension = Extension;
            }
            else
            {
                newExtension = Extension == ""
                    ? newName.SubstringAfterLast('.', out newName)
                    : Extension;
            }

            return new FilePath(_driveOrHost, _rootFolder, ParentPath, newName, newExtension, _isDirectory);
        }

        /// <summary>
        ///     Добавляет префикс перед расширением файла.
        ///     Например: <code>{f.e}.WithExtensionPrefix("p") => {f.p.e}</code>
        /// </summary>
        /// <param name="prefix">Гарантируется наличие точки в начале и отсутвие точки в конце строки.</param>
        public FilePath WithExtensionPrefix(string prefix)
        {
            return WithExtension("." + prefix.Trim('.') + Extension);
        }

        /// <summary>
        ///     Добавляет новое расширение файла, оставляя старое в конце имени файла.
        ///     Например: <code>{f.e}.WithExtensionSuffix("s") => {f.e.s}</code>
        /// </summary>
        /// <param name="suffix">Гарантируется наличие точки в начале и отсутвие точки в конце строки.</param>
        public FilePath WithExtensionSuffix(string suffix)
        {
            return WithExtension(Extension + "." + suffix.Trim('.'));
        }

        /// <summary>
        ///     Добавляет вставку между именем и расширением.
        ///     Например: <code>{f.e}.WithNameInfix("i") => {fi.e}</code>
        /// </summary>
        public FilePath WithNameInfix(string infix)
        {
            return WithName(NameWithoutExtension + infix + Extension);
        }

        /// <summary>
        ///     Заменяет имя вместе с раширением.
        ///     Например: <code>{f.e}.WithBasename("n.n") => {n.n}</code>
        /// </summary>
        public FilePath WithName(string newName)
        {
            if (string.IsNullOrEmpty(newName) || newName.IsRepeatOf('.'))
                throw Guard.FilePathNewNameRequired();

            var newExtension = newName.SubstringAfterLast('.', out newName, true);
            return new FilePath(_driveOrHost, _rootFolder, ParentPath, newName, newExtension, _isDirectory);
        }

        /// <summary>
        ///     Заменяет имя без раширения, если это путь файла или полностью если это путь директории.
        ///     Например: <code>{f.e}.WithBasename("n.n") => {n.n}</code>
        /// </summary>
        public FilePath WithBasename(string newName)
        {
            if (string.IsNullOrEmpty(newName) || newName.IsRepeatOf('.'))
                throw Guard.FilePathNewNameRequired();

            var newExtension = newName.SubstringAfterLast('.', out newName, true);
            if (!IsDirectory)
            {
                newName += newExtension;
                newExtension = Extension;
            }

            return new FilePath(_driveOrHost, _rootFolder, ParentPath, newName, newExtension, _isDirectory);
        }

        /// <summary>
        ///     Добавляет приставку.
        ///     Например: <code>{f.e}.WithNamePrefix("p") => {pf.e}</code>
        /// </summary>
        public FilePath WithNamePrefix(string prefix)
        {
            return WithName(prefix + Name);
        }

        /// <summary>
        ///     Добавляет окончание.
        ///     Например: <code>{f.e}.WithNameSuffix("s") => {f.es}</code>
        /// </summary>
        public FilePath WithNameSuffix(string suffix)
        {
            return WithName(Name + suffix);
        }

        public FilePath WithoutDriveOrHost()
        {
            return new FilePath("", _rootFolder, ParentPath, NameWithoutExtension, Extension, _isDirectory);
        }

        public FilePath WithoutDriveOrHostAndExtension()
        {
            var newExt = NameWithoutExtension.SubstringAfterLast('.', out var newName, true);
            return new FilePath("", _rootFolder, ParentPath, newName, newExt, false);
        }

        public string WithoutDriveOrHostAndExtensionAsString()
        {
            return _rootFolder + ParentPath + NameWithoutExtension;
        }

        [CanBeNull]
        public FilePath WithoutDriveOrHostAndFileName()
        {
            return _isDirectory ? WithoutDriveOrHost() : Parent?.WithoutDriveOrHost();
        }

        public string WithoutDriveOrHostAndFileNameAsString()
        {
            return _isDirectory
                ? _rootFolder + ParentPath + NameWithoutExtension + Extension + Separator
                : _rootFolder + ParentPath;
        }

        [CanBeNull]
        public FilePath WithoutDriveOrHostAndName()
        {
            return Parent?.WithoutDriveOrHost();
        }

        public string WithoutDriveOrHostAndNameAsString()
        {
            return _rootFolder + ParentPath;
        }

        public string WithoutDriveOrHostAsString()
        {
            return _rootFolder + ParentPath + NameWithoutExtension + Extension + (_isDirectory ? Separator : "");
        }

        public FilePath WithoutExtension()
        {
            var newExt = NameWithoutExtension.SubstringAfterLast('.', out var newName, true);
            return new FilePath(_driveOrHost, _rootFolder, ParentPath, newName, newExt, false);
        }

        public string WithoutExtensionAsString()
        {
            return _driveOrHost + _rootFolder + ParentPath + NameWithoutExtension;
        }

        public FilePath WithoutExtensions(int count = 1)
        {
            var parts = NameWithoutExtension.Split('.');
            if (parts.Length < count)
                throw new ArgumentOutOfRangeException();

            if (parts.Length == count)
                return new FilePath(_driveOrHost, _rootFolder, ParentPath, parts[0], "", _isDirectory);

            var ext = "." + parts[parts.Length - 1];
            parts = parts.Shrink(count);
            var newName = string.Join(".", parts);
            return new FilePath(_driveOrHost, _rootFolder, ParentPath, newName, ext, _isDirectory);
        }

        [CanBeNull]
        public FilePath WithoutFileName()
        {
            return _isDirectory ? this : Parent;
        }

        public string WithoutFileNameAsString()
        {
            return _isDirectory ? _path : _driveOrHost + _rootFolder + ParentPath;
        }

        [CanBeNull]
        public FilePath WithoutName()
        {
            return Parent;
        }

        public string WithoutNameAsString()
        {
            return _driveOrHost + _rootFolder + ParentPath;
        }

        public FilePath WithoutRootFolder()
        {
            return new FilePath("", "", ParentPath, NameWithoutExtension, Extension, _isDirectory);
        }

        public FilePath WithoutRootFolderAndExtension()
        {
            return new FilePath("", "", ParentPath, NameWithoutExtension, "", false);
        }

        public string WithoutRootFolderAndExtensionAsString()
        {
            return ParentPath + NameWithoutExtension;
        }

        [CanBeNull]
        public FilePath WithoutRootFolderAndFileName()
        {
            return _isDirectory ? WithoutRootFolder() : Parent?.WithoutRootFolder();
        }

        public string WithoutRootFolderAndFileNameAsString()
        {
            return _isDirectory ? ParentPath + NameWithoutExtension + Extension + Separator : ParentPath;
        }

        [CanBeNull]
        public FilePath WithoutRootFolderAndName()
        {
            return Parent?.WithoutRootFolder();
        }

        public string WithoutRootFolderAndNameAsString()
        {
            return ParentPath;
        }

        public string WithoutRootFolderAsString()
        {
            return ParentPath + NameWithoutExtension + Extension + (_isDirectory ? Separator : "");
        }

        public FilePath WithRoot(string newRoot)
        {
            var newRootFolder = _rootFolder;
            if (!string.IsNullOrEmpty(newRoot))
            {
                var root = newRoot.Trim().Replace('/', '\\');

                if (root.StartsWith(@"\\?\UNC\"))
                    root = root.Substring(7);
                else if (root.StartsWith(@"\\?\"))
                    root = root.Substring(4);

                if (root.Length == 1 || root.EndsWith(':'))
                {
                    if (!IsDriveLetter(root[0]))
                        throw new ArgumentOutOfRangeException(nameof(newRoot), $"Invalid drive letter '{newRoot}'.");

                    if (root.Length == 1)
                        root += ":";
                }
                else if (root.Length == 3 && root[1] == ':' && root[2] == '\\')
                {
                    if (!IsDriveLetter(root[0]))
                        throw new ArgumentOutOfRangeException(nameof(newRoot), $"Invalid drive letter '{newRoot}'.");

                    newRootFolder = Separator;
                    root = root[0] + ":";
                }
                else if (root.StartsWith('\\'))
                {
                    root = root.TrimStart('\\');
                    var share = root.SubstringAfter('\\');
                    if (share == "" || share.JoinsWith('\\'))
                        throw new ArgumentException($"Invalid network share root '{newRoot}'.");

                    root = root.RemoveRight(share.Length + 1);
                    newRootFolder = '\\' + share.EnsureEndsWith(Sep);

                    root = Network + root;
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

            return new FilePath(newRoot, newRootFolder, ParentPath, NameWithoutExtension, Extension, _isDirectory);
        }

        private FilePath Item(string name, bool directory)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(nameof(name));

            var ext = "";
            var pos = name.LastIndexOf('.');
            if (0 == pos)
            {
                ext = name;
                name = "";
            }
            else if (0 < pos)
            {
                ext = name.Substring(pos);
                name = name.Substring(0, pos);
            }

            var result = new FilePath(_driveOrHost, _rootFolder, DirectoryPath, name, ext, directory);
            return result;
        }

        private string NameWithoutExtensionsCore(string ext)
        {
            if (ext[0] != '.')
                ext = '.' + ext;

            if (ext.EndsWithIgnoreCase(Extension))
            {
                var subExt = ext.RemoveRight(Extension.Length);
                if (NameWithoutExtension.EndsWithIgnoreCase(subExt))
                    return NameWithoutExtension.RemoveRight(subExt.Length);
            }

            return null;
        }

        private bool TryToRelativeCore(string basePath, string baseRoot, string baseLocalPath,
            out FilePath relativePath)
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

        private bool TryToRelativeStringCore(string basePath, string baseRoot, string baseLocalPath,
            out string relativePath)
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
    }

    public enum FilePathRelation
    {
        None,

        /// <summary>
        ///     Равенство
        /// </summary>
        Equal,

        /// <summary>
        ///     Вложенность
        /// </summary>
        Child,

        /// <summary>
        ///     Включение
        /// </summary>
        Parent,

        /// <summary>
        ///     Неявная вложенность
        /// </summary>
        ImplicitChild,

        /// <summary>
        ///     Неявное включение
        /// </summary>
        ImplicitParent,

        /// <summary>
        ///     Общая корневая папка
        /// </summary>
        Sibling,

        /// <summary>
        ///     Общая папка
        /// </summary>
        CommonAncestor
    }
}