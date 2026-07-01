namespace Neovolve.Streamline.UnitTests;

using System;

public class ProviderWrapper : Wrapper
{
    public ProviderWrapper(params object[] values) : base(values)
    {
    }

    public IServiceProvider Provider => ServiceProvider;
}
