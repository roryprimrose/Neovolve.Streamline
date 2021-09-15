namespace Examples
{
    using System;
    using Examples.External;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class InternalTypePublicInterface : Tests<IPublicInterface>
    {
        [Fact]
        public void CanCreateAndUseInternalTypes()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            Service<IInternalScope>().GetValue(id).Returns(expected);

            var actual = SUT.GetValue(id);

            actual.Should().Be(expected);
        }

        protected override Type TargetType => typeof(InternalClassWithPublicInterface);
    }
}