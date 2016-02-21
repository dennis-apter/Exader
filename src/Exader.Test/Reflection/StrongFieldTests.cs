namespace Exader.Reflection
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class StrongFieldTests
    {
        #region Tests

        [Test]
        public void ByDirectAccess()
        {
            FieldInfo field = StrongField.By((Bar o) => o.DirectAccess);
            Assert.IsNotNull(field);
        }

        [Test]
        public void ByDirectAccessWithDefaultValue()
        {
            FieldInfo field = StrongField.By((Bar o) => o.DirectAccessWithDefaultValue);
            Assert.IsNull(field);
        }

        [Test]
        public void ByLateInit()
        {
            FieldInfo field = StrongField.By((Bar o) => o.LateInit);
            Assert.IsNotNull(field);
        }

        [Test]
        public void ByLateInitWithFlag()
        {
            FieldInfo field = StrongField.By((Bar o) => o.LateInitWithFlag);
            Assert.IsNotNull(field);
            Assert.AreEqual("foo", field.Name);
        }

        [Test]
        public void OfDirectAccess()
        {
            FieldInfo field = StrongField.Of((Bar o) => o.DirectAccess);
            Assert.IsNotNull(field);
        }

        [Test]
        public void OfDirectAccessWithDefaultValue()
        {
            FieldInfo field = StrongField.Of((Bar o) => o.DirectAccessWithDefaultValue);
            Assert.IsNull(field);
        }

        [Test]
        public void OfLateInit()
        {
            FieldInfo field = StrongField.Of((Bar o) => o.LateInit);
            Assert.IsNotNull(field);
        }

        [Test]
        public void OfLateInitWithFlag()
        {
            FieldInfo field = StrongField.Of((Bar o) => o.LateInitWithFlag);
            Assert.IsNotNull(field);
            Assert.AreEqual("foo", field.Name);
        }

        #endregion

        #region Others

        class Bar
        {
            #region Fields

            private string foo;

            private bool inited;

            #endregion

            #region Properties

            public string DirectAccess
            {
                get { return this.foo; }
            }

            public string DirectAccessWithDefaultValue
            {
                get { return this.foo ?? defaultFoo; }
            }

            public string LateInit
            {
                get { return this.foo ?? (this.foo = "LateInit"); }
            }

            public string LateInitWithFlag
            {
                get
                {
                    if (!this.inited)
                    {
                        this.foo = "LateInitWithFlag";
                        this.inited = true;
                    }

                    return this.foo;
                }
            }

            #endregion

            #region Static Members

            private static string defaultFoo = "Foo";

            #endregion
        }

        #endregion
    }
}
