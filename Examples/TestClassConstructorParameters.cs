namespace Examples;

using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

public class TestClassConstructorParameters
{
    private readonly ILogger _logger;

    public TestClassConstructorParameters(ILogger logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("Hey, we did something at {0}", DateTimeOffset.UtcNow);
    }
}

public class TestClassConstructorParametersTests : Tests<TestClassConstructorParameters>
{
    public TestClassConstructorParametersTests(ITestOutputHelper output) : base(output.BuildLogger())
    {
    }

    [Fact]
    public void CanUseTestConstructorArgument()
    {
        var logger = Service<ICacheLogger>();

        SUT.DoSomething();

        logger.Entries.Should().HaveCount(1);
    }
}