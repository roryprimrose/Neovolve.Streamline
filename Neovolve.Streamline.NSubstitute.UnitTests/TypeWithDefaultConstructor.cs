namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;

public class TypeWithDefaultConstructor
{
    public virtual string GetCustomValue(Guid id)
    {
        return id.ToString();
    }

    public string GetValue(Guid id)
    {
        return GetCustomValue(id);
    }
}