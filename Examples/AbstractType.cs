namespace Examples;

using System;
using FluentAssertions;
using NSubstitute;
using Xunit;
    
public abstract class AbstractType
{
    public string GetValue(Guid id)
    {
        return GetCustomValue(id);
    }

    public abstract string GetCustomValue(Guid id);
}

public class AbstractTypeTests : TestsSubstituteOf<AbstractType>
{
    [Fact]
    public void CanUseServiceOnMethod()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();
            
        SUT.GetCustomValue(id).Returns(expected);

        var actual = SUT.GetValue(id);

        actual.Should().Be(expected);
    }
}