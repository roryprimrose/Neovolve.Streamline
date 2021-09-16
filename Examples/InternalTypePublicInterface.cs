namespace Examples
{
    using System;
    using Examples.External;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    // This scenario is where an internal class implements and interface that is public
    // Tests<T> points to the public interface
    public class InternalTypePublicInterface : Tests<IPublicInterface>
    {
        // The actual SUT type to create is identified by the TargetType property
        protected override Type TargetType => typeof(InternalClassWithPublicInterface);

        [Fact]
        public void CanCreateAndUseInternalTypes()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            Service<IInternalScope>().GetValue(id).Returns(expected);

            var actual = SUT.GetValue(id);

            actual.Should().Be(expected);
        }
    }
}