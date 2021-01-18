namespace Neovolve.Streamline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class Tests<T> : IDisposable where T : class
    {
        private readonly Dictionary<string, object> _services = new();
        private T? _sut;

        protected Tests(params object[] services)
        {
            foreach (var service in services)
            {
                StoreServiceAsAllTypes(service);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TService Service<TService>()
        {
            return Service<TService>(string.Empty);
        }

        public TService Service<TService>(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            return (TService) ResolveService(typeof(TService), key);
        }

        public TService Use<TService>(TService service)
        {
            return Use(service, string.Empty);
        }

        public TService Use<TService>(TService service, string key)
        {
            service = service ?? throw new ArgumentNullException(nameof(service));
            key = key ?? throw new ArgumentNullException(nameof(key));
            
            StoreServiceAsAllTypes(service, key);

            return service;
        }

        protected abstract object BuildService(Type type, string key);

        protected virtual T BuildSUT()
        {
            var constructor = GetConstructor();

            var parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                var noParameters = Array.Empty<object>();

                return (T) constructor.Invoke(noParameters);
            }

            var parameterValues = parameters.Select(ResolveService).ToArray();

            return BuildSUT(constructor, parameterValues);
        }

        protected virtual T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
        {
            return (T) constructor.Invoke(parameterValues);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeSUT();

                // Dispose all the services and clear them out
                DisposeServices();
            }
        }

        protected virtual ConstructorInfo GetConstructor()
        {
            var publicConstructors =
                typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (publicConstructors.Length == 1)
            {
                return publicConstructors[0];
            }

            if (publicConstructors.Length > 1)
            {
                var publicMessage =
                    $"Unable to create an instance of {typeof(T).FullName} because there are {publicConstructors.Length} public constructors where only a single constructor is supported by default. To control the constructor to use, override the GetConstructor() method to return the specific constructor that should be used.";

                throw new InvalidOperationException(publicMessage);
            }

            var internalConstructors =
                typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            if (internalConstructors.Length == 1)
            {
                return internalConstructors[0];
            }

            var internalMessage =
                $"Unable to create an instance of {typeof(T).FullName} because there are {internalConstructors.Length} internal constructors where only a single constructor is supported by default. To control the constructor to use, override the GetConstructor() method to return the specific constructor that should be used.";

            throw new InvalidOperationException(internalMessage);
        }

        protected virtual object ResolveService(ParameterInfo parameter)
        {
            return ResolveService(parameter.ParameterType, string.Empty);
        }

        protected virtual object ResolveService(Type type, string key)
        {
            var cacheKey = GetCacheKey(type, key);

            if (_services.ContainsKey(cacheKey))
            {
                return _services[cacheKey];
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

        private void DisposeServices()
        {
            var services = _services.Values.OfType<IDisposable>();

            foreach (var service in services)
            {
                try
                {
                    service.Dispose();
                }
                catch
                {
                    // The service failed to dispose but we don't want to crash the test runner for this kind of failure
                }
            }

            _services.Clear();
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
    }
}