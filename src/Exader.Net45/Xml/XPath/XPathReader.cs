using System;
using System.Xml;
using System.Xml.XPath;

namespace Exader.Xml.XPath
{
	public class XPathReader : XPathNavigator
	{
		private static XPathNodeType ConvertNodeType(XmlNodeType nodeType)
		{
			switch (nodeType)
			{
				case XmlNodeType.Element:
					//case XmlNodeType.EndElement:
					return XPathNodeType.Element;

				case XmlNodeType.Attribute:
					return XPathNodeType.Attribute;

				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					return XPathNodeType.Text;

				case XmlNodeType.ProcessingInstruction:
					return XPathNodeType.ProcessingInstruction;

				case XmlNodeType.Comment:
					return XPathNodeType.Comment;

				case XmlNodeType.Whitespace:
					return XPathNodeType.Whitespace;

				case XmlNodeType.SignificantWhitespace:
					return XPathNodeType.SignificantWhitespace;

				case XmlNodeType.None:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentFragment:
					return XPathNodeType.Root;

				default:
					return ~XPathNodeType.Root;
			}
		}

		private static NotSupportedException NotSupportReverseRead()
		{
			return new NotSupportedException(
				"Exader.Xml.XPathReader поддерживает только последовательное чтение вперед.");
		}

		private readonly XmlReader reader;

		private bool moved;

		public XPathReader(XmlReader reader)
		{
			this.reader = reader;
		}

		public override XPathNavigator Clone()
		{
			return this;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			return ReferenceEquals(this, other);
		}

		public override bool MoveTo(XPathNavigator other)
		{
			return ReferenceEquals(this, other);
		}

		public override bool MoveToFirstAttribute()
		{
			return reader.MoveToFirstAttribute();
		}

		public override bool MoveToFirstChild()
		{
			if (XmlNodeType.None == reader.NodeType)
			{
				moved = true;
				reader.MoveToContent();
				return !reader.EOF;
			}

			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return false;
			}

			moved = true;
			return reader.Read();
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
		{
			switch (scope)
			{
				case XPathNamespaceScope.All:
					if (MoveToFirstNamespaceGlobal())
					{
						return true;
					}

					throw new NotImplementedException();

				case XPathNamespaceScope.ExcludeXml:
					throw new NotImplementedException();

				case XPathNamespaceScope.Local:
					if (reader.HasAttributes)
					{
						if (!MoveToFirstNamespaceLocal())
						{
							return false;
						}

						return true;
					}

					break;
			}

			return false;
		}

		public override bool MoveToId(string id)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToNext()
		{
			moved = true;
			return reader.Read();
		}

		public override bool MoveToNextAttribute()
		{
			return reader.MoveToNextAttribute();
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToParent()
		{
			if (XmlNodeType.Attribute == reader.NodeType)
			{
				return reader.MoveToElement();
			}

			throw NotSupportReverseRead();

			////while (this.reader.Read())
			////{
			////    if (this.reader.NodeType == XmlNodeType.EndElement)
			////    {
			////        return true;
			////    }
			////}

			////return !this.reader.EOF;
		}

		public override bool MoveToPrevious()
		{
			throw new NotSupportedException(
				"Exader.Xml.XPathReader поддерживает только последовательное чтение вперед.");
		}

		public override void MoveToRoot()
		{
			if (XPathNodeType.Root == NodeType)
			{
				reader.MoveToContent();
				return;
			}

			throw NotSupportReverseRead();
		}

		public override XmlReader ReadSubtree()
		{
			switch (NodeType)
			{
				case XPathNodeType.Root:
				case XPathNodeType.Element:
					return base.ReadSubtree();

				default:
					return reader;
			}
		}

		public override XPathNodeIterator Select(XPathExpression expr)
		{
			moved = false;
			XPathNodeIterator it = base.Select(expr);
			if (moved)
			{
				throw new NotImplementedException();
			}

			return it;
		}

		private bool IsNamespace()
		{
			return "http://www.w3.org/2000/xmlns/" == reader.NamespaceURI;
		}

		private bool MoveToFirstNamespaceGlobal()
		{
			if (MoveToFirstNamespaceLocal())
			{
				return true;
			}

			throw new NotSupportedException();
		}

		private bool MoveToFirstNamespaceLocal()
		{
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					if (IsNamespace())
					{
						return true;
					}

				} while (reader.MoveToNextAttribute());
			}

			return false;
		}

		public override string BaseURI
		{
			get { return reader.BaseURI; }
		}

		public override bool IsEmptyElement
		{
			get { return reader.IsEmptyElement; }
		}

		public override string LocalName
		{
			get { return reader.LocalName; }
		}

		public override string Name
		{
			get { return reader.Name; }
		}

		public override XmlNameTable NameTable
		{
			get { return reader.NameTable; }
		}
		public override string NamespaceURI
		{
			get { return reader.NamespaceURI; }
		}

		public override XPathNodeType NodeType
		{
			get { return ConvertNodeType(reader.NodeType); }
		}

		public override string Prefix
		{
			get { return reader.Prefix; }
		}

		public override string Value
		{
			get { return reader.Value; }
		}

		public XmlReader Reader
		{
			get { return reader; }
		}
	}
}