using Xunit;

namespace Exader
{
    public class SubstringBetweenTest
    {
        [Fact]
        public static void Between()
        {
            Assert.Equal("B", "A>>B>>C".Between(">>"));
            Assert.Equal("B", "A>>B>>C>>D".Between(">>"));
            Assert.Equal("B", "A<<B>>C".Between("<<", ">>"));
            Assert.Equal("B", "A<<B>>C<<D>>".Between("<<", ">>"));
            Assert.Equal("", "A<<>>C".Between("<<", ">>"));

            Assert.Equal("B", "A>B>C".Between('>'));
            Assert.Equal("B", "A>B>C>D".Between('>'));
            Assert.Equal("B", "A<B>C".Between('<', '>'));
            Assert.Equal("B", "A<B>C<D>".Between('<', '>'));
            Assert.Equal("", "A<>C".Between('<', '>'));
        }

        [Fact]
        public static void Between_Inclusive()
        {
            Assert.Equal(">>B>>", "A>>B>>C".Between(">>", true));
            Assert.Equal("<<B>>", "A<<B>>C".Between("<<", ">>", true));
            Assert.Equal("<<>>", "A<<>>C".Between("<<", ">>", true));

            Assert.Equal(">B>", "A>B>C".Between('>', true));
            Assert.Equal("<B>", "A<B>C".Between('<', '>', true));
            Assert.Equal("<>", "A<>C".Between('<', '>', true));
        }

