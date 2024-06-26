﻿namespace Neovolve.Streamline.UnitTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Neovolve.Logging.Xunit;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

public class TestsTests
{
    private readonly ICacheLogger _logger;

    public TestsTests(ITestOutputHelper output)
    {
        _logger = output.BuildLogger();
    }

    [Fact]
    public void CanCreateMissingServices()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new Wrapper();

        wrapper.Service<ITargetService>().GetValue(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    [Fact]
    public void CanCreateSUTFromTypeWithDefaultConstructor()
    {
        var wrapper = new Wrapper<DefaultConstructor>();

        wrapper.SUT.Should().NotBeNull();
    }

    [Fact]
    public void CanCreateSUTFromTypeWithSingleInternalConstructor()
    {
        var wrapper = new Wrapper<SingleInternalConstructor>();

        wrapper.SUT.Should().NotBeNull();
        wrapper.SUT.Logger.Should().NotBeNull();
    }

    [Fact]
    public void CanCreateSUTFromTypeWithSinglePublicConstructor()
    {
        var wrapper = new Wrapper<SinglePublicConstructor>();

        wrapper.SUT.Should().NotBeNull();
        wrapper.SUT.Service.Should().NotBeNull();
        wrapper.SUT.Logger.Should().BeNull();
    }

    [Fact]
    public void CanCreateSUTFromTypeWithSpecifiedConstructor()
    {
        var wrapper = new TargetSpecificConstructorWrapper();

        wrapper.SUT.Should().NotBeNull();
        wrapper.SUT.Service.Should().BeNull();
        wrapper.SUT.Logger.Should().NotBeNull();
    }

    [Fact]
    public void CanCreateWithConstructorProvidedService()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var service = Substitute.For<ITargetService>();

        service.GetValue(id).Returns(expected);

        var wrapper = new Wrapper(service);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    [Fact]
    public void CanPartialMockSUT()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new Wrapper();

        wrapper.SUT.GetValueInternal(id).Returns(expected);

        var actual = wrapper.SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    [Fact]
    public void CreatingWithServicesRegistersServicesAsMultipleTypes()
    {
        var wrapper = new Wrapper(_logger);

        wrapper.Service<ICacheLogger>().Should().BeSameAs(_logger);
        wrapper.Service<CacheLogger>().Should().BeSameAs(_logger);
        wrapper.Service<FilterLogger>().Should().BeSameAs(_logger);
        wrapper.Service<ILogger>().Should().BeSameAs(_logger);
    }

    [Fact]
    public async Task DisposeAsyncCleansUpServices()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);

        // Make sure the SUT exists
        wrapper.SUT.Should().NotBeNull();

        await wrapper.DisposeAsync();

        await service.Received().DisposeAsync();

        var nextService = wrapper.Service<ITargetService>();

