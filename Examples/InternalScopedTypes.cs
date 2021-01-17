namespace Examples
{
    using System;
    using Examples.External;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class InternalScopedTypes
    {
        // As InternalClass and IInternalScope are declared as internal in a different project, we need to do two things
        // 1. The project that declares them needs to include [assembly:InternalsVisibleTo("<THIS_PROJECT>")]
        // 2. This test class needs to create an implementation of Test<T> that is less visible than internal (therefore private so that it compiles)
        //      but still available within a public xUnit test class. This is achieved by using a simple private wrapper class internally
        //      This wrapper idea comes from https://pedro.digitaldias.com/?p=494
        private readonly Wrapper _wrapper = new Wrapper();

        private class Wrapper : Test<InternalClass>
        {

        }

        [Fact]
        public void CanCreateAndUseInternalTypes()
        {
            var id = Guid.NewGuid();
            var expected = Guid.NewGuid().ToString();

            _wrapper.Service<IInternalScope>().GetValue(id).Returns(expected);

            var actual = _wrapper.SUT.GetValue(id);

            actual.Should().Be(expected);
        }
    }
}