namespace Neovolve.Streamline;

/// <summary>
///     The <see cref="Tests{T}" /> class is used to define a base class for unit tests that configures a single system
///     under test (SUT) instance.
/// </summary>
/// <typeparam name="T">The type of the system under test.</typeparam>
/// <remarks>
///     This class provides a set of helper methods for creating and managing the SUT instance, as well as any dependencies
///     that it may have.
/// </remarks>
public abstract class Tests<T> : TestsBase where T : class
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Tests{T}" /> class.
    /// </summary>
    /// <param name="services">The services to use when creating the SUT.</param>
    protected Tests(params object[] services) : base(services)
    {
    }

    /// <summary>
    ///     Gets the system under test (SUT) instance for the current test.
    /// </summary>
    /// <remarks>
    ///     This property will return the same SUT instance for the lifetime of the class unless there are any changes made by
    ///     using the <see cref="TestsBase.Service{TService}()" /> or <see cref="TestsBase.Use{TService}()" /> methods.
    /// </remarks>
    public T SUT => GetSUT<T>();
}