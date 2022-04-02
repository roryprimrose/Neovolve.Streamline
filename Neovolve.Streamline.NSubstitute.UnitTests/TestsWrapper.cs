namespace Neovolve.Streamline.NSubstitute.UnitTests;

using global::NSubstitute;

public class TestsWrapper<T> : Tests<T> where T : class
{
    public TestsWrapper(params object[] values) : base(values)
    {
    }
}