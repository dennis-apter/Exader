using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Exader.IO
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CopyOptions
    {
        public static readonly CopyOptions Default = new CopyOptions();
        public static readonly CopyOptions ForceOverwrite = new CopyOptions {Overwrite = OverwriteCondition.Always};

        private Regex _exclude;
        private Regex _excludeDirectories;
        private Regex _excludeFiles;
        private bool _explicitExcludeDirectories;
        private bool _explicitExcludeFiles;
        private bool _explicitIncludeDirectories;
        private bool _explicitIncludeFiles;
        private Regex _include;
        private Regex _includeDirectories;
        private Regex _includeFiles;

        public DateTime BaseTime { get; set; }
        public ClearCondition Clear { get; set; }
        public bool ContinueOnError { get; set; }

        public Regex Exclude
        {
            get => _exclude;
            set
            {
                _exclude = value;

                if (!_explicitExcludeFiles)
                {
                    _excludeFiles = value;
                }

                if (!_explicitExcludeDirectories)
                {
                    _excludeDirectories = value;
                }
            }
        }

        public Regex ExcludeDirectories
        {
            get => _excludeDirectories;
            set
            {
                _excludeDirectories = value;
                _explicitExcludeDirectories = true;
            }
        }

        public bool ExcludeEmptyDirectories { get; set; }

        public Regex ExcludeFiles
        {
            get => _excludeFiles;
            set
            {
                _excludeFiles = value;
                _explicitExcludeFiles = true;
            }
        }

        public Regex Include
        {
            get => _include;
            set
            {
                _include = value;

                if (_explicitIncludeFiles)
                {
                    _includeFiles = value;
                }

                if (_explicitIncludeDirectories)
                {
                    _includeDirectories = value;
                }
            }
        }

        public bool IncludeBeforeExclude { get; set; }

        public Regex IncludeDirectories
        {
            get => _includeDirectories;
            set
            {
                _includeDirectories = value;
                _explicitIncludeDirectories = true;
            }
        }

        public Regex IncludeFiles
        {
            get => _includeFiles;
            set
            {
                _includeFiles = value;
                _explicitIncludeFiles = true;
            }
        }

        public OverwriteCondition Overwrite { get; set; }

        public bool Recursive { get; set; }

        public CopyOptions Clone()
        {
            return new CopyOptions
            {
                Clear = Clear,
                BaseTime = BaseTime,
                Overwrite = Overwrite,
                Recursive = Recursive,
                Include = Include,
                IncludeBeforeExclude = IncludeBeforeExclude,
                Exclude = Exclude,
                ExcludeEmptyDirectories = ExcludeEmptyDirectories,
                ContinueOnError = ContinueOnError
            };
        }

        public bool IsAccept(string path, bool isFile)
        {
            if (isFile)
            {
                if (IncludeBeforeExclude)
                {
                    return (null == IncludeFiles || IncludeFiles.IsMatch(path))
                           && (null == ExcludeFiles || !ExcludeFiles.IsMatch(path));
                }

                return (null == ExcludeFiles || !ExcludeFiles.IsMatch(path))
                       && (null == IncludeFiles || IncludeFiles.IsMatch(path));
            }

            if (IncludeBeforeExclude)
            {
                return (null == IncludeDirectories || IncludeDirectories.IsMatch(path))
                       && (null == ExcludeDirectories || !ExcludeDirectories.IsMatch(path));
            }

            return (null == ExcludeDirectories || !ExcludeDirectories.IsMatch(path))
                   && (null == IncludeDirectories || IncludeDirectories.IsMatch(path));
        }
    }

    public enum ClearCondition
    {
        None,
        All,
        IfOlderThenBaseTime
    }

    public enum OverwriteCondition
    {
        None,
        Always,
        IfNewer
    }
}
