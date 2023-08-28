namespace Neovolve.Streamline.UnitTests;

using System;

public interface ITargetService : IDisposable, IAsyncDisposable
{
    string GetValue(Guid id);
}