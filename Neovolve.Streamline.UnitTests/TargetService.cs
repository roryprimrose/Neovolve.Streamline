namespace Neovolve.Streamline.UnitTests;

using System;
using System.Threading.Tasks;

public class TargetService : ITargetService
{
    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public string GetValue(Guid id)
    {
        return id.ToString();
    }
}