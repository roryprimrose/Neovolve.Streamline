namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using global::NSubstitute;

    public class TestsSubstituteOfWrapper<T> : TestsSubstituteOf<T> where T : class
    {
        public TestsSubstituteOfWrapper(params object[] values) : base(values)
        {
        }
    }
}