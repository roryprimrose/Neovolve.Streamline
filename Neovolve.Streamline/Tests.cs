namespace Neovolve.Streamline;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
///     The <see cref="Tests{T}" /> class is used to define a base class for unit tests that configures a single system
///     under test (SUT) instance.
/// </summary>
/// <typeparam name="T">The type of the system under test.</typeparam>
/// <remarks>
///     This class provides a set of helper methods for creating and managing the SUT instance, as well as any dependencies
///     that it may have.
/// </remarks>
public abstract class Tests<T> : IAsyncDisposable, IDisposable where T : class
{
    private readonly Dictionary<string, object> _services = new();
    private T? _sut;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Tests{T}" /> class.
    /// </summary>
    /// <param name="services">The services to use when creating the SUT.</param>
    protected Tests(params object[] services)
    {
        foreach (var service in services)
        {
            StoreServiceAsAllTypes(service);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Returns the service instance for the specified type.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve.</typeparam>
    /// <returns>
    ///     Returns the service instance for the specified type.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The typical use case for this method is to create and store a service of the requested type that will be used
    ///         to create the SUT.
    ///         This method can also be used as a mechanism to create and store a variable for a unit test in a way that is
    ///         abstracted from how the value is created.
    ///     </para>
    ///     <para>
    ///         The derived class will provide the mechanism for creating the service instance.
    ///         This allows for extensibility such that different mocking frameworks can be used to create the services.
    ///     </para>
    /// </remarks>
    public TService Service<TService>()
    {
        return Service<TService>(string.Empty);
    }

    /// <summary>
    ///     Returns the service instance for the specified type and key.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve.</typeparam>
    /// <param name="key">The key of the service to retrieve.</param>
    /// <returns>
    ///     Returns the service instance for the specified type and key.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The typical use case for this method is to create and store a service of the requested type that will be used
    ///         to create the SUT.
    ///         This method can also be used as a mechanism to create and store a variable for a unit test in a way that is
    ///         abstracted from how the value is created.
    ///     </para>
    ///     <para>
    ///         The <paramref name="key" /> parameter allows a unit test to create multiple instances of a service type.
    ///     </para>
    /// </remarks>
    public TService Service<TService>(string key)
    {
        key = key ?? throw new ArgumentNullException(nameof(key));

        return (TService)ResolveService(typeof(TService), key);
    }

    /// <summary>
    ///     Stores the specified service type to be used in a unit test.
    /// </summary>
    /// <typeparam name="TService">The type of the service to store.</typeparam>
    /// <returns>Returns the stored service instance.</returns>
    /// <remarks>
    ///     This method allows a unit test provide its own service instance rather than having one created by
    ///     <see cref="Service{TService}()" />.
    /// </remarks>
    public TService Use<TService>() where TService : new()
    {
        var service = new TService();

        return Use(service, string.Empty);
    }

    /// <summary>
    ///     Stores the specified service instance to be used in a unit test.
    /// </summary>
    /// <typeparam name="TService">The type of the service to store.</typeparam>
    /// <param name="service">The service instance to store.</param>
    /// <returns>Returns the stored service instance.</returns>
    /// <remarks>
    ///     This method allows a unit test provide its own service instance rather than having one created by
    ///     <see cref="Service{TService}()" />.
    /// </remarks>
    public TService Use<TService>(TService service)
    {
        return Use(service, string.Empty);
    }

    /// <summary>
    ///     Stores the specified service instance to be used in a unit test.
    /// </summary>
    /// <typeparam name="TService">The type of the service to store.</typeparam>
    /// <param name="service">The service instance to store.</param>
    /// <param name="key">The key to use when storing the service instance.</param>
    /// <returns>Returns the stored service instance.</returns>
    /// <remarks>
    ///     <para>
    ///         This method allows a unit test to provide its own service instance rather than having one created by
    ///         <see cref="Service{TService}()" />.
    ///     </para>
    ///     <para>
    ///         The <paramref name="key" /> parameter allows a unit test to store multiple instances of a service type.
    ///     </para>
    /// </remarks>
    public TService Use<TService>(TService service, string key)
    {
        service = service ?? throw new ArgumentNullException(nameof(service));
        key = key ?? throw new ArgumentNullException(nameof(key));

        StoreServiceAsAllTypes(service, key);

        return service;
    }

    /// <summary>
    ///     Builds and returns an instance of the specified service type with the specified key.
    /// </summary>
    /// <param name="type">The type of the service to build.</param>
    /// <param name="key">The key of the service to build.</param>
    /// <returns>Returns an instance of the specified service type with the specified key.</returns>
    protected abstract object BuildService(Type type, string key);

    /// <summary>
    ///     Builds and returns an instance of the system under test (SUT) using the constructor and parameter values obtained
    ///     from <see cref="GetConstructor" /> and <see cref="ResolveService(ParameterInfo)" /> respectively.
    /// </summary>
    /// <returns>Returns an instance of the SUT.</returns>
    protected virtual T BuildSUT()
    {
        var constructor = GetConstructor();

        var parameters = constructor.GetParameters();

        if (parameters.Length == 0)
        {
            var noParameters = Array.Empty<object>();

            return BuildSUT(constructor, noParameters);
        }

        var parameterValues = parameters.Select(ResolveService).ToArray();

        return BuildSUT(constructor, parameterValues);
    }

    /// <summary>
    ///     Builds and returns an instance of the system under test (SUT) using the specified constructor and parameter values.
    /// </summary>
    /// <param name="constructor">The constructor to use to build the SUT.</param>
    /// <param name="parameterValues">The parameter values to pass to the constructor.</param>
    /// <returns>Returns an instance of the SUT.</returns>
    protected virtual T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
    {
        return (T)constructor.Invoke(parameterValues);
    }

    /// <summary>
    ///     Disposes of the resources used by the test class.
    /// </summary>
    /// <param name="disposing">
    ///     True to release both managed and unmanaged resources; false to release only unmanaged
    ///     resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeSUT();

            // Dispose all the services and clear them out
            DisposeServices();
        }
    }

