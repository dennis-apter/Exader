#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Exader.IO
{
	public partial class FilePath
	{
		public const char TildeFillerChar = '~';
		public const char UnderscoreFillerChar = '_';
		public const char HorizontalEllipsisFillerChar = '…';

		internal const string NetworkSharePrefix = @"\\";
		internal const string CurrentDirectoryPrefix = @".\";

		/// <summary>
		///     Заменяет в пути разделитель директорий <c>\</c> на <c>/</c>;
		///     удаляет схему URL <c>file://</c>;
		///     заменяет имя диска <c>volume|</c> на <c>volume:</c>.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="driveOrHost"></param>
		/// <param name="rootFolder"></param>
		/// <param name="strict">
		///     Задает строгий режим разбора, в котором разделителем пути и тома
		///     является только <see cref="System.IO.Path.DirectorySeparatorChar" /> и
		///     <see cref="System.IO.Path.VolumeSeparatorChar" />.
		/// </param>
		/// <returns>
		///     Путь в формате <c>dir1\dir2\dir3\file.ext</c>.
		/// </returns>
		public static string CanonicalizePath(string path, out string driveOrHost, out string rootFolder, bool strict = false)
		{
			// root  +  folder  +  parentPath + name + extension
			// C:       /          x/y/z/       file   .ext
			// C:                  x/y/z/       file   .ext
			// SERVER:  /share/    x/y/z/       file   .ext

			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			driveOrHost = string.Empty;
			rootFolder = string.Empty;

			if (!strict)
			{
				// Замена всех символов / на \
				path = path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
			}

			if (path.StartsWith(NetworkSharePrefix))
			{
				driveOrHost = path.Between(NetworkSharePrefix, @"\").ToUpperInvariant();
				if (driveOrHost == string.Empty)
				{
					driveOrHost = path.ToUpperInvariant();
					path = string.Empty;
					rootFolder = @"\";
				}
				else
				{
					driveOrHost = NetworkSharePrefix + driveOrHost;
					path = path.SubstringAfter(driveOrHost.Length);
				}
			}
			else if (path.StartsWith(CurrentDirectoryPrefix))
			{
				path = path.SubstringAfter(CurrentDirectoryPrefix.Length);
			}
			else
			{
				// TODO Учесть что имена диска могут быть указаны в относительном пути.
				// Например так:
				// c:Logs/log.txt
				// – или даже так —
				// c:./Logs/log.txt

				var schemaIndex = path.IndexOf(@":\\", StringComparison.Ordinal);

				if (0 <= schemaIndex)
				{
					var schema = path.Substring(0, schemaIndex);
					if (!"FILE".EqualsIgnoreCase(schema))
					{
						throw new UriFormatException();
					}

					var volIndex = schemaIndex + 3;
					if (strict)
					{
						var volSepIndex = volIndex + 1;
						if (volSepIndex < path.Length &&
							path[volSepIndex] == System.IO.Path.VolumeSeparatorChar &&
							char.IsLetter(path[volIndex]))
						{
							volIndex = volSepIndex;
						}
					}
					else
					{
						while (volIndex < path.Length &&
							   path[volIndex] != System.IO.Path.VolumeSeparatorChar &&
							   path[volIndex] != '|')
						{
							volIndex++;
						}
					}

					if ((schemaIndex + 3) < volIndex && volIndex < path.Length)
					{
						driveOrHost = path.Substring(schemaIndex + 3, volIndex - schemaIndex - 2);
						path = path.Substring(volIndex + 1);
					}
				}
				else
				{
					var volIndexStart = 0;
					var volIndexEnd = 0;
					do
					{
						if (strict)
						{
							var volSepIndex = volIndexEnd + 1;
							if (volSepIndex < path.Length &&
								path[volSepIndex] == System.IO.Path.VolumeSeparatorChar &&
								char.IsLetter(path[volIndexEnd]))
							{
								volIndexEnd = volSepIndex;
							}
						}
						else
						{
							while (volIndexEnd < path.Length &&
								   path[volIndexEnd] != System.IO.Path.VolumeSeparatorChar &&
								   path[volIndexEnd] != '|')
							{
								volIndexEnd++;
							}
						}

						if (0 < volIndexEnd && volIndexEnd < path.Length)
						{
							driveOrHost = path.Substring(volIndexStart, (volIndexEnd - volIndexStart) + 1);
							if ("FILE:".EqualsIgnoreCase(driveOrHost))
							{
								volIndexEnd++;
								volIndexStart = volIndexEnd + 1;
								continue;
							}
							driveOrHost = driveOrHost.ToUpperInvariant();

							path = path.Substring(volIndexEnd + 1);
						}

						break;
					}
					while (true);
				}

				if (!strict)
				{
					// Бывает и такое как file:///d|/files/
					driveOrHost = driveOrHost.Replace('|', ':').TrimStart('\\');
				}
			}

			if (path.StartsWith("\\"))
			{
				// network share
				if (driveOrHost.Length > 2)
				{
					path = path.Substring(1);
					rootFolder = path.SubstringBeforeOrSelf('\\', out path, true);
					rootFolder = "\\" + rootFolder;
				}
				else
				{
					rootFolder = "\\";
					path = path.Substring(1);
				}

				if (path.StartsWith(".."))
				{
					if (driveOrHost != string.Empty)
					{
						throw new FormatException();
					}
				}
			}

			while (path.EndsWith("\\.\\"))
			{
				path = path.RemoveRight(2);
			}

			return path;
		}

		public sealed class Builder<T> : Builder
		{
			public Builder(string value, Builder parent = null)
				: base(value, parent)
			{
			}

			public Builder(T item, string value, Builder parent = null)
				: base(value, parent)
			{
				Item = item;
			}

			private Builder(string name, string extension, int index)
				: base(name, extension, index)
			{
			}

			public T Item { get; set; }

			public new Builder<T> Clone()
			{
				return (Builder<T>)CloneInternal();
			}

			protected override Builder CloneInternal()
			{
				var clone = new Builder<T>(NameWithoutExtension, Extension, Index) { Item = Item };
				CopyTo(clone);
				return clone;
			}
		}

		public class Builder : IHierarchy<Builder>, ICloneable, IEquatable<Builder>
		{
			private const int MinNameLength = 8;
			private const int MaxNameLength = 255;
			[CanBeNull]
			private Dictionary<string, string> _annotations;
			[CanBeNull]
			private HashSet<Builder> _children;
			private int _limit;
			[NotNull]
			private readonly string _extension = string.Empty;
			[NotNull]
			private readonly string _name = string.Empty;
			[NotNull]
			private readonly string _value = string.Empty;

			/// <summary>
			///     Создает экземпляр пути корневой директории.
			/// </summary>
			public Builder()
			{
			}

			public Builder(string value, Builder parent = null)
			{
#if NET35
				if (!string.IsNullOrEmpty(value))
#else
				if (!string.IsNullOrWhiteSpace(value))
#endif
				{
					value = value.Trim();

					_value = value;
					_limit = value.Length;

					var ext = value.SubstringAfter('.', true);
					var name = value.RemoveRight(ext.Length);

					_name = SanitizeFileName(name, '_').Collapse('_', 1);
					_extension = SanitizeFileName(ext, '_').Collapse('_', 1);

					if (null != parent)
					{
						parent.Add(this);
					}
				}
				else if (parent != null)
				{
					throw new ArgumentException("Unable to create root file path with to parent folder.");
				}
			}

			protected Builder(string name, string extension, int index)
			{
				_name = name ?? string.Empty;
				_extension = extension ?? string.Empty;
				_limit = _name.Length + _extension.Length;
				Index = index;
			}

			public string this[string annotation]
			{
				get
				{
					return _annotations != null
						? _annotations.GetValueOrDefault(annotation)
						: null;
				}
				set
				{
					if (_annotations == null)
					{
						_annotations = new Dictionary<string, string>(1);
					}

					_annotations[annotation] = value;
				}
			}

			public string NameWithoutExtensionAndIndex
			{
				get { return GetNameWithoutExtensionAndIndex(); }
			}

			public string NameWithoutExtension
			{
				get { return GetNameWithoutExtension(); }
			}

			public string Name
			{
				get { return GetName(); }
			}

			public bool HasChildren
			{
				get { return !_children.IsNullOrEmpty(); }
			}

			public string Extension
			{
				get { return _extension; }
			}

			public int Index { get; private set; }

			public bool IsRoot
			{
				get
				{
					return Parent == null
						&& _name == string.Empty
						&& _extension == string.Empty;
				}
			}

			public string ParentPath
			{
				get { return Parent != null ? Parent.ToString() : string.Empty; }
			}

			object ICloneable.Clone()
			{
				return CloneInternal();
			}

			public bool Equals(Builder other)
			{
				if (ReferenceEquals(null, other))
				{
					return false;
				}

				if (ReferenceEquals(this, other))
				{
					return true;
				}

				return Index == other.Index
					&& _limit == other._limit
					&& string.Equals(_name, other._name)
					&& string.Equals(_extension, other._extension);
			}

			public Builder Parent { get; private set; }

			public IEnumerable<Builder> Children
			{
				get { return _children ?? Enumerable.Empty<Builder>(); }
			}

			public string GetNameWithoutExtensionAndIndex(char filler = HorizontalEllipsisFillerChar)
			{
				switch (filler)
				{
					case HorizontalEllipsisFillerChar:
					case UnderscoreFillerChar:
					case TildeFillerChar:
						break;
					default:
						throw new ArgumentOutOfRangeException("filler",
							"Invalid the trimmed file name filler character: '" + filler + "'.");
				}

				var name = _name;
				if (_limit < _value.Length)
				{
					var delta = _value.Length - _limit + 1 /*filler*/;
					//if (HasChildren)
					//{
					//	delta--;
					//}

					name = name.RemoveRight(delta) + filler;
				}

				return name;
			}

			public string GetNameWithoutExtension(char filler = HorizontalEllipsisFillerChar)
			{
				var name = GetNameWithoutExtensionAndIndex(filler);
				if (Index != 0)
				{
					name += "(" + Index + ")";
				}

				return name;
			}

			public string GetName(char filler = HorizontalEllipsisFillerChar)
			{
				var name = GetNameWithoutExtension(filler);
				if (_extension != string.Empty)
				{
					name += _extension;
				}

				return name;
			}

			/// <summary>
			///     Разбирает строку файлового пути, разделяя её через <see cref="Path.DirectorySeparatorChar" />.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Builder Parse(string path)
			{
				return Parse(path, System.IO.Path.DirectorySeparatorChar);
			}

			/// <summary>
			///     Разбирает строку файлового пути, разделяя её через <see cref="Path.AltDirectorySeparatorChar" />.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Builder ParseAlt(string path)
			{
				return Parse(path, System.IO.Path.AltDirectorySeparatorChar);
			}

			public static Builder Parse(string path, params char[] directorySeparatorChars)
			{
				Builder parent = null;
				Builder child = null;

				string driveOrHost;
				string rootFolder;
				path = CanonicalizePath(path, out driveOrHost, out rootFolder, true);
				var terminated = false;
				foreach (var name in path.Split(directorySeparatorChars))
				{
					if (parent != null && string.IsNullOrEmpty(name))
					{
						terminated = true;
						continue;
					}

					if (terminated)
					{
						throw new ArgumentException("File name is empty");
					}

					child = new Builder(name);
					if (parent != null)
					{
						parent.Add(child);
					}

					parent = child;
				}

				if (rootFolder != string.Empty)
				{
					var r = child.Root();
					if (r.Name != string.Empty)
					{
						new Builder().Add(r);
					}
				}

				return child;
			}

			public Builder Add(string path)
			{
				var child = Parse(path);
				Add(child.Root());
				return child;
			}

			public void Add(Builder child)
			{
				if (child == null) throw new ArgumentNullException("child");
				if (child.Parent != null) throw new InvalidOperationException("Parent of path already specified");
				if (child._limit == 0) throw new InvalidOperationException("Can't add root directory as child");
				if (_children == null) _children = new HashSet<Builder>();

				child.Parent = this;
				if (_children.Contains(child) && child.Index == 0)
				{
					var exist = _children.First(i => i.Equals(child));
					if (exist.HasChildren)
					{
						if (child.HasChildren)
						{
							foreach (var c in child.Children)
							{
								c.Parent = null;
								exist.Add(c);
							}
						}

						return;
					}

					_children.Remove(exist);
					exist.Index = 1;
					exist._limit = int.MaxValue;
					exist._limit = child.Name.Length;
					_children.Add(exist);

					child.Index = 2;
					child._limit = int.MaxValue;
					child._limit = child.Name.Length;
				}

				while (!_children.Add(child))
				{
					child.Index++;
					child._limit = int.MaxValue;
					child._limit = child.Name.Length;
				}
			}

			protected virtual Builder CloneInternal()
			{
				var clone = new Builder(_name, _extension, Index);
				CopyTo(clone);
				return clone;
			}

			protected virtual void CopyTo(Builder clone)
			{
				if (_annotations != null)
				{
					clone._annotations = new Dictionary<string, string>(_annotations);
				}

				if (Parent != null)
				{
					clone.Parent = Parent.Clone();
					if (clone.Parent._children == null)
					{
						clone.Parent._children = new HashSet<Builder>();
					}

					clone.Parent._children.Add(clone);
				}
			}

			public Builder Clone()
			{
				return CloneInternal();
			}

			private string ToString(char separator, char filler)
			{
				if (IsRoot)
				{
					return separator.ToString();
				}

				var str = GetName(filler);
				if (HasChildren)
				{
					str = str + separator;
				}

				if (Parent != null && 0 <= Parent._limit)
				{
					str = Parent.ToString(separator, filler) + str;
				}

				return str;
			}

			public string ToAltString()
			{
				return ToString(System.IO.Path.AltDirectorySeparatorChar, HorizontalEllipsisFillerChar);
			}

			public override string ToString()
			{
				return ToString(System.IO.Path.DirectorySeparatorChar, HorizontalEllipsisFillerChar);
			}

			public FilePath ToFilePath()
			{
				if (_name == string.Empty && _extension == string.Empty)
				{
					return RelativeRoot;
				}

				var parent = Parent != null
					? Parent.ToFilePath()
					: null;

				if (HasChildren)
				{
					return new FilePath(parent, NameWithoutExtension, Extension, true);
				}

				return new FilePath(parent, NameWithoutExtension, Extension, false);
			}

			public void Trim(int maxLength = MaxNameLength, int minLength = MinNameLength)
			{
				if (_children == null)
				{
					if (!TryTrim(maxLength, minLength))
					{
						throw new InvalidOperationException("Неудалось привести имена файлов к компактному представлению.");
					}

					return;
				}

				foreach (var leaf in this.Leaves().ToList())
				{
					if (!leaf.TryTrim(maxLength, minLength))
					{
						throw new InvalidOperationException("Неудалось привести имена файлов к компактному представлению.");
					}
				}
			}

			public int GetLength()
			{
				var length = Parent != null
					? Parent.GetLength()
					: 0;

				length += _name.Length - (_value.Length - _limit);

				if (_extension != string.Empty)
				{
					length += _extension.Length;
				}
				else if (length == 0)
				{
					// Root
					return 1;
				}

				if (0 < Index)
				{
					length += Index.ToString().Length + 2 /*()*/;
				}

				if (HasChildren)
				{
					// Separator
					length++;
				}

				return length;
			}

			private bool TryTrim(int maxLength, int minLength)
			{
				int index;
				do
				{
					var lim = _limit;
					var total = GetLength();
					if (HasChildren)
					{
						total--;
					}

					var rem = maxLength - total;
					if (rem < 0)
					{
						var minLim = minLength;
						if (minLim < _extension.Length)
						{
							minLim = _extension.Length + 1 /*name*/;
							if (0 < Index)
							{
								Debug.Assert(Parent != null, "Parent != null");
								Debug.Assert(Parent._children != null, "Parent._children != null");

								minLim += Parent._children.Count.ToString().Length + 2 /*()*/;
							}
						}

						lim += rem;
						if (lim < minLim)
						{
							if (Parent != null)
							{
								if (Parent.TryTrim(maxLength - (minLim + 1 /*separator*/), minLength))
								{
									lim = minLim;
								}
								else
								{
									return false;
								}
							}
							else
							{
								return false;
							}
						}
					}

					index = Index;

					if (_limit != lim)
					{
						if (Parent != null)
						{
							var parent = Parent;
							parent.Remove(this);

							_limit = lim;

							parent.Add(this);
						}
						else
						{
							_limit = lim;
						}
					}
				}
				while (Index != index && Index.ToString().Length != index.ToString().Length);

				return true;
			}

			private void Remove(Builder child)
			{
				child.Index = 0;
				child.Parent = null;

				if (_children == null) return;

				_children.Remove(child);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != GetType()) return false;
				return Equals((Builder)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = Index;
					hashCode = (hashCode * 397) ^ _limit;
					hashCode = (hashCode * 397) ^ _name.GetHashCode();
					hashCode = (hashCode * 397) ^ _extension.GetHashCode();
					return hashCode;
				}
			}

			public static bool operator ==(Builder left, Builder right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(Builder left, Builder right)
			{
				return !Equals(left, right);
			}
		}
	}
}

#endif