namespace NSubstitute
{
    using System;

    public abstract class Test<T> : Neovolve.Streamline.Test<T> where T : class
    {
        protected override object BuildService(Type type, string key)
        {
            var types = new[] {type};
            var parameters = Array.Empty<object>();

            return Substitute.For(types, parameters);
        }
    }
}