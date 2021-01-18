namespace Neovolve.Streamline.NSubstitute.UnitTests
{
    using global::NSubstitute;

    public class Wrapper : Tests<Target>
    {
        public Wrapper(params object[] values) : base(values)
        {
        }
    }
}