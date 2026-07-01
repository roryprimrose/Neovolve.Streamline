namespace Neovolve.Streamline.UnitTests;

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

public class ServiceProviderTests
{
    [Fact]
    public void ConfigureServicesRegistrationIsResolved()
    {
        var configured = Substitute.For<ITargetService>();

        var wrapper = new ProviderWrapper();

        wrapper.ConfigureServices(services => services.AddSingleton(configured));

        wrapper.Service<ITargetService>().Should().BeSameAs(configured);
    }

    [Fact]
    public void ConfigureServicesThrowsWhenConfigureIsNull()
    {
        var wrapper = new ProviderWrapper();

        Action action = () => wrapper.ConfigureServices(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ImportedProviderResolvesDependencyForSut()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var service = Substitute.For<ITargetService>();

        service.GetValue(id).Returns(expected);

        var collection = new ServiceCollection();

        collection.AddSingleton(service);

        using var provider = collection.BuildServiceProvider();

        var wrapper = new ProviderWrapper();

        wrapper.ImportServices(provider);

        wrapper.SUT.GetValue(id).Should().Be(expected);
    }

    [Fact]
    public void ImportServicesThrowsWhenProviderIsNull()
    {
        var wrapper = new ProviderWrapper();

        Action action = () => wrapper.ImportServices(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void KeyedServiceResolvesThroughProvider()
    {
        var wrapper = new ProviderWrapper();

        var keyed = wrapper.Service<ITargetService>("primary");

        wrapper.Provider.GetKeyedService<ITargetService>("primary").Should().BeSameAs(keyed);
    }

    [Fact]
    public void RequestingSutTypeThroughProviderThrows()
    {
        var wrapper = new ProviderWrapper();

        Action action = () => wrapper.Provider.GetService(typeof(Target));

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RequestingSutTypeThroughServiceThrows()
    {
        var wrapper = new ProviderWrapper();

        Action action = () => wrapper.Service<Target>();

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ServiceProviderAutoMocksUnresolvedService()
    {
        var wrapper = new ProviderWrapper();

        var resolved = wrapper.Provider.GetService(typeof(ITargetService));

        resolved.Should().NotBeNull();
        wrapper.Service<ITargetService>().Should().BeSameAs(resolved);
    }

    [Fact]
    public void ServiceProviderReturnsSameInstanceAsService()
    {
        var wrapper = new ProviderWrapper();

        var service = wrapper.Service<ITargetService>();

        wrapper.Provider.GetService(typeof(ITargetService)).Should().BeSameAs(service);
    }

    [Fact]
    public void ServiceProviderSuppliesItselfToSut()
    {
        var wrapper = new Wrapper<ProviderConsumer>();

        var service = wrapper.Service<ITargetService>();

        var resolved = wrapper.SUT.ServiceProvider.GetService(typeof(ITargetService));

        resolved.Should().BeSameAs(service);
    }

    [Fact]
    public void StoredServiceOverridesConfiguredServiceByDefault()
    {
        var configured = Substitute.For<ITargetService>();
        var stub = Substitute.For<ITargetService>();

        var wrapper = new ProviderWrapper();

        wrapper.ConfigureServices(services => services.AddSingleton(configured));
        wrapper.Use(stub);

        wrapper.Service<ITargetService>().Should().BeSameAs(stub);
    }

    [Fact]
    public void ImportedProviderOverridesStoredServiceWhenExternalWins()
    {
        var external = Substitute.For<ITargetService>();
        var stub = Substitute.For<ITargetService>();

        var collection = new ServiceCollection();

        collection.AddSingleton(external);

        using var provider = collection.BuildServiceProvider();

        var wrapper = new ProviderWrapper();

        wrapper.Use(stub);
        wrapper.ImportServices(provider, externalWins: true);

        wrapper.Service<ITargetService>().Should().BeSameAs(external);
    }

    [Fact]
    public void StoredServiceOverridesImportedProviderByDefault()
    {
        var external = Substitute.For<ITargetService>();
        var stub = Substitute.For<ITargetService>();

        var collection = new ServiceCollection();

        collection.AddSingleton(external);

        using var provider = collection.BuildServiceProvider();

        var wrapper = new ProviderWrapper();

        wrapper.Use(stub);
        wrapper.ImportServices(provider);

        wrapper.Service<ITargetService>().Should().BeSameAs(stub);
    }
}