    /// <summary>
    ///     Returns the constructor to use when building the system under test (SUT).
    /// </summary>
    /// <returns>The constructor to use when building the SUT.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when there are multiple public or internal constructors on the SUT
    ///     type.
    /// </exception>
    protected virtual ConstructorInfo GetConstructor()
    {
        var publicConstructors =
            TargetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        if (publicConstructors.Length == 1)
        {
            return publicConstructors[0];
        }

        if (publicConstructors.Length > 1)
        {
            var publicMessage =
                $"Unable to create an instance of {TargetType.FullName} because there are {publicConstructors.Length} public constructors where only a single constructor is supported by default. To control the constructor to use, override the GetConstructor() method to return the specific constructor that should be used.";

            throw new InvalidOperationException(publicMessage);
        }

        var internalConstructors =
            TargetType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

        if (internalConstructors.Length == 1)
        {
            return internalConstructors[0];
        }

        var internalMessage =
            $"Unable to create an instance of {TargetType.FullName} because there are {internalConstructors.Length} internal constructors where only a single constructor is supported by default. To control the constructor to use, override the GetConstructor() method to return the specific constructor that should be used.";

        throw new InvalidOperationException(internalMessage);
    }

    /// <summary>
    ///     Resolves a service instance for the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter to resolve a service instance for.</param>
    /// <returns>Returns a service instance for the specified parameter.</returns>
    protected virtual object ResolveService(ParameterInfo parameter)
    {
        return ResolveService(parameter.ParameterType, string.Empty);
    }

