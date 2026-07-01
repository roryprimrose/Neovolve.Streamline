namespace Neovolve.Streamline.UnitTests;

using System;

public class ProviderConsumer
{
    public ProviderConsumer(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }
}
