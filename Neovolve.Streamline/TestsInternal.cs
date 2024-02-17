namespace Neovolve.Streamline;

/// <summary>
///     The <see cref="TestsInternal" /> class is used to define a base class for unit tests that configures a single
///     system
///     under test (SUT) instance.
/// </summary>
/// <remarks>
///     This class provides a set of helper methods for creating and managing the SUT instance, as well as any dependencies
///     that it may have.
/// </remarks>
public abstract class TestsInternal : TestsBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TestsInternal" /> class.
    /// </summary>
    /// <param name="services">The services to use when creating the SUT.</param>
    protected TestsInternal(params object[] services) : base(services)
    {
    }
}