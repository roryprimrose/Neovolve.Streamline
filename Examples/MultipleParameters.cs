namespace Examples
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public interface IFirstService
    {
        int GetValue();
    }

    public interface ISecondService
    {
        int GetValue();
    }

    public interface IThirdService
    {
        int GetValue();
    }

    public interface IFourthService
    {
        string GetMessage(int value);
    }

    public class MultipleParameters
    {
        private readonly IFirstService _firstService;
        private readonly IFourthService _fourthService;
        private readonly ISecondService _secondService;
        private readonly IThirdService _thirdService;

        public MultipleParameters(IFirstService firstService, ISecondService secondService, IThirdService thirdService,
            IFourthService fourthService)
        {
            _firstService = firstService;
            _secondService = secondService;
            _thirdService = thirdService;
            _fourthService = fourthService;
        }

        public string BuildMessage()
        {
            var firstValue = _firstService.GetValue();
            var secondValue = _secondService.GetValue();
            var thirdValue = _thirdService.GetValue();

            var total = firstValue + secondValue + thirdValue;

            return _fourthService.GetMessage(total);
        }
    }

    public class MultipleParametersTest : Tests<MultipleParameters>
    {
        [Fact]
        public void CanSupportMultipleServices()
        {
            var expected = Guid.NewGuid().ToString();

            Service<IFirstService>().GetValue().Returns(3);
            Service<ISecondService>().GetValue().Returns(8);
            Service<IThirdService>().GetValue().Returns(43);
            Service<IFourthService>().GetMessage(54).Returns(expected);

            var actual = SUT.BuildMessage();

            actual.Should().Be(expected);
        }
    }
}