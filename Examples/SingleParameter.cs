namespace Examples;

using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

public interface IDoSomething
{
    string DoSomething(Guid id);
}

public class SingleParameter
{
    private readonly IDoSomething _something;

    public SingleParameter(IDoSomething something)
    {
        _something = something;
    }

    public string Run(Guid id)
    {
        return _something.DoSomething(id);
    }
}

public class SingleParameterTests : Tests<SingleParameter>
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