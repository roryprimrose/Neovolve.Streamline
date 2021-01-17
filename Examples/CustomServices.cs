namespace Examples
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public interface ICustomService
    {
        string GetValue(Guid id);
    }

    public class CustomServices
    {
        private readonly ICustomService _service;

        public CustomServices(ICustomService service)
        {
            _service = service;
        }

        public string GetValue(Guid id)
        {
            return _service.GetValue(id);
        }
    }

    public class CustomServicesTests : Test<CustomServices>
    {
        [Fact]
        public void CanUseCustomService()
        {
            var id = Guid.NewGuid();
            var wrapper = new Wrapper();

            Use(wrapper);

            var actual = SUT.GetValue(id);

            actual.Should().Be(id.ToString());
        }

        private class Wrapper : ICustomService
        {
            public string GetValue(Guid id)
            {
                return id.ToString();
            }
        }
    }
}