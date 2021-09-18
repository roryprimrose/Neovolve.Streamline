namespace Examples
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public interface IFullMockService
    {
        string GetValue(Guid id);
    }

    public class FullSUTMock
    {
        private readonly IFullMockService _service;

        public FullSUTMock(IFullMockService service)
        {
            _service = service;
        }

        public string GetValue(Guid id)
        {
            // Here we would like to make sure that we're able to stub other virtual methods of the same class.
            // We wanna be sure that we get a partial mock which is useful for checking a scenario
            // in which a method knows an algorithm. It doesn't know any lower level details
            // it just does something according to some algorithm.
            // In that algorithm it may call other services which were injected and we can stub(we're not concerned about those here)
            // it also may call some other members of this class which could be virtual or quite often they're
            // "protected virtual internal" those methods just may do some low level things so that our algorithm
            // looks nice and clean.
            // 
            // In this particular example the algorithm is pretty dumb it just delegates call to GetValueEx
            // We would like be able to stub completely the call if we wish.
            return GetValueEx(id);
        }

        protected internal virtual string GetValueEx(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    public class FullMockTests : TestsSubstituteOf<FullSUTMock>
    {
        [Fact]
        public void MethodIsNotSubbed_OriginalImplementationWasNotCalled()
        {
            var id = Guid.NewGuid();
            var wrapper = new Wrapper();

            Use(wrapper);

            var actual = SUT.GetValue(id);

            actual.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void MethodIsSubbed_TheValueWhichWasSetOnStubbedIsReturned()
        {
            var id = Guid.NewGuid();
            var wrapper = new Wrapper();

            Use(wrapper);

            SUT.GetValueEx(id).Returns("42");

            var actual = SUT.GetValue(id);

            actual.Should().Be("42");
        }

        private class Wrapper : IFullMockService
        {
            public string GetValue(Guid id)
            {
                return id.ToString();
            }
        }
    }
}