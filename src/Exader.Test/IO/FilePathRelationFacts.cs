using Xunit;

namespace Exader.IO
{
    public class FilePathRelationFacts
    {
        [Theory]
        [InlineData(@"C:\D\F.E", @"c:\d\f.e")]
        [InlineData(@"C:\D\F.E", @"c:/d/f.e")]
        [InlineData(@"C:\D\F.E", @"c://d//f.e")]
        [InlineData(@"C:\D\F.E", @"c://d//f.e")]
        [InlineData(@"C:\D\F.E", @"file:c://d//f.e")]
        [InlineData(@"C:\D\F.E", @"file://localhost/c://d//f.e")]
        [InlineData(@"C:\D\F.E", @"//?/c://d//f.e")]
        public void Equality(string a, string b)
        {
            Assert.Equal(FilePath.Parse(a), FilePath.Parse(b));
        }

        [Theory]
        [InlineData("a/b/c", "A")]
        [InlineData("a/b/c", "B")]
        [InlineData("a/b/c", "A/B")]
        public void Hierarchy(string d, string a)
        {
            var descendant = FilePath.Parse(d);
            var ancestor = FilePath.Parse(a);

            Assert.True(descendant.IsDescendantOf(ancestor));
            Assert.True(ancestor.IsAncestorOf(descendant));

            Assert.False(descendant.IsAncestorOf(ancestor));
            Assert.False(ancestor.IsDescendantOf(descendant));
        }

        [Theory]
        [InlineData("a/b/", "a/c/", FilePathRelation.CommonAncestor)]
        [InlineData("ab/bc/", "ab/cd/", FilePathRelation.CommonAncestor)]
        [InlineData("ab/bc/f", "ab/cd/f", FilePathRelation.CommonAncestor)]
        [InlineData("ab/bc/f.e", "ab/cd/f.e", FilePathRelation.CommonAncestor)]
        [InlineData("a/", "b/c/", FilePathRelation.Sibling)]
        [InlineData("ab/", "bc/cd/", FilePathRelation.Sibling)]
        [InlineData("ab/f", "bc/cd/f", FilePathRelation.Sibling)]
        [InlineData("ab/f.e", "bc/cd/f.e", FilePathRelation.Sibling)]
        [InlineData("a/b/", "a/b/", FilePathRelation.Equal)]
        [InlineData("a/B/", "A/b/", FilePathRelation.Equal)]
        [InlineData("ab/bc/", "ab/bc/", FilePathRelation.Equal)]
        [InlineData("AB/BC/", "ab/bc/", FilePathRelation.Equal)]
        [InlineData("AB/BC/f", "ab/bc/f", FilePathRelation.Equal)]
        [InlineData("AB/BC/f.e", "ab/bc/f.e", FilePathRelation.Equal)]
        [InlineData("p/c/", "p/", FilePathRelation.Child)]
        [InlineData("p/f", "p/", FilePathRelation.Child)]
        [InlineData("p/f.e", "p/", FilePathRelation.Child)]
        [InlineData("parent/child/", "parent/", FilePathRelation.Child)]
        [InlineData("p/", "p/c/", FilePathRelation.Parent)]
        [InlineData("p/", "p/f", FilePathRelation.Parent)]
        [InlineData("p/", "p/f.e", FilePathRelation.Parent)]
        [InlineData("parent/", "parent/child/", FilePathRelation.Parent)]
        [InlineData("i/", "", FilePathRelation.ImplicitChild)]
        [InlineData("i/f", "", FilePathRelation.ImplicitChild)]
        [InlineData("i/f.e", "", FilePathRelation.ImplicitChild)]
        [InlineData("", "i/", FilePathRelation.ImplicitParent)]
        [InlineData("", "i/f", FilePathRelation.ImplicitParent)]
        [InlineData("", "i/f.e", FilePathRelation.ImplicitParent)]
        public void TryRelateTo(string x, string y, FilePathRelation r)
        {
            var xfp = FilePath.Parse(x);
            var yfp = FilePath.Parse(y);

            FilePathRelation relation;
            Assert.True(xfp.TryRelateTo(yfp, out relation));
            Assert.Equal(r, relation);
        }
    }
}
