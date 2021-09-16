namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using global::NSubstitute;

    public class TestsPartOfWrapper<T> : TestsPartOf<T> where T : class
    {
        public TestsPartOfWrapper(params object[] values) : base(values)
        {
        }
    }
}