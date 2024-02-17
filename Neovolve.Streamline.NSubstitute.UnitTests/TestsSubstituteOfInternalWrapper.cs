namespace Neovolve.Streamline.NSubstitute.UnitTests;

using global::NSubstitute;

internal class TestsSubstituteOfInternalWrapper<T> : TestsSubstituteOfInternal where T : class
{
    public TestsSubstituteOfInternalWrapper(params object[] values) : base(values)
    {
    }

    public T SUT => GetSUT<T>();
}