namespace Neovolve.Streamline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class Test<T> where T : class
    {
        private readonly Dictionary<string, object> _services = new();
        private T _sut;

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
            return ResolveService(parameter.ParameterType, nameof(ParameterInfo));
        }

        protected object ResolveService(Type type, string key)
        {
            var fullKey = key + "|" + type.AssemblyQualifiedName;

            if (_services.ContainsKey(fullKey))
            {
                return _services[fullKey];
            }

            var service = BuildService(type, key);

            _services[fullKey] = service;

            return service;
        }

        protected TService Service<TService>()
        {
            // By default the service to return is the one created for a constructor parameter
            return Service<TService>(nameof(ParameterInfo));
        }

        protected TService Service<TService>(string key)
        {
            return (TService) ResolveService(typeof(TService), key);
        }

        // ReSharper disable once InconsistentNaming
        public T SUT
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