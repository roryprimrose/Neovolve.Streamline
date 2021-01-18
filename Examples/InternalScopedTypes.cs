namespace Examples
{
    using System;
    using Examples.External;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    // As InternalClass and IInternalScope in this example are declared as internal in a different project, we need to do three things:
    // 1. The project that declares the internal types to be tested needs to include [assembly:InternalsVisibleTo("<THIS_PROJECT>")]
    //      so that this test project can reference the types
    // 2. Create an internal class declared once in the test project that can be the proxy to the Test<T> that points to an internal type
    // 3. xUnit test classes need to be public and therefore cannot inherit from Test<T> where T is declared as internal.
    //      We can get around this by using the TestProxy class that is then declared as a field in the test class rather than the test class inheriting from Test<T>
    //      This idea comes from https://pedro.digitaldias.com/?p=494

    // As per #2 above, declare this once in your test project
    internal class TestProxy<T> : Tests<T> where T : class
    {
    }

    public class InternalScopedTypes
    {
        // As per #3 above, declare the proxy instance that the unit test methods can reference
        private readonly TestProxy<InternalClass> _proxy = new TestProxy<InternalClass>();

        [Fact]
        public void CanCreateAndUseInternalTypes()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            // Proxy is used here in a public xUnit test class in order to run against the Test<T> that points to an internal type
            _proxy.Service<IInternalScope>().GetValue(id).Returns(expected);

            var actual = _proxy.SUT.GetValue(id);

            actual.Should().Be(expected);
        }
    }
}