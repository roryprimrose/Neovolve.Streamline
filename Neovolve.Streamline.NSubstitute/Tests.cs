namespace NSubstitute
{
    using System;
    using System.Reflection;

    public abstract class Tests<T> : Neovolve.Streamline.Tests<T> where T : class
    {
        protected Tests(params object[] services) : base(services)
        {
        }

        protected virtual TValue BuildPartialSUT<TValue>(ConstructorInfo constructor, object[] parameterValues)
            where TValue : class, T
        {
            return Substitute.ForPartsOf<TValue>(parameterValues);
        }

        protected override object BuildService(Type type, string key)
        {
            var types = new[] { type };
            var parameters = Array.Empty<object>();

            return Substitute.For(types, parameters);
        }

        protected override T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
        {
            Type[] types = { TargetType };

            return (T)Substitute.For(types, parameterValues);
        }
    }
}