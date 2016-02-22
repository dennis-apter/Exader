#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        KeepDoubleDots,
    }

    public enum FilePathParseErrorType
    {
        None,
        InvalidDriveLetter,
        InvalidCharacter,
        InvalidLongPathPrefix,
    }

    public partial class FilePath
    {
        public static bool TryParse(string value, out FilePath result) => TryParse(value, out result, FilePathStyles.None);
        internal static bool TryParse(string value, out FilePath result, FilePathStyles styles)
        {
            var parser = new Parser(value, false, "", styles);
            if (parser.ErrorType == FilePathParseErrorType.None)
            {
                result = new FilePath(parser.DriveOrHost, parser.RootFolder, parser.Prefix, parser.Name, parser.Extension, parser.IsDirectory);
                return true;
            }

            result = null;
            return false;
        }

        public static FilePath Parse(string value) => Parse(value, FilePathStyles.None);
        internal static FilePath Parse(string value, FilePathStyles styles)
        {
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new FilePath(parser.DriveOrHost, parser.RootFolder, parser.Prefix, parser.Name, parser.Extension, parser.IsDirectory);
        }

        private class Parser
        {
            private readonly string _originalString;
            private readonly string _driveOrHost = string.Empty;
            private readonly string _rootFolder = string.Empty;
            private readonly string _prefix = string.Empty;
            private readonly string _name = string.Empty;
            private readonly string _extension = string.Empty;
            private readonly bool _isDirectory;
            private readonly int _errorStart;
            private readonly int _errorLength;
            private readonly FilePathParseErrorType _error = FilePathParseErrorType.None;

            public Parser(string value, bool rel, string rootFolder2, FilePathStyles styles = FilePathStyles.None)
            {
                _originalString = value;
                _rootFolder = rootFolder2;

                int sep = int.MinValue;
                bool isUri = false;
                var separators = new Stack<int>();

                var p = '\0';
                var length = TrimLength(value);
                var buffer = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    var c = value[i];
                    switch (c)
                    {
                        case '\\':
                        case '/':
                            c = '\\';
                            switch (p)
                            {
                                case '\\':
                                    // \\
                                    if (_driveOrHost == "")
                                    {
                                        // \\server
                                        //  ^
                                        _driveOrHost = @"\\";
                                        buffer.Append('\\');
                                    }
                                    else
                                    {
                                        //  ignore repeats
                                    }
                                    break;
                                case '.':
                                    if (buffer.Length > 0 && buffer[0] == '\\')
                                    {
                                        // \.\
                                        //   ^
                                        _rootFolder = @"\";
                                        buffer.Remove(0, 1);
                                    }

                                    sep = TryTerminateRepeatOfDots(buffer, sep, separators, true, i == length - 1);
                                    break;
                                case '\0':
                                    // \
                                    // ^
                                    buffer.Append('\\');
                                    break;
                                default:
                                    if (_driveOrHost == "" && isUri)
                                    {
                                        // file:\server
                                        //      ^
                                        _driveOrHost = @"\\";
                                        buffer.Append('\\');
                                        if (buffer.Length == 1)
                                            buffer.Append('\\');
                                    }
                                    else if (_driveOrHost == @"\\")
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
                                                    break;
                                            }

                                            var ss = value.Substring(i + 1, j - i - 1);
                                            if (ss.Length == 2 && ss[ss.Length - 1] == ':')
                                            {
                                                // \\?\C:\
                                                //    ^
                                                i += 3;
                                                _driveOrHost = ss;
                                                buffer.Append('\\');
                                                p = value[j];
                                            }
                                            else if ("UNC".EqualsIgnoreCase(ss))
                                            {
                                                // \\?\UNC\
                                                //    ^
                                                i += 4;
                                                buffer.Append(@"\\");
                                                p = value[j];
                                            }
                                            else
                                            {
                                                _error = FilePathParseErrorType.InvalidLongPathPrefix;
                                                _errorStart = i - 3;
                                                _errorLength = j - i + 3;
                                                return;
                                            }

                                            continue;
                                        }

                                        // \\server\
                                        //         ^
                                        _driveOrHost = buffer.ToString();
                                        buffer.Clear();
                                        buffer.Append('\\');
                                    }
                                    else if (buffer.Length > 0 && buffer[0] == '\\')
                                    {
                                        if (_driveOrHost.Length > 2)
                                        {
                                            // \\server\folder\
                                            //                ^
                                            buffer.Append('\\');
                                            _rootFolder = buffer.ToString();
                                            buffer.Clear();
                                        }
                                        else
                                        {
                                            // \dir\
                                            //     ^
                                            _rootFolder = @"\";

                                            TrimEnd(buffer);

                                            buffer.Remove(0, 1).Append('\\');
                                            sep = buffer.Length - 1;
                                        }
                                    }
                                    else
                                    {
                                        TrimEnd(buffer);

                                        // dir\
                                        //    ^
                                        buffer.Append('\\');
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
                            if (isUri && (@"\\".EqualsIgnoreCase(_driveOrHost) || @"\\localhost".EqualsIgnoreCase(_driveOrHost)))
                            {
                                if (_driveOrHost == @"\\")
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
                                    _error = FilePathParseErrorType.InvalidDriveLetter;
                                    _errorStart = i - buffer.Length;
                                    _errorLength = buffer.Length;
                                    return;
                                }

                                buffer.Append(':'); // C -> C:
                                _driveOrHost = buffer.ToString();
                                buffer.Clear();
                            }
                            else if (_driveOrHost == "")
                            {
                                if (buffer.Length == 1)
                                {
                                    // C:
                                    //  ^
                                    buffer.Append(':');
                                    _driveOrHost = buffer.ToString();
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
                                        _error = FilePathParseErrorType.InvalidDriveLetter;
                                        _errorStart = i - buffer.Length;
                                        _errorLength = buffer.Length;
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
                            if (isUri && (@"\\".EqualsIgnoreCase(_driveOrHost) || @"\\localhost".EqualsIgnoreCase(_driveOrHost)))
                            {
                                if (_driveOrHost == @"\\")
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
                                    _error = FilePathParseErrorType.InvalidDriveLetter;
                                    _errorStart = i - buffer.Length;
                                    _errorLength = buffer.Length;
                                    return;
                                }

                                buffer.Append(':'); // C -> C:
                                _driveOrHost = buffer.ToString();
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
                                continue;
                            }

                            if (c == '%')
                            {
                                char hex;
                                if (i + 2 < length && TryEncodeHex(value[i + 1], value[i + 2], out hex))
                                {
                                    i += 2;
                                    if (IsInvalidCharacter(hex))
                                    {
                                        continue;
                                    }

                                    c = hex;
                                }
                            }

                            buffer.Append(c);
                            break;
                    }

                    p = c;
                }

                if (_driveOrHost == @"\\")
                {
                    // \\server
                    //         ^
                    _driveOrHost = buffer.ToString();
                    return;
                }

                if (buffer.Length == 1 && buffer[0] == '\\')
                {
                    _rootFolder = @"\";
                    return;
                }

                if (buffer.Length > 0 && buffer[0] == '\\')
                {
                    _rootFolder = @"\";
                    buffer.Remove(0, 1);
                }

                if (p == '.')
                {
                    sep = TryTerminateRepeatOfDots(buffer, sep, separators, false, false);
                }

                if (sep == buffer.Length - 1)
                {
                    // dir/
                    _isDirectory = true;
                    buffer.Remove(buffer.Length - 1, 1);

                    sep = separators.Count > 0 ? separators.Pop() : 0;
                }

                string str = buffer.ToString();
                if (sep > 0)
                {
                    _prefix = str.Substring(0, sep + 1);
                    _name = str.Substring(sep + 1);
                }
                else
                {
                    _name = str;
                }

                if (_name == ".." || _name == "")
                {
                    _isDirectory = true;
                }
                else if (_name == ".")
                {
                    _isDirectory = true;
                    _name = "";
                }
                else
                {
                    string name;
                    _extension = _name.SubstringAfterLast('.', out name, true);
                    _name = name;

                    if (_extension == ".")
                    {
                        _extension = "";
                    }
                }
            }

