using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Exader.IO
{
    [Flags]
    [Obsolete("TODO #1 Parse a string representation of a file path in a specified style")]
    internal enum FilePathStyles
    {
        None,
        AllowDrive,
        AllowNetwork,
        AllowUnc,
        AllowFileUri,
        AllowDoubleDots,
        KeepDoubleDots
    }

    public enum FilePathParseErrorType
    {
        None,
        InvalidDriveLetter,
        InvalidCharacter,
        InvalidLongPathPrefix,
        NullValue
    }

    public partial class FilePath
    {
        private const char WinSep = '\\';
        private const string WinSeparator = "\\";
        private const char UnixSep = '/';
        private const string UnixSeparator = "/";

        /// <summary>
        /// Directory separator character (platform specific)
        /// </summary>
        private static char Sep;

        /// <summary>
        /// Alternate directory separator character (platform specific)
        /// </summary>
        private static char AltSep;

        /// <summary>
        /// Directory separator string (platform specific)
        /// </summary>
        private static string Separator;

        /// <summary>
        /// Alternate directory separator string (platform specific)
        /// </summary>
        private static string AltSeparator;

        private static string Unc;
        private static string LongPath;
        private static string Network;
        private static string Localhost;
        private static string RelativeParent;

        internal static string CurDir;
        internal static string DirSfx;
        internal static string SchSfx;

        private static bool _isWindows;

        public const char HorizontalEllipsisFillerChar = '…';

        public const char TildeFillerChar = '~';
        public const char UnderscoreFillerChar = '_';
        private const char VolumeSeparatorChar = ':';

        static FilePath()
        {
            IsWindows = System.IO.Path.DirectorySeparatorChar == WinSep;
        }

        public static bool IsWindows
        {
            get => _isWindows;
            set
            {
                _isWindows = value;

                Sep = value ? WinSep : UnixSep;
                AltSep = value ? UnixSep : WinSep;
                Separator = Sep.ToString();
                AltSeparator = AltSep.ToString();

                CurDir = "." + Separator;
                DirSfx = Separator + "." + Separator;
                SchSfx = ":" + Separator + Separator;

                Network = Separator + Separator;
                Localhost = Network + "localhost";
                LongPath = Network + "?" + Separator; // \\?\
                Unc = LongPath + "UNC" + Separator; // \\?\UNC\

                RelativeParent = ".." + Separator;
                RelativeRoot = new FilePath("", Separator, "", "", "", false);
            }
        }

        public static FilePath Parse(string value)
        {
            return Parse(value, FilePathStyles.None);
        }

        public static bool TryParse(string value, out FilePath result)
        {
            return TryParse(value, FilePathStyles.None, out result, out var error);
        }

        public static bool TryParse(string value, out FilePath result, out FilePathError error)
        {
            return TryParse(value, FilePathStyles.None, out result, out error);
        }

        internal static FilePath Parse(string value, FilePathStyles styles)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value == string.Empty)
            {
                return Empty;
            }

            var parser = new Parser(value, false, "", styles);
            switch (parser.ErrorType)
            {
                case FilePathParseErrorType.None:
                    break;
                case FilePathParseErrorType.InvalidCharacter:
                    throw new ArgumentException($"Invalid file path character '{parser.ErrorValue}'.");
                case FilePathParseErrorType.InvalidDriveLetter:
                    throw new ArgumentException($"Invalid drive letter '{parser.ErrorValue}'.");
                case FilePathParseErrorType.InvalidLongPathPrefix:
                    throw new ArgumentException($"Invalid long path prefix '{parser.ErrorValue}'.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new FilePath(parser.DriveOrHost, parser.RootFolder, parser.Prefix, parser.Name, parser.Extension,
                parser.IsDirectory);
        }

        internal static bool TryParse(string value, FilePathStyles styles, out FilePath result, out FilePathError error)
        {
            if (value == string.Empty)
            {
                result = Empty;
                error = new FilePathError(value, FilePathParseErrorType.None, 0, 0);
                return true;
            }

            if (value == null)
            {
                error = new FilePathError(value, FilePathParseErrorType.NullValue, 0, 0);
            }
            else
            {
                var parser = new Parser(value, false, "", styles);
                if (parser.ErrorType == FilePathParseErrorType.None)
                {
                    result = new FilePath(parser.DriveOrHost, parser.RootFolder, parser.Prefix, parser.Name,
                        parser.Extension, parser.IsDirectory);
                    error = new FilePathError(value, FilePathParseErrorType.None, 0, 0);
                    return true;
                }

                error = new FilePathError(value, parser.ErrorType, parser.ErrorStart, parser.ErrorLength);
            }

            result = null;
            return false;
        }

        private class Parser
        {
            private readonly string _originalString;

            public Parser(string value, bool rel, string rootFolder2, FilePathStyles styles = FilePathStyles.None)
            {
                _originalString = value;
                RootFolder = rootFolder2;

                var sep = int.MinValue;
                var isUri = false;
                var separators = new Stack<int>();

                var p = '\0';
                var length = TrimLength(value);
                var buffer = new StringBuilder(length);
                for (var i = 0; i < length; i++)
                {
                    var c = value[i];
                    switch (c)
                    {
                        case '\\':
                        case '/':
                            c = Sep;
                            switch (p)
                            {
                                case '\\':
                                case '/':
                                    // \\
                                    if (DriveOrHost == "")
                                    {
                                        // \\server
                                        //  ^
                                        DriveOrHost = Network;
                                        buffer.Append(Separator);
                                    }
                                    break;
                                case '.':
                                    if (buffer.Length > 0 && buffer[0] == Sep)
                                    {
                                        // \.\
                                        //   ^
                                        RootFolder = Separator;
                                        buffer.Remove(0, 1);
                                    }

                                    sep = TryTerminateRepeatOfDots(buffer, sep, separators, true, i == length - 1);
                                    break;
                                case '\0':
                                    // \
                                    // ^
                                    buffer.Append(Separator);
                                    break;
                                default:
                                    if (DriveOrHost == "" && isUri)
                                    {
                                        // file:\server
                                        //      ^
                                        DriveOrHost = Network;
                                        buffer.Append(Separator);
                                        if (buffer.Length == 1)
                                        {
                                            buffer.Append(Separator);
                                        }
                                    }
                                    else if (DriveOrHost == Network)
                                    {
                                        if (buffer.Length == 3 && p == '?')
                                        {
                                            // \\?\
                                            //    ^
                                            buffer.Clear();
                                            var j = i + 1;
                                            for (; j < length; j++)
                                            {
                                                var s = value[j];
                                                if (s == '\\' || s == '/')
                                                {
                                                    break;
                                                }
                                            }

                                            var ss = value.Substring(i + 1, j - i - 1);
                                            if (ss.Length == 2 && ss[ss.Length - 1] == ':')
                                            {
                                                // \\?\C:\
                                                //    ^
                                                i += 3;
                                                DriveOrHost = ss;
                                                buffer.Append(Separator);
                                                p = value[j];
                                            }
                                            else if ("UNC".EqualsIgnoreCase(ss))
                                            {
                                                // \\?\UNC\
                                                //    ^
                                                i += 4;
                                                buffer.Append(Network);
                                                p = value[j];
                                            }
                                            else
                                            {
                                                ErrorType = FilePathParseErrorType.InvalidLongPathPrefix;
                                                ErrorStart = i - 3;
                                                ErrorLength = j - i + 3;
                                                return;
                                            }

                                            continue;
                                        }

                                        // \\server\
                                        //         ^
                                        DriveOrHost = buffer.ToString();
                                        buffer.Clear();
                                        buffer.Append(Separator);
                                    }
                                    else if (buffer.Length > 0 && buffer[0] == Sep)
                                    {
                                        if (DriveOrHost.Length > 2)
                                        {
                                            // \\server\folder\
                                            //                ^
                                            buffer.Append(Separator);
                                            RootFolder = buffer.ToString();
                                            buffer.Clear();
                                        }
                                        else
                                        {
                                            // \dir\
                                            //     ^
                                            RootFolder = Separator;

                                            TrimEnd(buffer);

                                            buffer.Remove(0, 1).Append(Separator);
                                            sep = buffer.Length - 1;
                                        }
                                    }
                                    else
                                    {
                                        TrimEnd(buffer);

                                        // dir\
                                        //    ^
                                        buffer.Append(Separator);
                                        if (buffer.Length > 1)
                                        {
                                            if (0 < sep)
                                            {
                                                separators.Push(sep);
                                            }

                                            sep = buffer.Length - 1;
                                        }
                                    }

                                    break;
                            }

                            break;

                        case ':':
                            if (isUri && (Network.EqualsIgnoreCase(DriveOrHost) ||
                                          Localhost.EqualsIgnoreCase(DriveOrHost)))
                            {
                                if (DriveOrHost == Network)
                                {
                                    // file://C:
                                    //         ^
                                    buffer.Remove(0, 2); // \\C -> C
                                }
                                else
                                {
                                    // file://localhost/C:
                                    //                   ^
                                    buffer.Remove(0, 1); // \C -> C
                                }

                                if (buffer.Length > 1)
                                {
                                    ErrorType = FilePathParseErrorType.InvalidDriveLetter;
                                    ErrorStart = i - buffer.Length;
                                    ErrorLength = buffer.Length;
                                    return;
                                }

                                buffer.Append(':'); // C -> C:
                                DriveOrHost = buffer.ToString();
                                buffer.Clear();
                            }
                            else if (DriveOrHost == "")
                            {
                                if (buffer.Length == 1)
                                {
                                    // C:
                                    //  ^
                                    buffer.Append(':');
                                    DriveOrHost = buffer.ToString();
                                    buffer.Clear();
                                }
                                else
                                {
                                    // file:
                                    //     ^
                                    var schema = buffer.ToString();
                                    if ("file".EqualsIgnoreCase(schema))
                                    {
                                        isUri = true;
                                        buffer.Clear();
                                    }
                                    else
                                    {
                                        ErrorType = FilePathParseErrorType.InvalidDriveLetter;
                                        ErrorStart = i - buffer.Length;
                                        ErrorLength = buffer.Length;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                buffer.Append(':');
                            }

                            break;

                        case '|':
                            if (isUri && (Network.EqualsIgnoreCase(DriveOrHost) ||
                                          Localhost.EqualsIgnoreCase(DriveOrHost)))
                            {
                                if (DriveOrHost == Network)
                                {
                                    // file://C|
                                    //         ^
                                    buffer.Remove(0, 2); // \\C -> C
                                }
                                else
                                {
                                    // file://localhost/C|
                                    //                   ^
                                    buffer.Remove(0, 1); // \C -> C
                                }

                                if (buffer.Length > 1)
                                {
                                    ErrorType = FilePathParseErrorType.InvalidDriveLetter;
                                    ErrorStart = i - buffer.Length;
                                    ErrorLength = buffer.Length;
                                    return;
                                }

                                buffer.Append(':'); // C -> C:
                                DriveOrHost = buffer.ToString();
                                buffer.Clear();
                            }
                            else
                            {
                                buffer.Append('|');
                            }

                            break;

                        default:
                            if (IsInvalidCharacter(c))
                            {
                                ErrorType = FilePathParseErrorType.InvalidCharacter;
                                ErrorStart = i;
                                ErrorLength = 1;
                                return;
                            }

                            if (c == '%')
                            {
                                char hex;
                                if (i + 2 < length && TryEncodeHex(value[i + 1], value[i + 2], out hex))
                                {
                                    i += 2;
                                    if (IsInvalidCharacter(hex))
                                    {
                                        ErrorType = FilePathParseErrorType.InvalidCharacter;
                                        ErrorStart = i;
                                        ErrorLength = 3;
                                        return;
                                    }

                                    c = hex;
                                }
                            }

                            buffer.Append(c);
                            break;
                    }

                    p = c;
                }

                if (DriveOrHost == Network)
                {
                    // \\server
                    //         ^
                    DriveOrHost = buffer.ToString();
                    return;
                }

                if (buffer.Length == 1 && buffer[0] == Sep)
                {
                    RootFolder = Separator;
                    return;
                }

                if (buffer.Length > 0 && buffer[0] == Sep)
                {
                    RootFolder = Separator;
                    buffer.Remove(0, 1);
                }

                if (p == '.')
                {
                    sep = TryTerminateRepeatOfDots(buffer, sep, separators, false, false);
                }

                if (sep == buffer.Length - 1)
                {
                    // dir/
                    IsDirectory = true;
                    buffer.Remove(buffer.Length - 1, 1);

                    sep = separators.Count > 0 ? separators.Pop() : 0;
                }

                var str = buffer.ToString();
                if (sep > 0)
                {
                    Prefix = str.Substring(0, sep + 1);
                    Name = str.Substring(sep + 1);
                }
                else
                {
                    Name = str;
                }

                if (Name == ".." || Name == "")
                {
                    IsDirectory = true;
                }
                else if (Name == ".")
                {
                    IsDirectory = true;
                    Name = "";
                }
                else
                {
                    string name;
                    Extension = Name.SubstringAfterLast('.', out name, true);
                    Name = name;

                    if (Extension == ".")
                    {
                        Extension = "";
                    }
                }
            }

            public string DirectoryPath => IsDirectory ? Prefix + Name + Extension + Separator : Prefix;

            public string DriveOrHost { get; } = string.Empty;

            public int ErrorLength { get; }

            public int ErrorStart { get; }

            public FilePathParseErrorType ErrorType { get; } = FilePathParseErrorType.None;

            public string ErrorValue => _originalString.Substring(ErrorStart, ErrorLength);
            public string Extension { get; } = string.Empty;

            public bool IsDirectory { get; }

            public string Name { get; } = string.Empty;

            public string Prefix { get; } = string.Empty;

            public string RootFolder { get; } = string.Empty;

            internal static bool TryEncodeHex(char first, char second, out char result)
            {
                result = char.MinValue;
                if (!(first >= '0' && first <= '9' || first >= 'A' && first <= 'F' || first >= 'a' && first <= 'f'))
                {
                    return false;
                }

                if (!(second >= '0' && second <= '9' || second >= 'A' && second <= 'F' ||
                      second >= 'a' && second <= 'f'))
                {
                    return false;
                }

                int value;
                if (first <= '9')
                {
                    value = first - '0';
                }
                else
                {
                    if (first <= 'F')
                    {
                        value = first - 'A';
                    }
                    else
                    {
                        value = first - 'a';
                    }

                    value += 10;
                }

                value <<= 4;

                if (second <= '9')
                {
                    value += second - '0';
                }
                else
                {
                    if (second <= 'F')
                    {
                        value += second - 'A';
                    }
                    else
                    {
                        value += second - 'a';
                    }

                    value += 10;
                }

                result = (char) value;
                return true;
            }

            private static bool IsInvalidCharacter(char c)
            {
                switch (c)
                {
                    case '"':
                    case '<':
                    case '>':
                    case '|':
                    case char.MinValue:
                    case '\x0001':
                    case '\x0002':
                    case '\x0003':
                    case '\x0004':
                    case '\x0005':
                    case '\x0006':
                    case '\a':
                    case '\b':
                    case '\t':
                    case '\n':
                    case '\v':
                    case '\f':
                    case '\r':
                    case '\x000E':
                    case '\x000F':
                    case '\x0010':
                    case '\x0011':
                    case '\x0012':
                    case '\x0013':
                    case '\x0014':
                    case '\x0015':
                    case '\x0016':
                    case '\x0017':
                    case '\x0018':
                    case '\x0019':
                    case '\x001A':
                    case '\x001B':
                    case '\x001C':
                    case '\x001D':
                    case '\x001E':
                    case '\x001F':
                        return true;
                }

                return false;
            }

            public override string ToString()
            {
                return _originalString;
            }

            [Conditional("DEBUG")]
            private void Check(Stack<int> separators, int offset)
            {
                if (separators.Contains(offset))
                {
                    Debugger.Break();
                    Debug.Fail("Item already exists");
                }
            }

            [Obsolete("TODO Trim name only")]
            private void TrimEnd(StringBuilder buffer)
            {
                if (buffer.Length == 1 && buffer[0] == '.')
                {
                    return;
                }

                if (buffer.Length == 2 && buffer[0] == '.' && buffer[1] == '.')
                {
                    return;
                }

                if (buffer.Length > 2 && buffer[buffer.Length - 3] == Sep && buffer[buffer.Length - 2] == '.' &&
                    buffer[buffer.Length - 1] == '.')
                {
                    return;
                }

                var len = buffer.Length;
                for (var i = buffer.Length - 1; 0 <= i; i--)
                    switch (buffer[i])
                    {
                        case '.':
                        case '\t':
                        case '\n':
                        case '\v':
                        case '\f':
                        case (char) 0x20: // space
                        case (char) 0x85:
                        case (char) 0xA0: // nbsp
                            len--;
                            break;
                        default:
                            i = 0;
                            break;
                    }

                if (len < buffer.Length)
                {
                    buffer.Remove(len, buffer.Length - len);
                }
            }

            private int TrimLength(string value)
            {
                var len = value.Length;
                for (var i = len - 1; 0 <= i; i--)
                    switch (value[i])
                    {
                        //case '.':
                        case '\t':
                        case '\n':
                        case '\v':
                        case '\f':
                        case (char) 0x20: // space
                        case (char) 0x85:
                        case (char) 0xA0: // nbsp
                            len--;
                            break;
                        default:
                            i = 0;
                            break;
                    }

                return len;
            }

            private int TryTerminateRepeatOfDots(StringBuilder buffer, int sep, Stack<int> separators,
                bool appendSeparator, bool isLast)
            {
                if (buffer.Length == 1 || 0 < sep && buffer.Length - sep == 2)
                {
                    // ./
                    //  ^

                    buffer.Remove(buffer.Length - 1, 1);
                }
                else if (buffer.Length > 1)
                {
                    if (sep > 0)
                    {
                        if (buffer.IsRepeatOf('.', sep + 1))
                        {
                            if (separators.Count > 0)
                            {
                                // d/sd/../ -> d/
                                //        ^
                                sep = separators.Pop();
                                buffer.Remove(sep + 1, buffer.Length - sep - 1);
                            }
                            else if (buffer[sep - 1] != '.')
                            {
                                // d/.. ->
                                //        ^
                                buffer.Clear();
                                sep = int.MinValue;
                            }
                            else if (appendSeparator)
                            {
                                buffer.Append(Separator);
                                if (0 < sep && isLast)
                                {
                                    separators.Push(sep);
                                }

                                sep = buffer.Length - 1;
                            }
                        }
                        else
                        {
                            // d\sd.\
                            //      ^

                            TrimEnd(buffer);

                            if (appendSeparator)
                            {
                                buffer.Append(Separator);
                                if (0 < sep)
                                {
                                    separators.Push(sep);
                                }

                                sep = buffer.Length - 1;
                            }
                        }
                    }
                    else if (buffer[0] == '.' && RootFolder != "")
                    {
                        // ../ ->
                        //   ^
                        buffer.Clear();
                    }
                    else
                    {
                        if (buffer.IsRepeatOf('.'))
                        {
                            // ..\
                            //   ^

                            buffer.Clear().Append("..");
                        }
                        else
                        {
                            // d.\
                            //   ^

                            TrimEnd(buffer);
                        }

                        if (appendSeparator)
                        {
                            buffer.Append(Separator);
                            sep = buffer.Length - 1;
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                return sep;
            }
        }
    }

    public struct FilePathError
    {
        public FilePathError(string originalValue, FilePathParseErrorType errorType, int start, int length)
        {
            OriginalValue = originalValue;
            ErrorType = errorType;
            Start = start;
            Length = length;
        }

        public FilePathParseErrorType ErrorType { get; }
        public string ErrorValue => OriginalValue.Substring(Start, Length);
        public string OriginalValue { get; }
        public int Start { get; }
        public int Length { get; }
    }
}
