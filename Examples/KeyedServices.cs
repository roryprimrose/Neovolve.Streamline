namespace Examples;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Xunit;

public interface IDuplicateService
{
    int GetValue();
}

public class KeyedServices
{
    private readonly IDuplicateService _firstService;
    private readonly IDuplicateService _secondService;

    public KeyedServices(IDuplicateService firstService, IDuplicateService secondService)
    {
        _firstService = firstService;
        _secondService = secondService;
    }

    public int AddValues()
    {
        var firstValue = _firstService.GetValue();
        var secondValue = _secondService.GetValue();

        return firstValue + secondValue;
    }
}

public class KeyedServicesTests : Tests<KeyedServices>
{
    [Fact]
    public void CanConfigureKeyedServices()
    {
        Service<IDuplicateService>("A").GetValue().Returns(3);
        Service<IDuplicateService>("B").GetValue().Returns(15);

        var actual = SUT.AddValues();

        actual.Should().Be(18);
    }

    protected override object ResolveService(ParameterInfo parameter)
    {
        if (parameter.ParameterType != typeof(IDuplicateService))
        {
            return base.ResolveService(parameter);
        }

        if (parameter.Name == "firstService")
        {
            return ResolveService(parameter.ParameterType, "A");
        }

        return ResolveService(parameter.ParameterType, "B");
    }
}