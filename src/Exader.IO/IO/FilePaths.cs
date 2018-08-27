using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Exader.IO
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public static class FilePaths
    {
        /// <remarks>
        /// Возвращает путь текущей директории.
        /// </remarks>
        public static FilePath CurrentDirectory => SafeCreateFilePath(Path.GetFullPath(".\\"));

        private static FilePath SafeCreateFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return FilePath.Parse(path);
        }

        /// <summary>
        /// Возвращает путь к переносимому корневому каталогу пользовательских данных приложений (%APPDATA% или ~/.config/).
        /// </summary>
        public static FilePath ApplicationData
        {
            get
            {
#if NET45
                return SafeCreateFilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return SafeCreateFilePath(Environment.GetEnvironmentVariable("APPDATA"));
                }
                else
                {
                    return UserProfile / ".config/";
                }
#endif
            }
        }

        /// <summary>
        /// Возвращает путь к локальному корневому каталогу пользовательских данных приложений (%LOCALAPPDATA% или ~/.local/share/).
        /// </summary>
        public static FilePath LocalApplicationData
        {
            get
            {
#if NET45
                return SafeCreateFilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return SafeCreateFilePath(Environment.GetEnvironmentVariable("LOCALAPPDATA"));
                }
                else
                {
                    return UserProfile / ".local/share/";
                }
#endif
            }
        }

        /// <summary>
        /// Возвращает путь к каталогу общих данных приложений (%ProgramData% или /usr/share/).
        /// </summary>
        public static FilePath CommonApplicationData
        {
            get
            {
#if NET45
                return SafeCreateFilePath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return SafeCreateFilePath(Environment.GetEnvironmentVariable("ProgramData"));
                }
                else
                {
                    return FilePath.Parse("/usr/share/");
                }
#endif
            }
        }

        /// <summary>
        /// Возвращает путь к каталогу рабочего стола (%USERPROFILE%\Desktop или ~/Desktop/).
        /// </summary>
        public static FilePath Desktop
        {
            get
            {
#if NET45
                return SafeCreateFilePath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
#else
                return UserProfile / "Desktop/";
#endif
            }
        }

        public static FilePath UserProfile
        {
            get
            {
#if NET45
                return SafeCreateFilePath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return SafeCreateFilePath(Environment.GetEnvironmentVariable("USERPROFILE"));
                }
                else
                {
                    return SafeCreateFilePath(Environment.GetEnvironmentVariable("HOME"));
                }
#endif
            }
        }

        /// <summary>
        /// Возвращает путь к каталогу с исполняемым кодом.
        /// </summary>
        [CanBeNull]
        public static FilePath ExecutingAssemblyDirectory
        {
            get
            {
#if NET45
                return GetDirectoryPath(Assembly.GetExecutingAssembly());
#else
                return SafeCreateFilePath(AppContext.BaseDirectory);
#endif
            }
        }

#if NETSTANDARD1_5 || NET45

        [CanBeNull]
        public static FilePath GetPath(this Assembly assembly)
        {
            if (assembly != null)
            {
                var cb = assembly.CodeBase;
                if (!string.IsNullOrEmpty(cb))
                {
                    return SafeCreateFilePath(Uri.UnescapeDataString(new Uri(cb).AbsolutePath));
                }
            }

            return null;
        }

        [CanBeNull]
        public static FilePath GetDirectoryPath(this Assembly assembly)
        {
            var file = GetPath(assembly);
            return file?.Parent;
        }

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Assembly.GetEntryAssembly"/>.
        /// </remarks>
        [CanBeNull]
        public static FilePath EntryAssemblyDirectory => GetDirectoryPath(Assembly.GetEntryAssembly());

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Assembly.GetEntryAssembly"/>.
        /// </remarks>
        [CanBeNull]
        public static FilePath EntryAssemblyFile => GetPath(Assembly.GetEntryAssembly());

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Assembly.GetExecutingAssembly"/>.
        /// </remarks>
        [CanBeNull]
        public static FilePath ExecutingAssemblyFile => GetPath(Assembly.GetExecutingAssembly());

        public static FilePath GetPath(this Environment.SpecialFolder folder)
        {
            return SafeCreateFilePath(Environment.GetFolderPath(folder));
        }

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Assembly.GetCallingAssembly"/>.
        /// </remarks>
        [CanBeNull]
        public static FilePath CallingAssemblyDirectory => GetDirectoryPath(Assembly.GetCallingAssembly());

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Assembly.GetCallingAssembly"/>.
        /// </remarks>
        [CanBeNull]
        public static FilePath CallingAssemblyFile => GetPath(Assembly.GetCallingAssembly());

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.CommonProgramFiles"/>.
        /// </remarks>
        public static FilePath CommonProgramFiles => GetPath(Environment.SpecialFolder.CommonProgramFiles);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Cookies"/>.
        /// </remarks>
        public static FilePath Cookies => GetPath(Environment.SpecialFolder.Cookies);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Favorites"/>.
        /// </remarks>
        public static FilePath Favorites => GetPath(Environment.SpecialFolder.Favorites);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.History"/>.
        /// </remarks>
        public static FilePath History => GetPath(Environment.SpecialFolder.History);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.InternetCache"/>.
        /// </remarks>
        public static FilePath InternetCache => GetPath(Environment.SpecialFolder.InternetCache);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.MyDocuments"/>.
        /// </remarks>
        public static FilePath MyDocuments => GetPath(Environment.SpecialFolder.MyDocuments);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.MyMusic"/>.
        /// </remarks>
        public static FilePath MyMusic => GetPath(Environment.SpecialFolder.MyMusic);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.MyPictures"/>.
        /// </remarks>
        public static FilePath MyPictures => GetPath(Environment.SpecialFolder.MyPictures);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.ProgramFiles"/>.
        /// </remarks>
        public static FilePath ProgramFiles => GetPath(Environment.SpecialFolder.ProgramFiles);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Programs"/>.
        /// </remarks>
        public static FilePath Programs => GetPath(Environment.SpecialFolder.Programs);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Recent"/>.
        /// </remarks>
        public static FilePath Recent => GetPath(Environment.SpecialFolder.Recent);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.SendTo"/>.
        /// </remarks>
        public static FilePath SendTo => GetPath(Environment.SpecialFolder.SendTo);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.StartMenu"/>.
        /// </remarks>
        public static FilePath StartMenu => GetPath(Environment.SpecialFolder.StartMenu);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Startup"/>.
        /// </remarks>
        public static FilePath Startup => GetPath(Environment.SpecialFolder.Startup);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SystemDirectory"/>.
        /// </remarks>
        public static FilePath System => SafeCreateFilePath(Environment.SystemDirectory);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Templates"/>.
        /// </remarks>
        public static FilePath Templates => GetPath(Environment.SpecialFolder.Templates);
#endif
    }
}
