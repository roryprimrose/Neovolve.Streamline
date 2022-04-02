namespace Neovolve.Streamline.UnitTests;

using System;
using Microsoft.Extensions.Logging;

public class Target : IDisposable
{
    private readonly ILogger _logger;
    private readonly ITargetService _service;

    public Target(ITargetService service, ILogger logger)
    {
        _service = service;
        _logger = logger;
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public string GetValue(Guid id)
    {
        _logger.LogInformation($"Calling GetValue with {id}");

        return GetValueInternal(id);
    }

    protected internal virtual string GetValueInternal(Guid id)
    {
        _logger.LogInformation($"Calling GetValueInternal with {id}");

        return _service.GetValue(id);
    }

    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("Calling Dispose");

        if (disposing)
        {
        }
    }
}