namespace Neovolve.Streamline.UnitTests
{
    using System;
    using NSubstitute;

    public class Wrapper : Test<Target>
    {
        protected override object BuildService(Type type, string key)
        {
            var types = new[] {type};
            var parameters = Array.Empty<object>();

            return Substitute.For(types, parameters);
        }
    }
}