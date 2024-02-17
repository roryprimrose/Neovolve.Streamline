namespace Neovolve.Streamline;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
///     The <see cref="TestsBase" /> class is used to define a base class for unit tests that configures a single system
///     under test (SUT) instance.
/// </summary>
/// <remarks>
///     This class provides a set of helper methods for creating and managing the SUT instance, as well as any dependencies
///     that it may have.
/// </remarks>
public abstract class TestsBase : IDisposable
#if NETSTANDARD2_1
    , IAsyncDisposable
#endif
{
    private readonly Dictionary<string, object> _services = new();
    private object? _sut;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Tests{T}" /> class.
    /// </summary>
    /// <param name="services">The services to use when creating the SUT.</param>
    protected TestsBase(params object[] services)
    {
        for (var index = 0; index < services.Length; index++)
        {
            var service = services[index];

            StoreServiceAsAllTypes(service);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
#if NETSTANDARD2_1
    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }
#endif

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
    ///     from <see cref="TestsBase.GetConstructor{T}" /> and <see cref="TestsBase.ResolveService(ParameterInfo)" />
    ///     respectively.
    /// </summary>
    /// <typeparam name="T">The type of SUT to build.</typeparam>
    /// <returns>Returns an instance of the SUT.</returns>
    protected virtual T BuildSUT<T>() where T : class
    {
        var constructor = GetConstructor<T>();

        var parameters = constructor.GetParameters();

        if (parameters.Length == 0)
        {
            var noParameters = Array.Empty<object>();

            return BuildSUT<T>(constructor, noParameters);
        }

        var parameterValues = parameters.Select(ResolveService).ToArray();

        return BuildSUT<T>(constructor, parameterValues);
    }

    /// <summary>
    ///     Builds and returns an instance of the system under test (SUT) using the specified constructor and parameter values.
    /// </summary>
    /// <param name="constructor">The constructor to use to build the SUT.</param>
    /// <param name="parameterValues">The parameter values to pass to the constructor.</param>
    /// <typeparam name="T">The type of SUT to build.</typeparam>
    /// <returns>Returns an instance of the SUT.</returns>
    protected virtual T BuildSUT<T>(ConstructorInfo constructor, object[] parameterValues) where T : class
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
    protected virtual ConstructorInfo GetConstructor<T>() where T : class
    {
        var targetType = TargetType ?? typeof(T);

        var publicConstructors =
            targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        if (publicConstructors.Length == 1)
        {
            return publicConstructors[0];
        }

        if (publicConstructors.Length > 1)
        {
            var publicMessage =
                $"Unable to create an instance of {targetType.FullName} because there are {publicConstructors.Length} public constructors where only a single constructor is supported by default. To control the constructor to use, override the GetConstructor() method to return the specific constructor that should be used.";

            throw new InvalidOperationException(publicMessage);
        }

        var internalConstructors =
            targetType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

        if (internalConstructors.Length == 1)
        {
            return internalConstructors[0];
        }

        var internalMessage =
            $"Unable to create an instance of {targetType.FullName} because there are {internalConstructors.Length} internal constructors where only a single constructor is supported by default. To control the constructor to use, override the GetConstructor() method to return the specific constructor that should be used.";

        throw new InvalidOperationException(internalMessage);
    }

    /// <summary>
    ///     Gets the system under test (SUT) instance for the current test.
    /// </summary>
    /// <remarks>
    ///     This method will return the same SUT instance for the lifetime of the class unless there are any changes made by
    ///     using the <see cref="TestsBase.Service{TService}()" /> or <see cref="TestsBase.Use{TService}()" /> methods.
    /// </remarks>
    protected T GetSUT<T>() where T : class
    {
        if (_sut is T sut)
        {
            return sut;
        }

        if (_sut != null)
        {
            // The caller has requested a different type
            // Destroy the previous SUT first
            DisposeSUT();
        }

        // The SUT instance either does not exist or the caller is TestsInternal where a different SUT type is requested
        // In this case we will create a new SUT instance
        _sut = sut = BuildSUT<T>();

        return sut;
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

        for (var index = 0; index < interfaceTypes.Length; index++)
        {
            var interfaceType = interfaceTypes[index];

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
    
#if NETSTANDARD2_1
    private async Task DisposeAsyncCore()
    {
        if (_sut is IAsyncDisposable sut)
        {
            await sut.DisposeAsync().ConfigureAwait(false);
        }

        _sut = null;

        // Copy the keys so that we can safely enumerate them and remove the services from the dictionary
        var keys = _services.Keys.ToList();

        foreach (var key in keys)
        {
            var service = _services[key];
            var disposableService = service as IAsyncDisposable;

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
                // The service failed to dispose, but we don't want to crash the test runner for this kind of failure
            }
            finally
            {
                _services.Remove(key);
            }
        }
    }
#endif

    private void DisposeServices()
    {
        // Create a copy of the keys so that we can safely enumerate the list and remove services from the dictionary
        var keys = _services.Keys.ToList();

        foreach (var key in keys)
        {
            var service = _services[key];
            var disposableService = service as IDisposable;

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
                _services.Remove(key);
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
    ///     Gets the type of the system under test (SUT).
    /// </summary>
    /// <remarks>
    ///     This property allows a unit test class to override the type of SUT being created. For example, a unit test class
    ///     could target an interface or abstract class but set <see cref="TargetType" /> to a more specific but compatible
    ///     type.
    /// </remarks>
    protected virtual Type? TargetType => _sut?.GetType();
}