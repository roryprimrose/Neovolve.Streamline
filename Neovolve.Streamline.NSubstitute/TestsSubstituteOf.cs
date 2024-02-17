namespace NSubstitute;

using System;
using System.Reflection;

/// <summary>
///     The <see cref="TestsSubstituteOf{T}" /> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <typeparam name="T">The type of the class under test.</typeparam>
/// <remarks>
///     This class uses <see cref="Substitute.For" /> to create services required when reading from
///     <see cref="Neovolve.Streamline.Tests{T}.SUT" />
///     and uses <see cref="Substitute.For" /> to create <see cref="Neovolve.Streamline.Tests{T}.SUT" /> itself.
///     This class is typically used when a full mock of the class under test is required.
/// </remarks>
public abstract class TestsSubstituteOf<T> : Tests<T> where T : class
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TestsSubstituteOf{T}" /> class.
    /// </summary>
    /// <param name="services">The services used to create the <see cref="Neovolve.Streamline.Tests{T}.SUT" />.</param>
    protected TestsSubstituteOf(params object[] services) : base(services)
    {
    }

    /// <inheritdoc />
    protected override TSUT BuildSUT<TSUT>(ConstructorInfo constructor, object[] parameterValues)
    {
        Type[] types = [TargetType ?? typeof(TSUT)];

        return (TSUT)Substitute.For(types, parameterValues);
    }
}