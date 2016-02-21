namespace Exader.Reflection
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Xml.Serialization;
    using NUnit.Framework;

    /// <summary>
    /// Test for <see cref="CustomAttributeProviderExtensions"/>.
    /// </summary>
    [TestFixture]
    public class CustomAttributeProviderExtensionsTest
    {
        #region Constants

        private const string AbstractionName = "The Abstraction class";

        #endregion

        #region Tests

        [Test]
        public void TestGetCustomAttribute()
        {
            string displayName = typeof(Abstraction).GetCustomAttribute<DisplayNameAttribute>().DisplayName;
            Assert.AreEqual(AbstractionName, displayName);

            Assert.IsNull(typeof(Implementation).GetCustomAttribute<DisplayNameAttribute>());

            displayName = typeof(Implementation).GetCustomAttribute<DisplayNameAttribute>(true).DisplayName;
            Assert.AreEqual(AbstractionName, displayName);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetInvalidCustomAttribute()
        {
            typeof(Abstraction).GetCustomAttribute<XmlIncludeAttribute>();
        }

        #endregion

        #region Others

        private class Implementation : Intermediation
        {
        }

        private abstract class Intermediation : Abstraction
        {
        }

        [DisplayName(AbstractionName)]
        [XmlInclude(typeof(Intermediation))]
        [XmlInclude(typeof(Implementation))]
        private abstract class Abstraction
        {
        }

        #endregion
    }
}