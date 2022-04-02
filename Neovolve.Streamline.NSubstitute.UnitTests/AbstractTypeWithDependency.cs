namespace Neovolve.Streamline.NSubstitute.UnitTests;

using System;

public abstract class AbstractTypeWithDependency
{
    private readonly ITargetService _service;

    protected AbstractTypeWithDependency(ITargetService service)
    {
        _service = service;
    }

    public string GetValue(Guid id)
    {
        return GetValueFromService(id);
    }

    public virtual string GetValueFromService(Guid id)
    {
        return _service.GetValue(id);
    }
}