        nextService.Should().NotBeSameAs(service);
    }

    [Fact]
    public async Task DisposeAsyncCleansUpSUT()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = Substitute.ForPartsOf<Wrapper>();

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);

        // Make sure the SUT exists
        var sut = wrapper.SUT;

        sut.Should().NotBeNull();

        await wrapper.DisposeAsync();

        await wrapper.Received().DisposeAsync();

        var nextSut = wrapper.SUT;

        nextSut.Should().NotBeSameAs(sut);
    }

    [Fact]
    public async Task DisposeAsyncContinuesToDisposeServicesWhenOneServiceDisposeThrowsException()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        using var stream = Substitute.ForPartsOf<MemoryStream>();
        var wrapper = new Wrapper();

        wrapper.Use(stream);

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);
        service.When(x => x.DisposeAsync()).Do(_ => throw new TimeoutException());

        // Make sure the SUT exists
        wrapper.SUT.Should().NotBeNull();

        var action = async () => await wrapper.DisposeAsync();

        await action.Should().NotThrowAsync();
        await service.Received().DisposeAsync();
        await stream.Received().DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsyncDoesNotThrowExceptionWhenDisposingServiceThrowsException()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);
        service.When(x => x.DisposeAsync()).Do(_ => throw new TimeoutException());

        // Make sure the SUT exists
        wrapper.SUT.Should().NotBeNull();

        var action = async () => await wrapper.DisposeAsync();

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public void DisposeCleansUpServices()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);

        // Make sure the SUT exists
        wrapper.SUT.Should().NotBeNull();

        wrapper.Dispose();

        service.Received().Dispose();

        var nextService = wrapper.Service<ITargetService>();

        nextService.Should().NotBeSameAs(service);
    }

    [Fact]
    public void DisposeCleansUpSUT()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = Substitute.ForPartsOf<Wrapper>();

        wrapper.Service<ITargetService>().GetValue(id).Returns(expected);

        // Make sure the SUT exists
        var sut = wrapper.SUT;

        sut.Should().NotBeNull();

        wrapper.Dispose();

        wrapper.Received().Dispose();

        var nextSut = wrapper.SUT;

        nextSut.Should().NotBeSameAs(sut);
    }

    [Fact]
    public void DisposeContinuesToDisposeServicesWhenOneServiceDisposeThrowsException()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        using var stream = Substitute.ForPartsOf<MemoryStream>();
        var wrapper = new Wrapper();

        wrapper.Use(stream);

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);
        service.When(x => x.Dispose()).Do(_ => throw new TimeoutException());

        // Make sure the SUT exists
        wrapper.SUT.Should().NotBeNull();

        var action = () => wrapper.Dispose();

        action.Should().NotThrow();
        service.Received().Dispose();
        stream.Received().Dispose();
    }

    [Fact]
    public void DisposeDoesNotThrowExceptionWhenDisposingServiceThrowsException()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>();

        service.GetValue(id).Returns(expected);
        service.When(x => x.Dispose()).Do(_ => throw new TimeoutException());

        // Make sure the SUT exists
        wrapper.SUT.Should().NotBeNull();

        var action = () => wrapper.Dispose();

        action.Should().NotThrow();
    }

    [Fact]
    public void ServiceByKeyReturnsCachedInstanceForSameKey()
    {
        var key = Guid.NewGuid().ToString();
        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>(key);
        var nextService = wrapper.Service<ITargetService>(key);

        nextService.Should().BeSameAs(service);
    }

    [Fact]
    public void ServiceByKeyReturnsNewInstanceForDifferentKey()
    {
        var firstKey = Guid.NewGuid().ToString();
        var secondKey = Guid.NewGuid().ToString();
        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>(firstKey);
        var nextService = wrapper.Service<ITargetService>(secondKey);

        nextService.Should().NotBeSameAs(service);
    }

    [Fact]
    public void ServiceByKeyThrowsExceptionWhenNullServiceStored()
    {
        var key = Guid.NewGuid().ToString();
        var wrapper = new Wrapper();

        wrapper.Use<ILogger>(null, key);

        var action = () => wrapper.Service<ILogger>(key);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ServiceReturnsCachedInstance()
    {
        var wrapper = new Wrapper();

        var service = wrapper.Service<ITargetService>();
        var nextService = wrapper.Service<ITargetService>();

        nextService.Should().BeSameAs(service);
    }

    [Fact]
    public void ServiceReturnsConstructorProvidedService()
    {
        var expected = Substitute.For<ITargetService>();

        var wrapper = new Wrapper(expected);

        var actual = wrapper.Service<ITargetService>();

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void ServiceThrowsExceptionWhenNullServiceStored()
    {
        var wrapper = new Wrapper();

        wrapper.Use<ILogger>(null);

        var action = () => wrapper.Service<ILogger>();

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ServiceThrowsExceptionWithNullKey()
    {
        var service = Substitute.For<ITargetService>();

        var wrapper = new Wrapper(service);

        Action action = () => wrapper.Service<ITargetService>(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task SUTReturnsSameInstanceOnAllThreads()
    {
        var maxCount = 100;
        var tasks = new List<Task<Target>>(maxCount);
        var wrapper = new Wrapper();

        for (var index = 0; index < maxCount; index++)
        {
            var task = GetSUT(wrapper);

            tasks.Add(task);
        }

        var results = await Task.WhenAll(tasks);

        var first = results[0];

        foreach (var result in results)
        {
            result.Should().BeSameAs(first);
        }
    }

    [Fact]
    public void SUTThrowsExceptionWhenMultipleInternalConstructorsFound()
    {
        var wrapper = new Wrapper<MultipleInternalConstructors>();

        var action = () =>
        {
            // ReSharper disable once UnusedVariable
            var sut = wrapper.SUT;
        };

        var message = action.Should().Throw<InvalidOperationException>().And.Message;

        _logger.LogInformation(message);
    }

    [Fact]
    public void SUTThrowsExceptionWhenMultiplePublicConstructorsFound()
    {
        var wrapper = new Wrapper<MultiplePublicConstructors>();

        var action = () =>
        {
            // ReSharper disable once UnusedVariable
            var sut = wrapper.SUT;
        };

        var message = action.Should().Throw<InvalidOperationException>().And.Message;

        _logger.LogInformation(message);
    }

    [Fact]
    public void UseByKeyReturnsService()
    {
        var key = Guid.NewGuid().ToString();
        var wrapper = new Wrapper();

        var actual = wrapper.Use(_logger, key);

        actual.Should().BeSameAs(_logger);
    }

    [Fact]
    public void UseByKeyStoresCustomService()
    {
        var key = Guid.NewGuid().ToString();
        var expected = Substitute.For<ITargetService>();

        var wrapper = new Wrapper();

        wrapper.Use(expected, key);

        var actual = wrapper.Service<ITargetService>(key);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void UseByKeyStoresUniqueCustomServices()
    {
        var firstKey = Guid.NewGuid().ToString();
        var secondKey = Guid.NewGuid().ToString();
        var firstService = Substitute.For<ITargetService>();
        var secondService = Substitute.For<ITargetService>();

        var wrapper = new Wrapper();

        wrapper.Use(firstService, firstKey);
        wrapper.Use(secondService, secondKey);

        var firstActual = wrapper.Service<ITargetService>(firstKey);
        var secondActual = wrapper.Service<ITargetService>(secondKey);

        firstActual.Should().NotBeSameAs(secondActual);
        firstActual.Should().BeSameAs(firstService);
        secondActual.Should().BeSameAs(secondService);
    }

    [Fact]
    public void UseByKeyWithNullReturnsNull()
    {
        var key = Guid.NewGuid().ToString();
        var wrapper = new Wrapper();

        var actual = wrapper.Use<ILogger>(null, key);

        actual.Should().BeNull();
    }

    [Fact]
    public void UseOverwritesExistingService()
    {
        var service = Substitute.For<ITargetService>();
        var expected = Substitute.For<ITargetService>();

        var wrapper = new Wrapper();

        wrapper.Use(service);
        wrapper.Use(expected);

        var actual = wrapper.Service<ITargetService>();

        actual.Should().BeSameAs(expected);
        actual.Should().NotBeSameAs(service);
    }

    [Fact]
    public void UseRegistersServicesAsMultipleTypes()
    {
        var wrapper = new Wrapper();

        wrapper.Use(_logger);

        wrapper.Service<ICacheLogger>().Should().BeSameAs(_logger);
        wrapper.Service<CacheLogger>().Should().BeSameAs(_logger);
        wrapper.Service<FilterLogger>().Should().BeSameAs(_logger);
        wrapper.Service<ILogger>().Should().BeSameAs(_logger);
    }

    [Fact]
    public void UseResetsSUTWhenServiceUpdated()
    {
        var wrapper = new Wrapper();

        var firstService = Substitute.For<ITargetService>();

        wrapper.Use(firstService);

        var firstSut = wrapper.SUT;

        var secondService = Substitute.For<ITargetService>();

        wrapper.Use(secondService);

        var secondSut = wrapper.SUT;

        firstSut.Should().NotBeSameAs(secondSut);
    }

    [Fact]
    public void UseReturnsService()
    {
        var wrapper = new Wrapper();

        var actual = wrapper.Use(_logger);

        actual.Should().BeSameAs(_logger);
    }

    [Fact]
    public void UseStoresCustomService()
    {
        var expected = Substitute.For<ITargetService>();

        var wrapper = new Wrapper();

        wrapper.Use(expected);

        var actual = wrapper.Service<ITargetService>();

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void UseThrowsExceptionWithNullKey()
    {
        var service = Substitute.For<ITargetService>();

        var wrapper = new Wrapper(service);

        Action action = () => wrapper.Use<ITargetService>(null, null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UseThrowsExceptionWithNullService()
    {
        var sut = new Wrapper();

        Action action = () => sut.Use<object>(null, null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UseTOverwritesExistingService()
    {
        var service = Substitute.For<ITargetService>();

        var wrapper = new Wrapper();

        wrapper.Use(service);
        wrapper.Use<TargetService>();

        var actual = wrapper.Service<ITargetService>();

        actual.Should().NotBeSameAs(service);
    }

    [Fact]
    public void UseTRegistersServicesAsMultipleTypes()
    {
        var wrapper = new Wrapper();

        wrapper.Use<TargetService>();

        var expected = wrapper.Service<TargetService>();

        wrapper.Service<ITargetService>().Should().BeSameAs(expected);
        wrapper.Service<IDisposable>().Should().BeSameAs(expected);
    }

    [Fact]
    public void UseTResetsSUTWhenServiceUpdated()
    {
        var wrapper = new Wrapper();

        wrapper.Use<TargetService>();

        var firstSut = wrapper.SUT;

        wrapper.Use<TargetService>();

        var secondSut = wrapper.SUT;

        firstSut.Should().NotBeSameAs(secondSut);
    }

    [Fact]
    public void UseTReturnsService()
    {
        var wrapper = new Wrapper();

        var actual = wrapper.Use<TargetService>();

        actual.Should().BeOfType<TargetService>();
    }

    [Fact]
    public void UseTStoresCustomService()
    {
        var wrapper = new Wrapper();

        wrapper.Use<TargetService>();

        var actual = wrapper.Service<ITargetService>();

        actual.Should().BeOfType<TargetService>();
    }

    [Fact]
    public void UseWithNullReturnsNull()
    {
        var wrapper = new Wrapper();

        var actual = wrapper.Use<ILogger>(null);

        actual.Should().BeNull();
    }

    private async Task<Target> GetSUT(Wrapper wrapper)
    {
        await Task.Delay(10);

        return wrapper.SUT;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class DefaultConstructor
    {
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class MultipleInternalConstructors
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        internal MultipleInternalConstructors(ITargetService service)
        {
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        internal MultipleInternalConstructors(ILogger logger)
        {
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class MultiplePublicConstructors
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        public MultiplePublicConstructors(ITargetService service)
        {
            Service = service;
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        public MultiplePublicConstructors(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger? Logger { get; }

        public ITargetService? Service { get; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class SingleInternalConstructor
    {
        internal SingleInternalConstructor(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class SinglePublicConstructor
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        public SinglePublicConstructor(ITargetService service)
        {
            Service = service;
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedMember.Local
        internal SinglePublicConstructor(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger? Logger { get; }

        public ITargetService? Service { get; }
    }

    private class TargetSpecificConstructorWrapper : Wrapper<MultiplePublicConstructors>
    {
        protected override ConstructorInfo GetConstructor<TMultiplePublicConstructors>()
        {
            var constructors = typeof(TMultiplePublicConstructors).GetConstructors()
                .Where(x => x.GetParameters().Any(y => y.ParameterType == typeof(ILogger)));

            return constructors.Single();
        }
    }
}