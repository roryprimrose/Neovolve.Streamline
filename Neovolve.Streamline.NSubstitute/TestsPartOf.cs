namespace NSubstitute;

using System.Reflection;

/// <summary>
/// The <see cref="TestsPartOf{T}"/> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <typeparam name="T">The type of the class under test.</typeparam>
/// <remarks>
/// This class uses <see cref="Substitute.For"/> to create services required when reading from <see cref="SUT" />
/// and uses <see cref="Substitute.ForPartsOf"/> to create <see cref="SUT" /> itself.
/// This class is typically used when a partial mock of the class under test is required.
/// </remarks>
public abstract class TestsPartOf<T> : Tests<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestsPartOf{T}"/> class.
    /// </summary>
    /// <param name="services">The services used to create the <see cref="SUT" />.</param>
    protected TestsPartOf(params object[] services) : base(services)
    {
    }

    /// <inheritdoc />
    protected override T BuildSUT(ConstructorInfo constructor, object[] parameterValues)
    {
        return Substitute.ForPartsOf<T>(parameterValues);
    }
}