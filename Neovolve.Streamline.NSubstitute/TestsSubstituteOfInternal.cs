namespace NSubstitute;

using System;
using System.Reflection;
using Neovolve.Streamline;

/// <summary>
///     The <see cref="TestsSubstituteOfInternal" /> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <remarks>
///     This class uses <see cref="Substitute.For" /> to create services required when creating the SUT
///     and uses <see cref="Substitute.For" /> to create <see cref="TestsBase.GetSUT{T}()" /> itself.
///     This class is typically used when a full mock of the class under test is required.
/// </remarks>
public abstract class TestsSubstituteOfInternal : TestsInternal
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TestsSubstituteOfInternal" /> class.
    /// </summary>
    /// <param name="services">The services used to create the SUT.</param>
    protected TestsSubstituteOfInternal(params object[] services) : base(services)
    {
    }

    /// <inheritdoc />
    protected override TSUT BuildSUT<TSUT>(ConstructorInfo constructor, object?[] parameterValues)
    {
        Type[] types = [TargetType ?? typeof(TSUT)];

        return (TSUT)Substitute.For(types, parameterValues);
    }
}