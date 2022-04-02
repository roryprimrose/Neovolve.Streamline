using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Examples")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Examples.External;

using System;

internal class InternalClass
{
    private readonly IInternalScope _scope;

    public InternalClass(IInternalScope scope)
    {
        _scope = scope;
    }

    public string GetValue(Guid id)
    {
        return _scope.GetValue(id);
    }
}