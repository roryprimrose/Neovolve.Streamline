namespace NSubstitute;

using System.Reflection;

public abstract class TestsPartOf<T> : Tests<T> where T : class
{
    protected TestsPartOf(params object[] services) : base(services)
    {
    }

    protected override T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
    {
        return Substitute.ForPartsOf<T>(parameterValues);
    }
}