        [Fact]
        public static void Between_Outs()
        {
            string left, right;

            Assert.Equal("B", "A<<B>>C".Between("<<", ">>", out left, out right));
            Assert.Equal("A<<", left);
            Assert.Equal(">>C", right);

            Assert.Equal("B", "A>>B>>C".Between(">>", ">>", out left, out right));
            Assert.Equal("A>>", left);
            Assert.Equal(">>C", right);

            Assert.Equal("B", "A<B>C".Between('<', '>', out left, out right));
            Assert.Equal("A<", left);
            Assert.Equal(">C", right);

            Assert.Equal("B", "A>B>C".Between('>', '>', out left, out right));
            Assert.Equal("A>", left);
            Assert.Equal(">C", right);
            //
            Assert.Equal("B", "A<<B>>C<<D>>".Between("<<", ">>", out left, out right));
            Assert.Equal("A<<", left);
            Assert.Equal(">>C<<D>>", right);

            Assert.Equal("B", "A>>B>>C>>D".Between(">>", ">>", out left, out right));
            Assert.Equal("A>>", left);
            Assert.Equal(">>C>>D", right);

            Assert.Equal("B", "A<B>C<D>".Between('<', '>', out left, out right));
            Assert.Equal("A<", left);
            Assert.Equal(">C<D>", right);

            Assert.Equal("B", "A>B>C>D".Between('>', '>', out left, out right));
            Assert.Equal("A>", left);
            Assert.Equal(">C>D", right);
            //
            Assert.Equal("", "A<<>>C".Between("<<", ">>", out left, out right));
            Assert.Equal("A<<", left);
            Assert.Equal(">>C", right);

            Assert.Equal("", "A>>>>C".Between(">>", ">>", out left, out right));
            Assert.Equal("A>>", left);
            Assert.Equal(">>C", right);

            Assert.Equal("", "A<>C".Between('<', '>', out left, out right));
            Assert.Equal("A<", left);
            Assert.Equal(">C", right);

            Assert.Equal("", "A>>C".Between('>', '>', out left, out right));
            Assert.Equal("A>", left);
            Assert.Equal(">C", right);
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
        public static void BetweenLast()
        {
            Assert.Equal("B", "A>>B>>C".BetweenLast(">>"));
            Assert.Equal("C", "A>>B>>C>>D".BetweenLast(">>"));
            Assert.Equal("B", "A<<B>>C".BetweenLast("<<", ">>"));
            Assert.Equal("D", "A<<B>>C<<D>>".BetweenLast("<<", ">>"));
            Assert.Equal("", "A<<>>C".BetweenLast("<<", ">>"));

            Assert.Equal("B", "A>B>C".BetweenLast('>'));
            Assert.Equal("C", "A>B>C>D".BetweenLast('>'));
            Assert.Equal("B", "A<B>C".BetweenLast('<', '>'));
            Assert.Equal("D", "A<B>C<D>".BetweenLast('<', '>'));
            Assert.Equal("", "A<>C".BetweenLast('<', '>'));
        }

        [Fact]
        public static void BetweenLast_Inclusive()
        {
            Assert.Equal(">>B>>", "A>>B>>C".BetweenLast(">>", true));
            Assert.Equal("<<B>>", "A<<B>>C".BetweenLast("<<", ">>", true));
            Assert.Equal("", "A<<>>C".BetweenLast("<<", ">>", true));

            Assert.Equal(">B>", "A>B>C".BetweenLast('>', true));
            Assert.Equal("<B>", "A<B>C".BetweenLast('<', '>', true));
            Assert.Equal("", "A<>C".BetweenLast('<', '>', true));
        }

        [Fact]
        public static void BetweenLast_Outs()
        {
            string left, right;

            Assert.Equal("B", "A<<B>>C".BetweenLast("<<", ">>", out left, out right));
            Assert.Equal("A<<", left);
            Assert.Equal(">>C", right);

            Assert.Equal("B", "A>>B>>C".BetweenLast(">>", ">>", out left, out right));
            Assert.Equal("A>>", left);
            Assert.Equal(">>C", right);

            Assert.Equal("B", "A<B>C".BetweenLast('<', '>', out left, out right));
            Assert.Equal("A<", left);
            Assert.Equal(">C", right);

            Assert.Equal("B", "A>B>C".BetweenLast('>', '>', out left, out right));
            Assert.Equal("A>", left);
            Assert.Equal(">C", right);
            //
            Assert.Equal("D", "A<<B>>C<<D>>".BetweenLast("<<", ">>", out left, out right));
            Assert.Equal("A<<B>>C<<", left);
            Assert.Equal(">>", right);

            Assert.Equal("C", "A>>B>>C>>D".BetweenLast(">>", ">>", out left, out right));
            Assert.Equal("A>>B>>", left);
            Assert.Equal(">>D", right);

            Assert.Equal("D", "A<B>C<D>".BetweenLast('<', '>', out left, out right));
            Assert.Equal("A<B>C<", left);
            Assert.Equal(">", right);

            Assert.Equal("C", "A>B>C>D".BetweenLast('>', '>', out left, out right));
            Assert.Equal("A>B>", left);
            Assert.Equal(">D", right);
            //
            Assert.Equal("", "A<<>>C".BetweenLast("<<", ">>", out left, out right));
            Assert.Equal("A<<", left);
            Assert.Equal(">>C", right);

            Assert.Equal("", "A>>>>C".BetweenLast(">>", ">>", out left, out right));
            Assert.Equal("A>>", left);
            Assert.Equal(">>C", right);

            Assert.Equal("", "A<>C".BetweenLast('<', '>', out left, out right));
            Assert.Equal("A<", left);
            Assert.Equal(">C", right);

            Assert.Equal("", "A>>C".BetweenLast('>', '>', out left, out right));
            Assert.Equal("A>", left);
            Assert.Equal(">C", right);
        }

        [Fact]
        public static void BetweenLastOrSelf()
        {
            Assert.Equal("A<<>>C", "A<<>>C".BetweenLastOrSelf("<<", ">>"));
            Assert.Equal("A<<>>C", "A<<>>C".BetweenLastOrSelf("<<", ">>", true));

            Assert.Equal("A<>C", "A<>C".BetweenLastOrSelf('<', '>'));
            Assert.Equal("A<>C", "A<>C".BetweenLastOrSelf('<', '>', true));
        }

        [Fact]
        public static void NotBetween()
        {
            Assert.Equal("", "B>>C".Between("<<", ">>"));
            Assert.Equal("", "A<<B".Between("<<", ">>"));
            Assert.Equal("", "A>>B<<C".Between("<<", ">>"));

            Assert.Equal("", "B>C".Between('<', '>'));
            Assert.Equal("", "A<B".Between('<', '>'));
            Assert.Equal("", "A>B<C".Between('<', '>'));
        }

        [Fact]
        public static void NotBetween_Outs()
        {
            string left, right;

            Assert.Equal("", "A>>B<<C".Between("<<", ">>", out left, out right));
            Assert.Equal("", left);
            Assert.Equal("", right);

            Assert.Equal("", "D<<E".Between("<<", ">>", out left, out right));
            Assert.Equal("", left);
            Assert.Equal("", right);

            Assert.Equal("", "F>>G".Between("<<", ">>", out left, out right));
            Assert.Equal("", left);
            Assert.Equal("", right);

            Assert.Equal("", "A>B<C".Between('<', '>', out left, out right));
            Assert.Equal("", left);
            Assert.Equal("", right);

            Assert.Equal("", "D<E".Between('<', '>', out left, out right));
            Assert.Equal("", left);
            Assert.Equal("", right);

            Assert.Equal("", "F>G".Between('<', '>', out left, out right));
            Assert.Equal("", left);
            Assert.Equal("", right);
        }
    }
}
