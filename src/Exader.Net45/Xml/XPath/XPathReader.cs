using System;
using System.Xml;
using System.Xml.XPath;
using JetBrains.Annotations;

namespace Exader.Xml.XPath
{
    public class XPathReader : XPathNavigator
    {
        private bool _moved;

        [NotNull]
        private readonly XmlReader _reader;

        public XPathReader(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _reader = reader;
        }

        public XmlReader Reader => _reader;

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
            return _reader.MoveToFirstAttribute();
        }

        public override bool MoveToFirstChild()
        {
            if (XmlNodeType.None == _reader.NodeType)
            {
                _moved = true;
                _reader.MoveToContent();
                return !_reader.EOF;
            }

            _reader.MoveToElement();
            if (_reader.IsEmptyElement)
            {
                return false;
            }

            _moved = true;
            return _reader.Read();
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
                    if (_reader.HasAttributes)
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
            _moved = true;
            return _reader.Read();
        }

        public override bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope scope)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToParent()
        {
            if (XmlNodeType.Attribute == _reader.NodeType)
            {
                return _reader.MoveToElement();
            }

            throw NotSupportReverseRead();
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
                _reader.MoveToContent();
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
                    return _reader;
            }
        }

        public override XPathNodeIterator Select(XPathExpression expr)
        {
            _moved = false;
            XPathNodeIterator it = base.Select(expr);
            if (_moved)
            {
                throw new NotImplementedException();
            }

            return it;
        }

        public override string BaseURI => _reader.BaseURI;
        public override bool IsEmptyElement => _reader.IsEmptyElement;
        public override string LocalName => _reader.LocalName;
        public override string Name => _reader.Name;
        public override string NamespaceURI => _reader.NamespaceURI;
        public override XmlNameTable NameTable => _reader.NameTable;
        public override XPathNodeType NodeType => ConvertNodeType(_reader.NodeType);
        public override string Prefix => _reader.Prefix;
        public override string Value => _reader.Value;

        private bool IsNamespace()
        {
            return "http://www.w3.org/2000/xmlns/" == _reader.NamespaceURI;
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
            if (_reader.MoveToFirstAttribute())
            {
                do
                {
                    if (IsNamespace())
                    {
                        return true;
                    }

                } while (_reader.MoveToNextAttribute());
            }

            return false;
        }

        private static XPathNodeType ConvertNodeType(XmlNodeType nodeType)
        {
            switch (nodeType)
            {
                case XmlNodeType.Element:
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
    }
}
