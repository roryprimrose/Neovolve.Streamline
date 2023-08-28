namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;

public class TypeWithDefaultConstructor
{
    public string GetValue(Guid id)
    {
        return GetCustomValue(id);
    }

    public virtual string GetCustomValue(Guid id)
    {
        return id.ToString();
    }
}