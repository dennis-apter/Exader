using System;
using System.Text.RegularExpressions;

namespace Exader.IO
{
#if SILVERLIGHT
	public class CopyOptions
#else
	public class CopyOptions : ICloneable
#endif
	{
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
				IsTraceEnabled = IsTraceEnabled,
				ContinueOnError = ContinueOnError,
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

#if !SILVERLIGHT
		#region ICloneable Members

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
#endif

		public DateTime BaseTime { get; set; }

		public ClearCondition Clear { get; set; }

		public bool ContinueOnError { get; set; }

		public Regex Exclude
		{
			get { return exclude; }
			set
			{
				exclude = value;

				if (!explicitExcludeFiles)
				{
					excludeFiles = value;
				}

				if (!explicitExcludeDirectories)
				{
					excludeDirectories = value;
				}
			}
		}

		public Regex ExcludeDirectories
		{
			get { return excludeDirectories; }
			set
			{
				excludeDirectories = value;
				explicitExcludeDirectories = true;
			}
		}

		public bool ExcludeEmptyDirectories { get; set; }

		public Regex ExcludeFiles
		{
			get { return excludeFiles; }
			set
			{
				excludeFiles = value;
				explicitExcludeFiles = true;
			}
		}

		public Regex Include
		{
			get { return include; }
			set
			{
				include = value;

				if (explicitIncludeFiles)
				{
					includeFiles = value;
				}

				if (explicitIncludeDirectories)
				{
					includeDirectories = value;
				}
			}
		}

		public bool IncludeBeforeExclude { get; set; }

		public Regex IncludeDirectories
		{
			get { return includeDirectories; }
			set
			{
				includeDirectories = value;
				explicitIncludeDirectories = true;
			}
		}

		public Regex IncludeFiles
		{
			get { return includeFiles; }
			set
			{
				includeFiles = value;
				explicitIncludeFiles = true;
			}
		}

		public bool IsTraceEnabled { get; set; }

		public OverwriteCondition Overwrite { get; set; }

		public bool Recursive { get; set; }
		private Regex exclude;

		private Regex excludeDirectories;

		private Regex excludeFiles;

		private bool explicitExcludeDirectories;

		private bool explicitExcludeFiles;

		private bool explicitIncludeDirectories;

		private bool explicitIncludeFiles;

		private Regex include;

		private Regex includeDirectories;

		private Regex includeFiles;

		public static readonly CopyOptions Default = new CopyOptions();
		public static readonly CopyOptions ForceOverwrite = new CopyOptions { Overwrite = OverwriteCondition.Always };
	}

	public enum ClearCondition
	{
		None,
		All,
		IfOlderThenBaseTime,
	}

	public enum OverwriteCondition
	{
		None,
		Always,
		IfNewer,
	}
}
