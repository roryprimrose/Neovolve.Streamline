namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;
using FluentAssertions;
using global::NSubstitute;
using Xunit;

public class TestsPartOfTests
{

    [Fact]
    public void CanPartialMockSUT()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new TestsPartOfWrapper<TypeWithVirtual>();

        wrapper.SUT.GetValueEx(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }
}