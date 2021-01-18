namespace Neovolve.Streamline.UnitTests
{
    using System;

    public interface ITargetService : IDisposable
    {
        string GetValue(Guid id);
    }
}