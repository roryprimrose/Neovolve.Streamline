namespace Examples.External;

using System;

internal class InternalClassWithPublicInterface : IPublicInterface
{
    private readonly IInternalScope _scope;

    public InternalClassWithPublicInterface(IInternalScope scope)
    {
        _scope = scope;
    }

    public string GetValue(Guid id)
    {
        return _scope.GetValue(id);
    }
}