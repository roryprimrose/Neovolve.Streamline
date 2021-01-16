namespace Examples.Source.UnitTests
{
    using System;
    using FluentAssertions;
    using Neovolve.Streamline.NSubstitute;
    using NSubstitute;
    using Xunit;

    public class SingleParameterTests : Test<SingleParameter>
    {
        [Fact]
        public void Test1()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            Service<IDoSomething>().DoSomething(id).Returns(expected);

            var actual = SUT.Run(id);

            actual.Should().Be(expected);
        }
    }
}