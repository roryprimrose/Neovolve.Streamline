namespace Neovolve.Streamline.NSubstitute
{
    using System;
    using global::NSubstitute;

    public abstract class Test<T> : Streamline.Test<T> where T : class
    {
        protected override object BuildService(Type type, string key)
        {
            var types = new[] {type};
            var parameters = Array.Empty<object>();

            return Substitute.For(types, parameters);
        }
    }
}