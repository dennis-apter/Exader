using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;

namespace Exader.IO
{
    public class FilePathPatternTests
    {
        [Fact]
        public void Test1()
        {
            var regex = Create("Images/*.jpg");

            Assert.True(regex.IsMatch(@"Images\foo.jpg"));
            Assert.False(regex.IsMatch(@"Images\M\foo.jpg"));
            Assert.False(regex.IsMatch(@"R\Images\foo.jpg"));
        }

        [Fact]
        public void Test2()
        {
            var regex = Create("Images/**/*.jpg");

            Assert.True(regex.IsMatch(@"Images\foo.jpg"));
            Assert.True(regex.IsMatch(@"Images\M\foo.jpg"));
            Assert.False(regex.IsMatch(@"R\Images\foo.jpg"));
            Assert.False(regex.IsMatch(@"R\Images\M\foo.jpg"));
        }

        [Fact]
        public void Test3()
        {
            var regex = Create("**/Images/**/*.jpg");

            Assert.False(regex.IsMatch(@"Images\foo.jpg"));
            Assert.False(regex.IsMatch(@"Images\M\foo.jpg"));
            Assert.True(regex.IsMatch(@"R\Images\foo.jpg"));
            Assert.True(regex.IsMatch(@"R\Images\M\foo.jpg"));
        }

        private Regex Create(string items)
        {
            var groups = new List<string>();
            foreach (var item in items.SplitAndRemoveEmpties(";"))
            {
                var exclude = item.StartsWith('-') ? "?!" : string.Empty;
                var path = FilePath.Parse(item.TrimStart('-', '+'));
                var root = !path.IsAbsolute
                    ? (path.HasRootFolder ? @"\\" : string.Empty)
                    : Regex.Escape(path.DriveOrHost);
                var partialPath = Regex.Escape(path.WithoutNameAsString())
                    .Replace(@"\\\*\*\\", @"\\(.*)")
                    .Replace(@"\*\*\\", @"(.*)\\")
                    .Replace(@"\*", @"([^\\]*)")
                    .Replace(@"\?", @"(.)");
                var name = Regex.Escape(path.Name)
                    .Replace(@"\*", @"([^\\]*)")
                    .Replace(@"\?", @"(.)");

                groups.Add(exclude + root + partialPath + name);
            }

            var pattern = "^(" + string.Join(")|(", groups) + ")$";
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}