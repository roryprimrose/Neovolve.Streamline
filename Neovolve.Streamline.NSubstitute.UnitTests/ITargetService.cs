namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using System;

    public interface ITargetService
    {
        string GetValue(Guid id);
    }
}