using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Exader
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public struct ProductVersion : IXmlSerializable, IEquatable<ProductVersion>, IComparable<ProductVersion>
	{
		private int maintenance;
		private int major;
		private int minor;
		private int revision;
		private ProductStage stage;
		private int subStage;

		public ProductVersion(int major, int minor)
		{
			this.major = major;
			this.minor = minor;
			maintenance = -1;
			revision = -1;
			stage = ProductStage.Release;
			subStage = 1;
		}

		public ProductVersion(int major, int minor, ProductStage stage)
		{
			this.major = major;
			this.minor = minor;
			maintenance = -1;
			revision = -1;
			this.stage = stage;
			subStage = 1;
		}

		public ProductVersion(int major, int minor, ProductStage stage, int subStage)
		{
			this.major = major;
			this.minor = minor;
			maintenance = -1;
			revision = -1;
			this.stage = stage;
			this.subStage = subStage;
		}

		public ProductVersion(int major, int minor, int maintenance)
		{
			this.major = major;
			this.minor = minor;
			this.maintenance = maintenance;
			revision = -1;
			stage = ProductStage.Release;
			subStage = 1;
		}

		public ProductVersion(int major, int minor, int milestone, ProductStage stage)
		{
			this.major = major;
			this.minor = minor;
			maintenance = milestone;
			revision = -1;
			this.stage = stage;
			subStage = 1;
		}

		public ProductVersion(int major, int minor, int maintenance, ProductStage stage, int subStage)
		{
			this.major = major;
			this.minor = minor;
			this.maintenance = maintenance;
			revision = -1;
			this.stage = stage;
			this.subStage = subStage;
		}

		public ProductVersion(int major, int minor, int maintenance, int revision)
		{
			this.major = major;
			this.minor = minor;
			this.maintenance = maintenance;
			this.revision = revision;
			stage = ProductStage.Release;
			subStage = 1;
		}

		public ProductVersion(int major, int minor, int maintenance, int revision, ProductStage stage)
		{
			this.major = major;
			this.minor = minor;
			this.maintenance = maintenance;
			this.revision = revision;
			this.stage = stage;
			subStage = 1;
		}

		public ProductVersion(
			int major,
			int minor,
			int maintenance,
			int revision,
			ProductStage stage,
			int subStage)
		{
			this.major = major;
			this.minor = minor;
			this.maintenance = maintenance;
			this.revision = revision;
			this.stage = stage;
			this.subStage = subStage;
		}

		public ProductVersion(
			int major,
			int? minor,
			int? maintenance,
			int? revision,
			ProductStage stage,
			int? subStage)
		{
			this.major = major;
			this.minor = minor ?? -1;
			this.maintenance = maintenance ?? -1;
			this.revision = revision ?? -1;
			this.stage = stage;
			this.subStage = subStage ?? 1;
		}

		public ProductVersion(Version version)
		{
			major = version.Major;
			minor = version.Minor;
			maintenance = version.Build;
			revision = version.Revision;
			stage = ProductStage.Release;
			subStage = 1;
		}

		public ProductVersion(Version version, ProductStage stage)
		{
			major = version.Major;
			minor = version.Minor;
			maintenance = version.Build;
			revision = version.Revision;
			this.stage = stage;
			subStage = 1;
		}

		public ProductVersion(Version version, ProductStage stage, int subStage)
		{
			major = version.Major;
			minor = version.Minor;
			maintenance = version.Build;
			revision = version.Revision;
			this.stage = stage;
			this.subStage = subStage;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (obj.GetType() != typeof(ProductVersion))
			{
				return false;
			}

			return Equals((ProductVersion)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = major;
				result = (result * 397) ^ minor;
				result = (result * 397) ^ maintenance;
				result = (result * 397) ^ revision;
				result = (result * 397) ^ stage.GetHashCode();
				result = (result * 397) ^ subStage;
				return result;
			}
		}

		public override string ToString()
		{
			return ToString(false);
		}

		/// <summary>
		/// Версия содержащая основной функционал продукта (начато тестирование).
		/// </summary>
		/// <remarks>
		/// На этом этапе продукт заканчивает интеграцию различных модулей и переходит
		/// к их совместному тестированию (с применением заглушек и модульных тестов).
		/// </remarks>
		public ProductVersion A(int number)
		{
			return Alpha(number);
		}

		/// <summary>
		/// Версия содержащая основной функционал продукта (начато тестирование).
		/// </summary>
		/// <remarks>
		/// На этом этапе продукт заканчивает интеграцию различных модулей и переходит
		/// к их совместному тестированию (с применением заглушек и модульных тестов).
		/// </remarks>
		public ProductVersion Alpha(int number)
		{
			stage = ProductStage.Alpha;
			subStage = number;
			return this;
		}

		/// <summary>
		/// В основном стабильная версия продукта (заканчивается тестирование).
		/// </summary>
		/// <remarks>
		/// На этом этапе продукт заканчивает основное тестирование и согласуется
		/// с заказчиком для выявления ошибок реализации (тестирование в реальном окружении,
		/// всей системы в сборе).
		/// </remarks>
		public ProductVersion B(int number)
		{
			return Beta(number);
		}

		/// <summary>
		/// В основном стабильная версия продукта (заканчивается тестирование).
		/// </summary>
		/// <remarks>
		/// На этом этапе продукт заканчивает основное тестирование и согласуется
		/// с заказчиком для выявления ошибок реализации (тестирование в реальном окружении,
		/// всей системы в сборе).
		/// </remarks>
		public ProductVersion Beta(int number)
		{
			stage = ProductStage.Beta;
			subStage = number;
			return this;
		}

		/// <summary>
		/// Внутрення версия для разработки. Может реализовать различные части функционала продукта,
		/// предназначена для обката различных технологических путей реализации модулей продукта.
		/// </summary>
		/// <remarks>
		/// На этом этапе согласование проходит между разработчиками продукта.
		/// Заказчик в этот процес ни как не вовлечен.
		/// </remarks>
		public ProductVersion D(int number)
		{
			return Development(number);
		}

		/// <summary>
		/// Внутрення версия для разработки. Может реализовать различные части функционала продукта,
		/// предназначена для обката различных технологических путей реализации модулей продукта.
		/// </summary>
		/// <remarks>
		/// На этом этапе согласование проходит между разработчиками продукта.
		/// Заказчик в этот процес ни как не вовлечен.
		/// </remarks>
		public ProductVersion Development(int number)
		{
			stage = ProductStage.Development;
			subStage = number;
			return this;
		}

		/// <summary>
		/// Кандидат на выпуск продукта (все уже готово, испраление ошибок,
		/// мелкие доработки оформления и документации).
		/// </summary>
		public ProductVersion P(int number)
		{
			return PrePelease(number);
		}

		/// <summary>
		/// Кандидат на выпуск продукта (все уже готово, испраление ошибок,
		/// мелкие доработки оформления и документации).
		/// </summary>
		public ProductVersion PrePelease(int number)
		{
			stage = ProductStage.PreRelease;
			subStage = number;
			return this;
		}

		public ProductVersion R(int number)
		{
			return Release(subStage);
		}

		public ProductVersion Release(int number)
		{
			stage = ProductStage.Release;
			subStage = number;
			return this;
		}

		public string ToString(bool longStageName)
		{
			var buffer = new StringBuilder(major + "." + minor);
			if (0 < maintenance)
			{
				buffer.Append("." + maintenance);
			}

			if (0 < revision)
			{
				buffer.Append("." + revision);
			}

			switch (stage)
			{
				case ProductStage.Development:
					buffer.Append(longStageName ? " Development" : "d");
					break;

				case ProductStage.Alpha:
					buffer.Append(longStageName ? " Alpha" : "a");
					break;

				case ProductStage.Beta:
					buffer.Append(longStageName ? " Beta" : "b");
					break;

				case ProductStage.PreRelease:
					buffer.Append(longStageName ? " Release Candidate" : "rc");
					break;

				case ProductStage.Release:
					if (1 < subStage) { buffer.Append(longStageName ? " Release" : "r"); }
					break;
			}

			if (1 < subStage)
			{
				if (longStageName)
				{
					buffer.Append(" ");
				}

				buffer.Append(subStage);
			}

			return buffer.ToString();
		}

		#region IComparable<ProductVersion> Members

		public int CompareTo(ProductVersion other)
		{
			if (major != other.major)
			{
				if (major < other.major)
				{
					return -1;
				}

				return 1;
			}

			if (minor != other.minor)
			{
				if (minor < other.minor)
				{
					return -1;
				}

				return 1;
			}

			if (maintenance != other.maintenance)
			{
				if (maintenance < other.maintenance)
				{
					return -1;
				}

				return 1;
			}

			if (revision != other.revision)
			{
				if (revision < other.revision)
				{
					return -1;
				}

				return 1;
			}

			if (stage != other.stage)
			{
				if (stage < other.stage)
				{
					return -1;
				}

				return 1;
			}

			if (subStage == other.subStage)
			{
				return 0;
			}

			if (subStage < other.subStage)
			{
				return -1;
			}

			return 1;

		}

		#endregion

		#region IEquatable<ProductVersion> Members

		public bool Equals(ProductVersion other)
		{
			return major == other.major
				&& maintenance == other.maintenance
					&& minor == other.minor
						&& revision == other.revision
							&& Equals(stage, other.stage)
								&& subStage == other.subStage;
		}

		#endregion

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
#if SILVERLIGHT
			return null;
#else
			var schema = new XmlSchema
			{
				Id = GetType().Name,
				ElementFormDefault = XmlSchemaForm.Qualified,
				TargetNamespace = "http://regioncad.ru/wsdl/types/"
			};

			schema.Items.Add(
				new XmlSchemaSimpleType
				{
					Name = GetType().Name,
					Content = new XmlSchemaSimpleTypeRestriction
					{
						BaseType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String)
					}
				});

			return schema;
