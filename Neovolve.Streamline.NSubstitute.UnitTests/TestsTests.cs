namespace Neovolve.Streamline.NSubstitute.UnitTests;

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
    public void CanCreateSUTWithVirtualMembers()
    {
        // In this test we are proving that using Tests<T> returns an instance of T rather than a substitute of T
        var expected = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();

        var wrapper = new TestsWrapper<TargetWithVirtual>();

        wrapper.Service<ITargetService>().GetValue(id).Returns(expected);

        wrapper.SUT.Should().NotBeNull();
        wrapper.SUT.Service.Should().NotBeNull();

        var actual = wrapper.SUT.RunTest(id);

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

    private class TargetWithVirtual
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        public TargetWithVirtual(ITargetService service)
        {
            Service = service;
        }

        public virtual string RunTest(Guid id)
        {
            var service = Service ?? throw new InvalidOperationException("No service defined");

            return service.GetValue(id);
        }

        public ITargetService? Service { get; }
    }
}