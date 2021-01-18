namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using System;

    public class Target
    {
        private readonly ITargetService _service;

        public Target(ITargetService service)
        {
            _service = service;
        }

        public string GetValue(Guid id)
        {
            return GetValueInternal(id);
        }

        protected internal virtual string GetValueInternal(Guid id)
        {
            return _service.GetValue(id);
        }
    }
}