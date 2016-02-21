using Xunit;

namespace Exader
{
	public partial class StringExtensionsFacts
	{
		public class TheBetweenMethods
		{
			[Fact]
			public static void Between()
			{
				Assert.Equal("B", "A>>B>>C".Between(">>"));
				Assert.Equal("B", "A<<B>>C".Between("<<", ">>"));
				Assert.Equal(string.Empty, "A<<>>C".Between("<<", ">>"));

				Assert.Equal("B", "A>B>C".Between('>'));
				Assert.Equal("B", "A<B>C".Between('<', '>'));
				Assert.Equal(string.Empty, "A<>C".Between('<', '>'));
			}

			[Fact(Skip = "TODO")]
			public static void BetweenLast()
			{
				/*
                Assert.Equal("B", "A>>B>>C".BetweenLast(">>"));
				Assert.Equal("B", "A<<B>>C".BetweenLast("<<", ">>"));
				Assert.Equal("B", "A<<B>>C".BetweenLast("<<", ">>"));
				Assert.Equal(string.Empty, "A<<>>C".BetweenLast("<<", ">>"));

				Assert.Equal("B", "A>B>C".BetweenLast('>'));
				Assert.Equal("B", "A<B>C".BetweenLast('<', '>'));
				Assert.Equal(string.Empty, "A<>C".BetweenLast('<', '>'));
                */
			}

			[Fact]
			public static void BetweenOrSelf()
			{
				Assert.Equal("A<<>>C", "A<<>>C".BetweenOrSelf("<<", ">>"));
				Assert.Equal("A<<>>C", "A<<>>C".BetweenOrSelf("<<", ">>", true));

				Assert.Equal("A<>C", "A<>C".BetweenOrSelf('<', '>'));
				Assert.Equal("A<>C", "A<>C".BetweenOrSelf('<', '>', true));
			}

			[Fact]
			public static void BetweenWithInclude()
			{
				Assert.Equal(">>B>>", "A>>B>>C".Between(">>", true));
				Assert.Equal("<<B>>", "A<<B>>C".Between("<<", ">>", true));
				Assert.Equal("<<>>", "A<<>>C".Between("<<", ">>", true));

				Assert.Equal(">B>", "A>B>C".Between('>', true));
				Assert.Equal("<B>", "A<B>C".Between('<', '>', true));
				Assert.Equal("<>", "A<>C".Between('<', '>', true));
			}

			[Fact]
			public static void BetweenWithOuts()
			{
				string left, right;

				Assert.Equal("B", "A<<B>>C".Between("<<", ">>", out left, out right));
				Assert.Equal("A", left);
				Assert.Equal("C", right);

				Assert.Equal("B", "A<B>C".Between('<', '>', out left, out right));
				Assert.Equal("A", left);
				Assert.Equal("C", right);
			}

			[Fact]
			public static void NotBetween()
			{
				Assert.Equal(string.Empty, "B>>C".Between("<<", ">>"));
				Assert.Equal(string.Empty, "A<<B".Between("<<", ">>"));
				Assert.Equal(string.Empty, "A>>B<<C".Between("<<", ">>"));

				Assert.Equal(string.Empty, "B>C".Between('<', '>'));
				Assert.Equal(string.Empty, "A<B".Between('<', '>'));
				Assert.Equal(string.Empty, "A>B<C".Between('<', '>'));
			}

			[Fact]
			public static void NotBetweenWithOuts()
			{
				string left, right;

				Assert.Equal(string.Empty, "A>>B<<C".Between("<<", ">>", out left, out right));
				Assert.Equal(string.Empty, left);
				Assert.Equal(string.Empty, right);

				Assert.Equal(string.Empty, "D<<E".Between("<<", ">>", out left, out right));
				Assert.Equal(string.Empty, left);
				Assert.Equal(string.Empty, right);

				Assert.Equal(string.Empty, "F>>G".Between("<<", ">>", out left, out right));
				Assert.Equal(string.Empty, left);
				Assert.Equal(string.Empty, right);

				Assert.Equal(string.Empty, "A>B<C".Between('<', '>', out left, out right));
				Assert.Equal(string.Empty, left);
				Assert.Equal(string.Empty, right);

				Assert.Equal(string.Empty, "D<E".Between('<', '>', out left, out right));
				Assert.Equal(string.Empty, left);
				Assert.Equal(string.Empty, right);

				Assert.Equal(string.Empty, "F>G".Between('<', '>', out left, out right));
				Assert.Equal(string.Empty, left);
				Assert.Equal(string.Empty, right);
			}
		}
	}
}