#endif
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader.Read())
			{
				string rawValue = reader.Value;
				////if (!reader.HasAttributes)
				////{
				////    return;
				////}

				////rawValue = reader.GetAttribute("Value");
				ProductVersion pv = Parse(rawValue);

				major = pv.major;
				minor = pv.minor;
				maintenance = pv.maintenance;

				stage = pv.stage;
				subStage = pv.subStage;

				revision = pv.revision;
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			////writer.WriteStartElement("ProductVersion");
			writer.WriteValue(ToString());
			////writer.WriteEndElement();
		}

		#endregion

		public int Maintenance
		{
			get { return maintenance; }
		}

		public int Major
		{
			get { return major; }
		}

		public int Minor
		{
			get { return minor; }
		}

		public int Revision
		{
			get { return revision; }
		}

		public ProductStage Stage
		{
			get { return stage; }
		}

		public int SubStage
		{
			get { return subStage; }
		}

		public static bool operator ==(ProductVersion x, ProductVersion y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(ProductVersion x, ProductVersion y)
		{
			return !Equals(x, y);
		}

		public static string Copiright(Assembly assembly)
		{
			var customAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
			if (customAttribute == null)
			{
				return null;
			}
			return customAttribute.Copyright;
		}

		public static string Description(Assembly assembly)
		{
			var customAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
			if (customAttribute == null)
			{
				return null;
			}

			return customAttribute.Description;
		}

		public static ProductVersion Of(Type type)
		{
			var attr = Attribute.GetCustomAttribute(type, typeof(ProductVersionAttribute)) as ProductVersionAttribute;
			if (null == attr)
			{
				attr = Attribute.GetCustomAttribute(type.Assembly, typeof(ProductVersionAttribute)) as ProductVersionAttribute;
				if (null == attr)
				{
					return Default;
				}
			}

			return attr.Version;
		}

		public static ProductVersion Parse(string text)
		{
			text = text.Trim();
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}

			int stageIndex = text.Length;
			int subStageIndex = text.Length;
			bool hasStage = false;
			for (int index = text.Length - 1; 0 < index; index--)
			{
				char symbol = text[index];
				if (char.IsDigit(symbol))
				{
					if (hasStage)
					{
						break;
					}

					subStageIndex = index;
				}

				if (char.IsLetter(symbol))
				{
					hasStage = true;
					stageIndex = index;
				}
			}

			var stage = ProductStage.Release;
			int subStage = 1;
			if (hasStage)
			{
				string rawSubStage = text.Substring(subStageIndex);
				if (!int.TryParse(rawSubStage, out subStage))
				{
					subStage = 1;
				}

				try
				{
					string rawStage = text.Substring(stageIndex, subStageIndex - stageIndex).Trim().ToUpper();
					if (rawStage.Length <= 2)
					{
						switch (rawStage)
						{
							case "D":
							case "M":
								stage = ProductStage.Development;
								break;

							case "A":
								stage = ProductStage.Alpha;
								break;

							case "B":
								stage = ProductStage.Beta;
								break;

							case "P":
							case "PR":
							case "RC":
								stage = ProductStage.PreRelease;
								break;
						}
					}
					else
					{
						try
						{
							stage = (ProductStage)Enum.Parse(typeof(ProductStage), rawStage, true);
						}
						catch (ArgumentException)
						{
							rawStage = rawStage.Replace(" ", string.Empty).Replace("-", string.Empty).ToUpper();
							switch (rawStage)
							{
								case "DEV":
								case "MILESTONE":
									stage = ProductStage.Development;
									break;

								case "PRERELEASE":
								case "RELEASECANDIDATE":
									stage = ProductStage.PreRelease;
									break;
							}
						}
					}
				}
				catch (Exception)
				{
					stage = ProductStage.Release;
				}
			}

			string vtext = text.Substring(0, stageIndex);
			try
			{
				var version = new Version(vtext);
				return new ProductVersion(version, stage, subStage);
			}
			catch (ArgumentException)
			{
				return Default;
			}
		}

		public static string Product(Assembly assembly)
		{
			var customAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
			if (customAttribute == null)
			{
				return null;
			}
			return customAttribute.Product;

		}

		public static readonly ProductVersion Default = new ProductVersion(1, 0);
	}

	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
		| AttributeTargets.Interface | AttributeTargets.Struct)]
	public sealed class ProductVersionAttribute : Attribute
	{
		public ProductVersionAttribute(int major, int minor)
		{
			Version = new ProductVersion(major, minor);
		}

		public ProductVersionAttribute(int major, int minor, ProductStage stage)
		{
			Version = new ProductVersion(major, minor, stage);
		}

		public ProductVersionAttribute(int major, int minor, ProductStage stage, int subStage)
		{
			Version = new ProductVersion(major, minor, stage, subStage);
		}

		public ProductVersionAttribute(int major, int minor, int milestone)
		{
			Version = new ProductVersion(major, minor, milestone);
		}

		public ProductVersionAttribute(int major, int minor, int milestone, ProductStage stage)
		{
			Version = new ProductVersion(major, minor, milestone, stage);
		}

		public ProductVersionAttribute(int major, int minor, int milestone, ProductStage stage, int subStage)
		{
			Version = new ProductVersion(major, minor, milestone, stage, subStage);
		}

		public ProductVersionAttribute(int major, int minor, int milestone, int revision)
		{
			Version = new ProductVersion(major, minor, milestone, revision);
		}

		public ProductVersionAttribute(int major, int minor, int milestone, int revision, ProductStage stage)
		{
			Version = new ProductVersion(major, minor, milestone, revision, stage);
		}

		public ProductVersionAttribute(
			int major,
			int minor,
			int milestone,
			int revision,
			ProductStage stage,
			int subStage)
		{
			Version = new ProductVersion(major, minor, milestone, revision, stage, subStage);
		}

		public ProductVersionAttribute(
			int major,
			int? minor,
			int? milestone,
			int? revision,
			ProductStage stage,
			int? subStage)
		{
			Version = new ProductVersion(major, minor, milestone, revision, stage, subStage);
		}

		public ProductVersionAttribute(Version version)
		{
			Version = new ProductVersion(version);
		}

		public ProductVersionAttribute(Version version, ProductStage stage)
		{
			Version = new ProductVersion(version, stage);
		}

		public ProductVersionAttribute(Version version, ProductStage stage, int subStage)
		{
			Version = new ProductVersion(version, stage, subStage);
		}

		public ProductVersion Version
		{
			get;
			private set;
		}
	}

	public enum ProductStage
	{
		Development,
		Alpha,
		Beta,
		PreRelease,
		Release,
	}
}