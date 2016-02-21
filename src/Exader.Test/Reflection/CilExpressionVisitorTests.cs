namespace Exader.Reflection
{
  using System;
  using System.Linq.Expressions;
  using NUnit.Framework;

  [TestFixture]
  public class CilExpressionVisitorTests
  {
    #region Tests

    [Test]
    public void ParametrizedInstanceMethodAccess()
    {
      var visitor = new CilExpressionVisitor(StrongMethod.Of((MyClass e, CilExpressionVisitorTests p) => e.ParametrizedInstanceMethodAccess(p)));
      Expression expression = visitor.Translate();

      Assert.IsNotNull(expression);
      Assert.AreEqual("tests.ParametrizedInstanceMethodAccess()", expression.ToString());
    }

    [Test]
    public void RecursiveInstanceMethodAccess()
    {
      var visitor = new CilExpressionVisitor(StrongMethod.Of((MyClass e, CilExpressionVisitorTests p) => e.RecursiveInstanceMethodAccess(p)));
      Expression expression = visitor.Translate();

      Assert.IsNotNull(expression);
      Assert.AreEqual("this.RecursiveInstanceMethodAccess(tests)", expression.ToString());
    }

    [Test]
    public void IfThen()
    {
      string debugView = @"
      .Block() {
          $loc1 = True == .Call System.String.op_Equality(
              ""MyClass"",
              .Call $this.ToString());
          .If (
              .IsTrue($loc1)
          ) {
              $loc0 = (System.Object)((System.Int32).Call $this.GetHashCode())
          } .Else {
              .Block() {
                  $loc1 = True == .Call $this.Equals(null);
                  .If (
                      .IsTrue($loc1)
                  ) {
                      $loc0 = (System.Object)((System.Boolean).Call $this.Equals($this))
                  } .Else {
                      $loc0 = (System.Object).Call $this.GetType()
                  }
              }
          };
          $loc0
      }";

      debugView = debugView.Unindent(3, ' ', 2).Trim();

      var visitor = new CilExpressionVisitor(StrongMethod.Of((MyClass e) => e.IfThen()));
      var expression = visitor.Translate();

      Assert.IsNotNull(expression);
      Assert.AreEqual(debugView, expression.GetNonPublicPropertyValue("DebugView"));
    }

    [Test]
    public void Test1()
    {
      var visitor = new CilExpressionVisitor(StrongMethod.Of(() => MyClass.StaticPropertyAccess()));
      Expression expression = visitor.Translate();

      Assert.IsNotNull(expression);
      Assert.AreEqual("get_OSVersion().ToString()", expression.ToString());
    }

    #endregion

    #region Others

    public class MyClass
    {
      #region Methods

      public void ParametrizedInstanceMethodAccess(CilExpressionVisitorTests tests)
      {
        tests.ParametrizedInstanceMethodAccess();
      }

      public void RecursiveInstanceMethodAccess(CilExpressionVisitorTests tests)
      {
        this.RecursiveInstanceMethodAccess(tests);
      }

      #endregion

      #region Static Members

      public static void StaticPropertyAccess()
      {
        Environment.OSVersion.ToString();
      }

      #endregion

      public object IfThen()
      {
        if (this.ToString() == "MyClass")
        {
          return this.GetHashCode();
        }
        else if (this.Equals(null))
        {
          return this.Equals(this);
        }
        else
        {
          return this.GetType();
        }
      }
    }

    #endregion
  }
}
