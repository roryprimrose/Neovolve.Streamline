namespace NSubstitute
{
    using System;
    using System.Reflection;

    public abstract class Tests<T> : Neovolve.Streamline.Tests<T> where T : class
    {
        protected Tests(params object[]? services) : base(services)
        {
        }

        protected override object BuildService(Type type, string key)
        {
            var types = new[] {type};
            var parameters = Array.Empty<object>();

            return Substitute.For(types, parameters);
        }

        protected override T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
        {
            return Substitute.ForPartsOf<T>(parameterValues);
        }
    }
}