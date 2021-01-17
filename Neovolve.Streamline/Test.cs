namespace Neovolve.Streamline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class Test<T> where T : class
    {
        private readonly Dictionary<string, object> _services = new();
        private readonly string DefaultKey = nameof(ParameterInfo);
        private T _sut;

        protected Test(params object[] services)
        {
            foreach (var service in services)
            {
                StoreService(service, DefaultKey);
            }
        }

        protected abstract object BuildService(Type type, string key);

        // ReSharper disable once InconsistentNaming
        protected virtual T BuildSUT()
        {
            var constructor = GetConstructor();

            var parameters = constructor.GetParameters();
            var parameterValues = parameters.Select(ResolveService).ToArray();

            return (T) constructor.Invoke(parameterValues);
        }

        protected virtual ConstructorInfo GetConstructor()
        {
            var publicConstructors =
                typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (publicConstructors.Length == 1)
            {
                return publicConstructors[0];
            }

            var internalConstructors =
                typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            if (internalConstructors.Length == 1)
            {
                return internalConstructors[0];
            }

            var message =
                $"Unable to create an instance of {typeof(T).FullName} because there are {publicConstructors.Length} public constructors and {internalConstructors.Length} non-public constructors but only one on either is supported.";

            throw new InvalidOperationException(message);
        }

        protected object ResolveService(ParameterInfo parameter)
        {
            // The reason we join the parameter name with the default key is so that we can support building multiple service instances of the same type for different parameter declarations
            var key = DefaultKey + "|" + parameter.Name;

            return ResolveService(parameter.ParameterType, key);
        }

        protected object ResolveService(Type type, string key)
        {
            var fullKey = key + "|" + type.AssemblyQualifiedName;

            if (_services.ContainsKey(fullKey))
            {
                return _services[fullKey];
            }

            var service = BuildService(type, key);

            return StoreService(service, fullKey);
        }

        protected TService Service<TService>()
        {
            // By default the service to return is the one created for a constructor parameter
            return Service<TService>(DefaultKey);
        }

        protected TService Service<TService>(string key)
        {
            return (TService) ResolveService(typeof(TService), key);
        }

        protected object Use(object service)
        {
            return Use(service, DefaultKey);
        }

        protected object Use(object service, string key)
        {
            service = service ?? throw new ArgumentNullException(nameof(service));

            return StoreService(service, key);
        }

        private object StoreService(object service, string key)
        {
            var serviceType = service.GetType();
            var fullKey = key + "|" + serviceType.AssemblyQualifiedName;

            _services[fullKey] = service;

            if (_sut != null)
            {
                // Reset the SUT
                if (_sut is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _sut = null;
            }

            return service;
        }

        // ReSharper disable once InconsistentNaming
        protected T SUT
        {
            get
            {
                // Return the built SUT instance
                if (_sut != null)
                {
                    return _sut;
                }

                _sut = BuildSUT();

                return _sut;
            }
        }
    }
}