namespace NSubstitute;

using System;

/// <summary>
/// The <see cref="Tests{T}"/> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <typeparam name="T">The type of the class under test.</typeparam>
/// <remarks>
/// This class uses <see cref="Substitute.For"/> to create services required when reading from <see cref="Neovolve.Streamline.Tests{T}.SUT" />.
/// This class is typically used when the class under test is a concrete class.
/// </remarks>
public abstract class Tests<T> : Neovolve.Streamline.Tests<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tests{T}"/> class.
    /// </summary>
    /// <param name="services">The services used to create the <see cref="Neovolve.Streamline.Tests{T}.SUT"/>.</param>
    protected Tests(params object[] services) : base(services)
    {
    }

    /// <inheritdoc />
    /// <remarks>This member uses NSubstitute.For to create the service.</remarks>
    protected override object BuildService(Type type, string key)
    {
        var types = new[] {type};
        var parameters = Array.Empty<object>();

        return Substitute.For(types, parameters);
    }
}