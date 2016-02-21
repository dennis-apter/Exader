using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exader
{
    /// <summary>
    /// Представляет поведение перечисляемого узла иерархии.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if NET35
    public interface IHierarchy<T> where T : IHierarchy<T>
#else
    public interface IHierarchy<out T> where T : IHierarchy<T>
#endif
    {
        /// <summary>
        /// Возвращает дочерние узлы.
        /// </summary>
        IEnumerable<T> Children { get; }

        /// <summary>
        /// Возвращает родительский узел.
        /// </summary>
        T Parent { get; }
    }

    public static class HierarchyExtensions
    {
        public static T AncestorAt<T>(this T node, int index) where T : IHierarchy<T>
        {
            var ancestor = node;
            while (!Equals(ancestor, default(T)) && (0 < index))
            {
                index--;
                ancestor = ancestor.Parent;
            }

            if (0 < index)
            {
                throw new IndexOutOfRangeException();
            }

            return ancestor;
        }

        public static IEnumerable<T> Ancestors<T>(this T self) where T : IHierarchy<T>
        {
            return AncestorsCore(self, false);
        }

        public static IEnumerable<T> AncestorsAndSelf<T>(this T self) where T : IHierarchy<T>
        {
            return AncestorsCore(self, true);
        }

        public static IEnumerable<T> Descendants<T>(this T self) where T : IHierarchy<T>
        {
            return DescendantsCore(self, false);
        }

        public static IEnumerable<T> Descendants<T>(this IEnumerable<T> nodes) where T : IHierarchy<T>
        {
            return nodes.SelectMany(node => node.Descendants());
        }

        public static IEnumerable<T> DescendantsAndSelf<T>(this T self) where T : IHierarchy<T>
        {
            return DescendantsCore(self, true);
        }

        public static IEnumerable<T> DescendantsAndSelf<T>(this IEnumerable<T> nodes) where T : IHierarchy<T>
        {
            return nodes.SelectMany(node => node.DescendantsAndSelf());
        }

        public static IEnumerable<T> Hierarchize<T>(this IEnumerable<T> collection) where T : class, IHierarchy<T>
        {
            var lookup = collection.ToLookup(e => e.Parent);
            return HierarchizeCore(lookup);
        }

        public static IEnumerable<T> Hierarchize<T>(this IEnumerable<T> self, Func<T, T> parent) where T : class
        {
            var lookup = self.ToLookup(parent);
            return HierarchizeCore(lookup);
        }

        public static bool IsAncestorOf<T>(this T self, T other) where T : IHierarchy<T>
        {
            return IsAncestorsCore(self, other, false);
        }

        public static bool IsAncestorOrSelfOf<T>(this T self, T other) where T : IHierarchy<T>
        {
            return IsAncestorsCore(self, other, true);
        }

        public static bool IsDescendantOf<T>(this T self, T other) where T : IHierarchy<T>
        {
            return IsAncestorsCore(other, self, false);
        }

        public static bool IsDescendantOrSelfOf<T>(this T self, T other) where T : IHierarchy<T>
        {
            return IsAncestorsCore(other, self, true);
        }

        public static bool IsLeaf<T>(this T node) where T : IHierarchy<T>
        {
            return node.Children == null || !node.Children.Any();
        }

        public static bool IsRoot<T>(this T node) where T : IHierarchy<T>
        {
            return ReferenceEquals(node.Parent, default(T)) ||
                   ReferenceEquals(node, node.Parent);
        }

        public static IEnumerable<T> Leaves<T>(this T self) where T : IHierarchy<T>
        {
            return self.Descendants().Where(e => e.IsLeaf());
        }

        public static int Level<T>(this T self) where T : IHierarchy<T>
        {
            var level = 0;
            var ancestor = self.Parent;
            while (!Equals(ancestor, default(T)))
            {
                level++;
                ancestor = ancestor.Parent;
            }

            return level;
        }

        public static Path<T> Path<T>(this T self, Func<T, T> parent) where T : class
        {
            var stack = new Stack<T>();
            stack.Push(self);
            var ancestor = parent(self);
            while (null != ancestor)
            {
                stack.Push(ancestor);
                ancestor = parent(ancestor);
            }

            return new Path<T>(stack.ToArray());
        }

        public static Path<T> Path<T>(this T self) where T : IHierarchy<T>
        {
            var stack = new Stack<T>();
            stack.Push(self);
            var ancestor = self.Parent;
            while (!Equals(ancestor, default(T)))
            {
                stack.Push(ancestor);
                ancestor = ancestor.Parent;
            }

            return new Path<T>(stack.ToArray());
        }

        public static T Root<T>(this T self) where T : IHierarchy<T>
        {
            var root = self;
            var ancestor = self.Parent;
            while (!Equals(ancestor, default(T)))
            {
                root = ancestor;
                ancestor = ancestor.Parent;
            }

            return root;
        }

        public static string ToTreeString<T>(this IEnumerable<T> self, Func<T, string> toString = null)
            where T : IHierarchy<T>
        {
            var buffer = new StringBuilder();
            foreach (var root in self)
            {
                AppendTreeString(buffer, root, toString);
            }

            return buffer.ToString();
        }

        public static string ToTreeString<T>(this T self, Func<T, string> toString = null) where T : IHierarchy<T>
        {
            var buffer = new StringBuilder();
            AppendTreeString(buffer, self, toString);
            return buffer.ToString();
        }

        private static IEnumerable<T> AncestorsCore<T>(T self, bool andSelf) where T : IHierarchy<T>
        {
            if (andSelf)
            {
                yield return self;
            }

            var ancestor = self.Parent;
            while (!Equals(ancestor, default(T)))
            {
                yield return ancestor;

                ancestor = ancestor.Parent;
            }
        }

        private static void AppendTreeString<T>(StringBuilder buffer, T self, Func<T, string> toString) where T : IHierarchy<T>
        {
            if (!typeof(T).IsValueType && Equals(self, default(T)))
            {
                buffer.AppendLine("┬─ <null>");
                return;
            }

            string displayString = toString == null ? Convert.ToString(self) : toString(self);

            buffer.Append("┬ ").AppendLine(displayString);

            if (self.Children == null) return;

            var stack = new Stack<IEnumerator<T>>();
            var en = self.Children.GetEnumerator();
            do
            {
                if (en.MoveNext())
                {
                    if (!typeof(T).IsValueType && Equals(en.Current, default(T)))
                    {
                        buffer.Append("│".Repeat(stack.Count)).AppendLine("├─ <null>");
                        continue;
                    }

                    displayString = toString == null ? Convert.ToString(en.Current) : toString(en.Current);

                    buffer.Append("│".Repeat(stack.Count));
                    buffer.Append("├")
                        .Append(en.Current.IsLeaf() ? "─ " : "┬ ")
                        .AppendLine(displayString);

                    if (en.Current.Children == null) continue;

                    stack.Push(en);
                    en = en.Current.Children.GetEnumerator();
                }
                else
                {
                    if (0 < stack.Count)
                    {
                        en = stack.Pop();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            while (true);
        }

        private static IEnumerable<T> DescendantsCore<T>(T self, bool andSelf) where T : IHierarchy<T>
        {
            if (andSelf)
            {
                yield return self;
            }

            if (self.Children == null) yield break;

            var stack = new Stack<IEnumerator<T>>();
            var en = self.Children.GetEnumerator();
            do
            {
                if (en.MoveNext())
                {
                    yield return en.Current;

                    if (en.Current.Children == null) continue;

                    stack.Push(en);
                    en = en.Current.Children.GetEnumerator();
                }
                else if (0 < stack.Count)
                {
                    en = stack.Pop();
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        private static IEnumerable<T> HierarchizeCore<T>(ILookup<T, T> lookup) where T : class
        {
            var roots = lookup[null];
            var en = roots.GetEnumerator();
            var levels = new Stack<IEnumerator<T>>();
            do
            {
                if (en.MoveNext())
                {
                    yield return en.Current;

                    levels.Push(en);
                    en = lookup[en.Current].GetEnumerator();
                }
                else if (0 < levels.Count)
                {
                    en = levels.Pop();
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        private static bool IsAncestorsCore<T>(T self, T other, bool orSelf) where T : IHierarchy<T>
        {
            if (ReferenceEquals(other, null)) return orSelf && ReferenceEquals(self, null);

            var ancestor = orSelf ? other : other.Parent;
            while (!ReferenceEquals(ancestor, default(T)))
            {
                if (ReferenceEquals(ancestor, self)) return true;

                ancestor = ancestor.Parent;
            }

            return false;
        }
    }
}
