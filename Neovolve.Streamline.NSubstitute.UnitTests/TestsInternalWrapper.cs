namespace Neovolve.Streamline.NSubstitute.UnitTests;

using global::NSubstitute;

internal class TestsInternalWrapper<T> : TestsInternal where T : class
{
    public TestsInternalWrapper(params object[] values) : base(values)
    {
    }

    public T SUT => GetSUT<T>();
}