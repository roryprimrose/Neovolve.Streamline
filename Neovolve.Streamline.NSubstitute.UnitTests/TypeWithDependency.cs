namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using System;

    public class TypeWithDependency
    {
        private readonly ITargetService _service;

        public TypeWithDependency(ITargetService service)
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
}