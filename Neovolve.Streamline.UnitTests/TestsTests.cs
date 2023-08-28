namespace Neovolve.Streamline.UnitTests;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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

        await wrapper.DisposeAsync().ConfigureAwait(false);

        await service.Received().DisposeAsync().ConfigureAwait(false);

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

        await wrapper.DisposeAsync().ConfigureAwait(false);

        await wrapper.Received().DisposeAsync().ConfigureAwait(false);

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

        var action = async () => await wrapper.DisposeAsync().ConfigureAwait(false);

        await action.Should().NotThrowAsync().ConfigureAwait(false);
        await service.Received().DisposeAsync().ConfigureAwait(false);
        await stream.Received().DisposeAsync().ConfigureAwait(false);
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

        var action = async () => await wrapper.DisposeAsync().ConfigureAwait(false);

        await action.Should().NotThrowAsync().ConfigureAwait(false);
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
    public void ServiceThrowsExceptionWithNullKey()
    {
        var service = Substitute.For<ITargetService>();

        var wrapper = new Wrapper(service);

        Action action = () => wrapper.Service<ITargetService>(null!);

        action.Should().Throw<ArgumentNullException>();
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

        Action action = () => wrapper.Use<ITargetService>(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UseThrowsExceptionWithNullService()
    {
        var sut = new Wrapper();

        Action action = () => sut.Use<object>(null!);

        action.Should().Throw<ArgumentNullException>();
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
        protected override ConstructorInfo GetConstructor()
        {
            var constructors = typeof(MultiplePublicConstructors).GetConstructors()
                .Where(x => x.GetParameters().Any(y => y.ParameterType == typeof(ILogger)));

            return constructors.Single();
        }
    }
}