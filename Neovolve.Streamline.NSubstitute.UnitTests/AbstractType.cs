namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;

public abstract class AbstractType
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