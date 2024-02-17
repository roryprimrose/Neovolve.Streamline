namespace Neovolve.Streamline.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class TestsInternalTests
    {
        [Fact]
        public void CanCreateWithConstructorProvidedService()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            var service = Substitute.For<ITargetService>();

            service.GetValue(id).Returns(expected);

            var wrapper = new InternalWrapper(service);

            var actual = wrapper.SUT.GetValue(id);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CanCreateWithNoProvidedServices()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            var wrapper = new InternalWrapper();

            wrapper.Service<ITargetService>().GetValue(id).Returns(expected);

            var actual = wrapper.SUT.GetValue(id);

            actual.Should().Be(expected);
        }
    }
}