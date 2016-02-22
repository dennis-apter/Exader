using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Exader.Reflection
{
    public class TypeExtensionsTest
    {
        [Fact]
        public void IsGenericOf()
        {
            Assert.True(typeof(GenericList).IsGenericOf(typeof(List<>)));
            Assert.True(typeof(List<string>).IsGenericOf(typeof(List<>)));
            Assert.True(typeof(List<string>).IsGenericOf(typeof(IList<>)));
            Assert.True(typeof(List<string>).IsGenericOf(typeof(ICollection<>)));
            Assert.True(typeof(List<string>).IsGenericOf(typeof(IEnumerable<>)));
        }

        [Fact]
        public void GenericOf()
        {
            Assert.Equal(typeof(Collection<string>), typeof(ObservableCollection<string>).GenericOf(typeof(Collection<>)));
        }

        [Fact]
        public void IsGenericInterfaceOf()
        {
            Assert.True(typeof(IList<string>).IsGenericOf(typeof(IList<>)));
            Assert.True(typeof(IList<string>).IsGenericOf(typeof(ICollection<>)));
            Assert.True(typeof(IList<string>).IsGenericOf(typeof(IEnumerable<>)));
        }

        [Fact]
        public void GenericInterfaceOf()
        {
            Assert.Equal(typeof(ICollection<string>), typeof(IList<string>).GenericOf(typeof(ICollection<>)));
            Assert.Equal(typeof(IEnumerable<string>), typeof(IList<string>).GenericOf(typeof(IEnumerable<>)));
        }

        [Fact]
        public void IsNullable()
        {
            Assert.False(typeof(int).IsNullable());
            Assert.True(typeof(int?).IsNullable());

            Assert.Equal(typeof(int), typeof(int).GetNonNullable());
            Assert.Equal(typeof(int), typeof(int?).GetNonNullable());

            Assert.False(typeof(string).IsNullable());
        }

        //[Fact]
        public void GetUsedOwnProperties()
        {
            var method = typeof(Name).GetProperty("Full").GetGetMethod();

            var ownProperties = method.GetUsedOwnProperties().ToList();
            Assert.Equal(4, ownProperties.Count);

            var propertyNames = ownProperties.Select(p => p.Name).ToList();

            var exception = propertyNames.Except(
                new[] { "DefaultPrefix", "First", "Middle", "Last" });

            Assert.False(exception.Any());
        }

        private class Name
        {
            public string DefaultPrefix { get; set; }

            public string Full
            {
                get
                {
                    if (string.IsNullOrEmpty(DefaultPrefix))
                    {
                        return string.Format("{0} {1} {2}", First, Middle, Last);
                    }

                    return string.Format("{0} {1} {2} {3}", DefaultPrefix, First, Middle, Last);
                }
            }

            public string First { get; set; }

            public string Middle { get; set; }

            public string Last { get; set; }
        }

        private sealed class GenericList : List<string> { }
    }
}