#if !SILVERLIGHT && !NET35
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

            private int TryTerminateRepeatOfDots(StringBuilder buffer, int sep, Stack<int> separators, bool appendSeparator, bool isLast)
            {
                if (buffer.Length == 1 || (0 < sep && buffer.Length - sep == 2))
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
                                buffer.Append('\\');
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
                                buffer.Append('\\');
                                if (0 < sep)
                                {
                                    separators.Push(sep);
                                }

                                sep = buffer.Length - 1;
                            }
                        }
                    }
                    else if (buffer[0] == '.' && _rootFolder != "")
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
                            buffer.Append('\\');
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

            [Conditional("DEBUG")]
            private void Check(Stack<int> separators, int offset)
            {
                if (separators.Contains(offset))
                {
                    Debugger.Break();
                    Debug.Fail("Item already exists");
                }
            }

            public FilePathParseErrorType ErrorType
            {
                get { return _error; }
            }

#if !NET35 && !SILVERLIGHT
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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

            private int TrimLength(string value)
            {
                int len = value.Length;
                for (int i = len - 1; 0 <= i; i--)
                {
                    switch (value[i])
                    {
                        //case '.':
                        case '\t':
                        case '\n':
                        case '\v':
                        case '\f':
                        case (char)0x20: // space
                        case (char)0x85:
                        case (char)0xA0: // nbsp
                            len--;
                            break;
                        default:
                            i = 0;
                            break;
                    }
                }

                return len;
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

                if (buffer.Length > 2 && buffer[buffer.Length - 3] == '\\' && buffer[buffer.Length - 2] == '.' && buffer[buffer.Length - 1] == '.')
                {
                    return;
                }

                int len = buffer.Length;
                for (int i = buffer.Length - 1; 0 <= i; i--)
                {
                    switch (buffer[i])
                    {
                        case '.':
                        case '\t':
                        case '\n':
                        case '\v':
                        case '\f':
                        case (char)0x20: // space
                        case (char)0x85:
                        case (char)0xA0: // nbsp
                            len--;
                            break;
                        default:
                            i = 0;
                            break;
                    }
                }

                if (len < buffer.Length)
                {
                    buffer.Remove(len, buffer.Length - len);
                }
            }

            internal static bool TryEncodeHex(char first, char second, out char result)
            {
                result = char.MinValue;
                if (!(((first >= '0') && (first <= '9')) || ((first >= 'A') && (first <= 'F')) || ((first >= 'a') && (first <= 'f'))))
                {
                    return false;
                }

                if (!(((second >= '0') && (second <= '9')) || ((second >= 'A') && (second <= 'F')) || ((second >= 'a') && (second <= 'f'))))
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

                result = (char)value;
                return true;
            }

            public string DriveOrHost => _driveOrHost;
            public string RootFolder => _rootFolder;
            public string Prefix => _prefix;
            public string Name => _name;
            public string Extension => _extension;
            public string DirectoryPath => _isDirectory ? _prefix + _name + _extension + "\\" : _prefix;

            public bool IsDirectory => _isDirectory;

            public string ErrorValue => _originalString.Substring(_errorStart, _errorLength);

            public override string ToString() => _originalString;
        }
    }
}

#endif
