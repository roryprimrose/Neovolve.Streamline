namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;

public abstract class AbstractType
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