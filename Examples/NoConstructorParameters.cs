namespace Examples
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public interface IValueStore
    {
        string GetValue(Guid id);
    }

    public class NoConstructorParameters
    {
        public string GetValue(IValueStore store, Guid id)
        {
            return store.GetValue(id);
        }
    }

    public class NoConstructorParametersTests : Tests<NoConstructorParameters>
    {
        [Fact]
        public void CanUseServiceOnMethod()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            var store = Service<IValueStore>();

            store.GetValue(id).Returns(expected);

            var actual = SUT.GetValue(store, id);

            actual.Should().Be(expected);
        }
    }
}