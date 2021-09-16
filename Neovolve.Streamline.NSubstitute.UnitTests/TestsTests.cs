namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using System;
    using FluentAssertions;
    using global::NSubstitute;
    using Xunit;

    public class TestsTests
    {
        [Fact]
        public void CanCreateMissingServices()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            var wrapper = new TestsWrapper<TypeWithDependency>();

            wrapper.Service<ITargetService>().GetValue(id).Returns(expected);

            var actual = wrapper.SUT.GetValue(id);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CanCreateWithConstructorProvidedService()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            var service = Substitute.For<ITargetService>();

            service.GetValue(id).Returns(expected);

            var wrapper = new TestsWrapper<TypeWithDependency>(service);

            var actual = wrapper.SUT.GetValue(id);

            actual.Should().Be(expected);
        }
    }
}