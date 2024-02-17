namespace Neovolve.Streamline.UnitTests;

using System;
using NSubstitute;

public class Wrapper<T> : Tests<T> where T : class
{
    public Wrapper(params object[] values) : base(values)
    {
    }

    protected override object BuildService(Type type, string key)
    {
        var types = new[] { type };
        var parameters = Array.Empty<object>();

        return Substitute.For(types, parameters);
    }
}