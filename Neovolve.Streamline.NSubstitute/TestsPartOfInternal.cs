namespace NSubstitute;

using System.Reflection;
using Neovolve.Streamline;

/// <summary>
///     The <see cref="TestsPartOfInternal" /> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <remarks>
///     This class uses <see cref="Substitute.For" /> to create services required when building the SUT
///     and uses <see cref="Substitute.ForPartsOf{T}" /> to create <see cref="TestsBase.GetSUT{T}()" /> itself.
///     This class is typically used when a partial mock of the class under test is required.
/// </remarks>
public abstract class TestsPartOfInternal : TestsInternal
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TestsPartOfInternal" /> class.
    /// </summary>
    /// <param name="services">The services used to create the SUT.</param>
    protected TestsPartOfInternal(params object[] services) : base(services)
    {
    }

    /// <inheritdoc />
    protected override TSUT BuildSUT<TSUT>(ConstructorInfo constructor, object[] parameterValues)
    {
        return Substitute.ForPartsOf<TSUT>(parameterValues);
    }
}