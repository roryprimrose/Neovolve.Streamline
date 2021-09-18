namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using System;
    using FluentAssertions;
    using global::NSubstitute;
    using Xunit;

    public class TestsSubstituteOfTests
    {
        [Fact]
        public void CanSubstituteVirtualMethod()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            var wrapper = new TestsSubstituteOfWrapper<TypeWithDependency>();

            wrapper.SUT.GetValueFromService(id).Returns(expected);

            var actual = wrapper.SUT.GetValue(id);

            actual.Should().Be(expected);
        }
    }
}