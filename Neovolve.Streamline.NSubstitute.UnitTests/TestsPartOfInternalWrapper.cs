namespace Neovolve.Streamline.NSubstitute.UnitTests;

using global::NSubstitute;

internal class TestsPartOfInternalWrapper<T> : TestsPartOfInternal where T : class
{
    public TestsPartOfInternalWrapper(params object[] values) : base(values)
    {
    }

    public T SUT => GetSUT<T>();
}