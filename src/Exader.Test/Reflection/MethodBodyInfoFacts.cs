using System.Diagnostics;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Exader.Reflection
{
    public class MethodBodyInfoFacts
    {
        private ITestOutputHelper _output;

        public MethodBodyInfoFacts(ITestOutputHelper output)
        {
            _output = output;
        }

        //[Fact]
        public void NotTaggedAsPure()
        {
            const BindingFlags bf = BindingFlags.DeclaredOnly
                | BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.Instance;

            foreach (var type in typeof(StringExtensions).Assembly.GetTypes())
            {
                if (type.IsNotPublic)
                    continue;

                foreach (var method in type.GetMethods(bf))
                {
                    if (method.IsAbstract || method.ReturnType == typeof(void))
                        continue;

                    bool isTaggedAsPure = false;
                    foreach (var customAttribute in method.CustomAttributes)
                    {
                        if (customAttribute.AttributeType.Name == "PureAttribute")
                        {
                            isTaggedAsPure = true;
                            break;
                        }
                    }

                    var body = method.GetMethodBodyInfo();
                    if (body.ChangesState())
                    {
                        if (isTaggedAsPure)
                            _output.WriteLine($"[NotPure] {type.GetTypeName()}.{method.GetMethodName()}");
                    }
                    else
                    {
                        if (!isTaggedAsPure)
                            _output.WriteLine($"[Pure] {type.GetTypeName()}.{method.GetMethodName()}");
                    }
                }
            }
        }

        [Fact]
        public void ChangesState()
        {
            var type = typeof(State);

            var changeObjectStateMethod = type.GetMethodBodyInfo(nameof(State.ChangeObjectState));
            var changeObjectStateByRefMethod = type.GetMethodBodyInfo(nameof(State.ChangeObjectStateByRef));
            var changeObjectStateByRef2Method = type.GetMethodBodyInfo(nameof(State.ChangeObjectStateByRef2));

            var changeTypeStateMethod = type.GetMethodBodyInfo(nameof(State.ChangeTypeState));
            var changeTypeStateByRefMethod = type.GetMethodBodyInfo(nameof(State.ChangeTypeStateByRef));
            var changeTypeStateByRef2Method = type.GetMethodBodyInfo(nameof(State.ChangeTypeStateByRef2));

            Assert.True(changeObjectStateMethod.ChangesObjectState());
            Assert.False(changeObjectStateByRefMethod.ChangesObjectState());
            Assert.False(changeObjectStateByRef2Method.ChangesObjectState());
            Assert.True(changeObjectStateByRefMethod.ChangesObjectState(true));
            Assert.True(changeObjectStateByRef2Method.ChangesObjectState(true));

            Assert.True(changeTypeStateMethod.ChangesTypeState());
            Assert.False(changeTypeStateByRefMethod.ChangesTypeState());
            Assert.False(changeTypeStateByRef2Method.ChangesTypeState());
            Assert.True(changeTypeStateByRefMethod.ChangesTypeState(true));
            Assert.True(changeTypeStateByRef2Method.ChangesTypeState(true));
        }

        private class State
        {
            private static int TypeState;
            private static string TypeState2;

            private int _objectState;
            private string _objectState2;

            public void ChangeObjectState() => _objectState = 1;
            public void ChangeObjectStateByRef() => ChangeStateByRef(ref _objectState);
            public void ChangeObjectStateByRef2() => ChangeStateByRef(_objectState, ref _objectState2);

            public static void ChangeTypeState() => TypeState = 1;
            public static void ChangeTypeStateByRef() => ChangeStateByRef(ref TypeState);
            public static void ChangeTypeStateByRef2() => ChangeStateByRef(TypeState, ref TypeState2);

            private static void ChangeStateByRef(ref int state) => state = 1;
            private static void ChangeStateByRef(int state, ref string s) => s = "";
        }
    }
}
