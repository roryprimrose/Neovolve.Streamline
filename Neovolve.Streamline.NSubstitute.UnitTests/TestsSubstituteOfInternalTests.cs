namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;
using FluentAssertions;
using global::NSubstitute;
using Xunit;

public class TestsSubstituteOfInternalTests
{
    [Fact]
    public void CanSubstituteAbstractTypeWithoutParameters()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new TestsSubstituteOfInternalWrapper<AbstractType>();

        wrapper.SUT.GetCustomValue(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    [Fact]
    public void CanSubstituteAbstractTypeWithParameters()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new TestsSubstituteOfInternalWrapper<AbstractTypeWithDependency>();

        wrapper.SUT.GetValueFromService(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    [Fact]
    public void CanSubstituteTypeWithDefaultConstructor()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new TestsSubstituteOfInternalWrapper<TypeWithDefaultConstructor>();

        wrapper.SUT.GetCustomValue(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    [Fact]
    public void CanSubstituteVirtualMethod()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new TestsSubstituteOfInternalWrapper<TypeWithDependency>();

        wrapper.SUT.GetValueFromService(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }
}