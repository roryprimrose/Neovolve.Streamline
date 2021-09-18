namespace NSubstitute
{
    using System;
    using System.Reflection;

    public abstract class TestsSubstituteOf<T> : Tests<T> where T : class
    {
        protected TestsSubstituteOf(params object[] services) : base(services)
        {
        }
        
        protected override T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
        {
            Type[] types = { TargetType };

            return (T)Substitute.For(types, parameterValues);
        }
    }
}