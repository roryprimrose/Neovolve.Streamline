namespace Neovolve.Streamline.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class TestTests
    {
        [Fact]
        public void UseThrowsExceptionWithNullService()
        {
            var sut = new Wrapper();

            Action action = () => sut.Use<object>(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}