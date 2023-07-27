namespace NSubstitute;

using System;
using System.Reflection;

/// <summary>
/// The <see cref="TestsSubstituteOf{T}"/> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <typeparam name="T">The type of the class under test.</typeparam>
/// <remarks>
/// This class uses <see cref="Substitute.For"/> to create services required when reading from <see cref="SUT" />
/// and uses <see cref="Substitute.For"/> to create <see cref="SUT" /> itself.
/// This class is typically used when a full mock of the class under test is required.
/// </remarks>
public abstract class TestsSubstituteOf<T> : Tests<T> where T : class
{
    protected TestsSubstituteOf(params object[] services) : base(services)
    {
    }
        
    /// <inheritdoc />
    protected override T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
    {
        Type[] types = { TargetType };

        return (T)Substitute.For(types, parameterValues);
    }
}