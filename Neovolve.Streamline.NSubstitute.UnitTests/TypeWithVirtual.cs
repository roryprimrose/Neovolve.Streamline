namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using System;

    public class TypeWithVirtual
    {
        private readonly ITargetService _service;

        public TypeWithVirtual(ITargetService service)
        {
            _service = service;
        }

        public string GetValue(Guid id)
        {
            return GetValueEx(id);
        }

        protected internal virtual string GetValueEx(Guid id)
        {
            return _service.GetValue(id);
        }
    }
}