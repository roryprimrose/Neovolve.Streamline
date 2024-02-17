namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;
using FluentAssertions;
using global::NSubstitute;
using Xunit;

public class TestsPartOfInternalTests
{
    [Fact]
    public void CanPartialMockSUT()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new TestsPartOfInternalWrapper<TypeWithVirtual>();

        wrapper.SUT.GetValueEx(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }
}