#if !SILVERLIGHT
using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Exader.IO
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public static class FilePaths
    {
        [CanBeNull]
        public static FilePath GetPath(this Assembly assembly)
        {
            var cb = assembly.CodeBase;
            if (string.IsNullOrEmpty(cb))
                return null;

            return SafeCreateFilePath(Uri.UnescapeDataString(new Uri(cb).AbsolutePath));
        }

        [CanBeNull]
        public static FilePath GetDirectoryPath(this Assembly assembly)
        {
            var file = GetPath(assembly);
            return file?.Parent;
        }
        
        public static FilePath GetPath(this Environment.SpecialFolder folder)
        {
            return SafeCreateFilePath(Environment.GetFolderPath(folder));
        }

        private static FilePath SafeCreateFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return FilePath.Parse(path);
        }

        /// <summary>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.ApplicationData"/>.
        /// </summary>
        public static FilePath ApplicationData => SafeCreateFilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

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

        /// <summary>
        /// Represents the file system directory that serves as a common repository for application-specific data that
        /// is used by all users.
        /// </summary>
        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.CommonApplicationData"/>.
        /// </remarks>
        public static FilePath CommonApplicationData => GetPath(Environment.SpecialFolder.CommonApplicationData);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.CommonProgramFiles"/>.
        /// </remarks>
        public static FilePath CommonProgramFiles => GetPath(Environment.SpecialFolder.CommonProgramFiles);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.Cookies"/>.
        /// </remarks>
        public static FilePath Cookies => GetPath(Environment.SpecialFolder.Cookies);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.CurrentDirectory"/>.
        /// </remarks>
        public static FilePath CurrentDirectory => SafeCreateFilePath(Environment.CurrentDirectory);

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.DesktopDirectory"/>.
        /// </remarks>
        public static FilePath Desktop => GetPath(Environment.SpecialFolder.DesktopDirectory);

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
        public static FilePath ExecutingAssemblyDirectory => GetDirectoryPath(Assembly.GetExecutingAssembly());

        /// <remarks>
        /// Возвращает обертку для пути <see cref="Assembly.GetExecutingAssembly"/>.
        /// </remarks>
        [CanBeNull]
        public static FilePath ExecutingAssemblyFile => GetPath(Assembly.GetExecutingAssembly());

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
        /// Возвращает обертку для пути <see cref="Environment.SpecialFolder.LocalApplicationData"/>.
        /// </remarks>
        public static FilePath LocalApplicationData => GetPath(Environment.SpecialFolder.LocalApplicationData);

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
    }
}
#endif