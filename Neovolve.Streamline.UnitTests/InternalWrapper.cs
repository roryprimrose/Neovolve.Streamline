namespace Neovolve.Streamline.UnitTests;

using System;
using NSubstitute;

public class InternalWrapper : TestsInternal
{
    public InternalWrapper(params object[] values) : base(values)
    {
    }

    protected override object BuildService(Type type, string key)
    {
        var types = new[] { type };
        var parameters = Array.Empty<object>();

        return Substitute.For(types, parameters);
    }

    public Target SUT => GetSUT<Target>();
}