    /// <summary>
    ///     Resolves a service instance for the specified type and key.
    /// </summary>
    /// <param name="type">The type of the service to resolve.</param>
    /// <param name="key">The key of the service to resolve.</param>
    /// <returns>Returns a service instance for the specified type and key.</returns>
    protected virtual object ResolveService(Type type, string key)
    {
        var cacheKey = GetCacheKey(type, key);

        if (_services.TryGetValue(cacheKey, out var existingService))
        {
            return existingService;
        }

        var service = BuildService(type, key);

        StoreService(service, type, key);

        return service;
    }

    private static IEnumerable<Type> GetAllRelatedTypes(Type sourceType)
    {
        yield return sourceType;

        // Find all the implemented interfaces and return them
        var interfaceTypes = sourceType.GetInterfaces();

        foreach (var interfaceType in interfaceTypes)
        {
            yield return interfaceType;
        }

        // Find all the base types
        var baseType = sourceType.BaseType;

        while (baseType != null
               && baseType != typeof(object))
        {
            yield return baseType;

            baseType = baseType.BaseType;
        }
    }

    private static string GetCacheKey(Type type, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return type.AssemblyQualifiedName!;
        }

        return type.AssemblyQualifiedName + "|" + key;
    }

    private async Task DisposeAsyncCore()
    {
        if (SUT is IAsyncDisposable sut)
        {
            await sut.DisposeAsync().ConfigureAwait(false);
        }

        _sut = null;

        foreach (var service in _services)
        {
            var disposableService = service.Value as IAsyncDisposable;

            if (disposableService == null)
            {
                continue;
            }

            try
            {
                await disposableService.DisposeAsync().ConfigureAwait(false);
            }
            catch
            {
                // The service failed to dispose but we don't want to crash the test runner for this kind of failure
            }
            finally
            {
                _services.Remove(service.Key);
            }
        }
    }

    private void DisposeServices()
    {
        foreach (var service in _services)
        {
            var disposableService = service.Value as IDisposable;

            if (disposableService == null)
            {
                continue;
            }

            try
            {
                disposableService.Dispose();
            }
            catch
            {
                // The service failed to dispose but we don't want to crash the test runner for this kind of failure
            }
            finally
            {
                _services.Remove(service.Key);
            }
        }
    }

    private void DisposeSUT()
    {
        if (_sut == null)
        {
            return;
        }

        // Reset the SUT
        if (_sut is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _sut = null;
    }

    private void StoreService(object service, Type serviceType, string key = "")
    {
        // The reason we can't use service.GetType() is because that would be the type of the implementation
        // which will often be different than the service type requested (class vs interface for example)
        var cacheKey = GetCacheKey(serviceType, key);

        _services[cacheKey] = service;

        // Services have changed. Tear down the SUT so that it can be build with the new services
        DisposeSUT();
    }

    private void StoreServiceAsAllTypes(object service, string key = "")
    {
        var serviceTypes = GetAllRelatedTypes(service.GetType());

        foreach (var serviceType in serviceTypes)
        {
            StoreService(service, serviceType, key);
        }
    }

    /// <summary>
    ///     Gets the system under test (SUT) instance for the current test.
    /// </summary>
    /// <remarks>
    ///     This property will return the same SUT instance for the lifetime of the class unless there are any changes made by
    ///     using the <see cref="Service{TService}()" /> or <see cref="Use{TService}(TService)" /> methods.
    /// </remarks>
    public T SUT
    {
        get
        {
            if (_sut != null)
            {
                return _sut;
            }

            // The SUT instance does not exist so we need to create it
            _sut = BuildSUT();

            return _sut;
        }
    }

    /// <summary>
    ///     Gets the type of the system under test (SUT).
    /// </summary>
    /// <remarks>
    ///     This property allows a unit test class to override the type of SUT being created. For example, a unit test class
    ///     could target an interface or abstract class but set <see cref="TargetType" /> to a more specific but compatible
    ///     type.
    /// </remarks>
    protected virtual Type TargetType => typeof(T);
}