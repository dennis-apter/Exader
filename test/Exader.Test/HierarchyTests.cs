using System;
using System.Collections.Generic;
using System.Linq;
using Exader.IO;
using Xunit;

namespace Exader
{
    public class HierarchyTests
    {
        [Fact]
        public void Level()
        {
            Node leaf;
            var root = new Node
            {
                Children = new List<Node>
                {
                    new Node
                    {
                        Children = new List<Node>
                        {
                            (leaf = new Node())
                        }
                    }
                }
            };

            Assert.Equal(0, root.Level());
            Assert.Equal(2, leaf.Level());
        }

        [Fact]
        public void Path()
        {
            Node leaf;
            var root = new Node
            {
                Children = new List<Node>
                {
                    new Node
                    {
                        Children = new List<Node>
                        {
                            (leaf = new Node())
                        }
                    }
                }
            };

            var path = leaf.Path();
            Assert.Equal("/Node/Node/Node", path.ToString());
            Assert.Equal("Node|Node|Node", path.ToString("|", true));

            Assert.Equal(3, path.Count);
            Assert.Equal(root, path.Root);
            Assert.Equal(leaf, path.Leaf);

            Assert.Equal(leaf.Parent.Parent, path[0]);
            Assert.Equal(leaf.Parent, path[1]);
            Assert.Equal(leaf, path[2]);

            Assert.NotEqual(path.Root, path.Leaf);
        }

        [Fact]
        public void IsAncestorOf()
        {
            Node leaf;
            var root = new Node
            {
                Children = new List<Node>
                {
                    new Node
                    {
                        Children = new List<Node>
                        {
                            (leaf = new Node())
                        }
                    }
                }
            };

            Assert.True(root.IsAncestorOf(leaf));
            Assert.True(leaf.IsAncestorOrSelfOf(leaf));
            Assert.True(root.IsAncestorOrSelfOf(root));

            Assert.True(leaf.IsDescendantOf(root));
            Assert.True(leaf.IsDescendantOrSelfOf(leaf));
            Assert.True(root.IsDescendantOrSelfOf(root));
        }

        [Fact]
        public void IsAncestorOfNullReference()
        {
            Node leaf;
            var root = new Node
            {
                Children = new List<Node>
                {
                    new Node
                    {
                        Children = new List<Node>
                        {
                            (leaf = new Node())
                        }
                    }
                }
            };

            Assert.False(root.IsAncestorOf(null));
            Assert.False(((IHierarchy<Node>)null).IsAncestorOf(root));
            Assert.False(((IHierarchy<Node>)null).IsAncestorOf(null));

            Assert.False(leaf.IsAncestorOrSelfOf(null));
            Assert.False(((IHierarchy<Node>)null).IsAncestorOrSelfOf(leaf));
            Assert.True(((IHierarchy<Node>)null).IsAncestorOrSelfOf(null));

            Assert.False(root.IsDescendantOf(null));
            Assert.False(((IHierarchy<Node>)null).IsDescendantOf(root));
            Assert.False(((IHierarchy<Node>)null).IsDescendantOf(null));

            Assert.False(leaf.IsDescendantOrSelfOf(null));
            Assert.False(((IHierarchy<Node>)null).IsDescendantOrSelfOf(leaf));
            Assert.True(((IHierarchy<Node>)null).IsDescendantOrSelfOf(null));
        }

        [Fact]
        public void Leaves()
        {
            //var root = CreateNode(FilePaths.System);
            var root = new Node
            {
                Children = new[]
                {
                    new Node(),
                    new Node { Children = new [] { new Node(), new Node()} },
                    new Node(),
                }
            };

            var leaves1 = root.Descendants().Where(d => d.IsLeaf()).ToList();
            var leaves2 = root.Leaves().ToList();
            Assert.Equal(leaves1, leaves2);
        }

        //private static Node CreateNode(FilePath dir)
        //{
        //	try
        //	{
        //		return new Node
        //		{
        //			Children = dir.Directories().Select(CreateNode).ToList()
        //		};
        //	}
        //	catch (Exception)
        //	{
        //		return new Node
        //		{
        //			Children = Enumerable.Empty<Node>()
        //		};
        //	}
        //}

        private class Node : IHierarchy<Node>
        {
            private IEnumerable<Node> m_Children;

            public Node()
            {
            }

            public Node(string name)
            {
                Name = name;
            }

            public IEnumerable<Node> Children
            {
                get { return m_Children; }
                set
                {
                    m_Children = value;
                    if (m_Children == null) return;
                    m_Children = m_Children.ToList();
                    if (!m_Children.Any())
                    {
                        m_Children = null;
                        return;
                    }

                    foreach (var child in m_Children)
                    {
                        child.Parent = this;
                    }
                }
            }

            public Node Parent { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return Name ?? "Node";
            }
        }
    }
}
