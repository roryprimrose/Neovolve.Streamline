namespace NSubstitute;

using System;
using Neovolve.Streamline;

/// <summary>
///     The <see cref="TestsInternal" /> class defines the base class for NSubstitute-based tests.
/// </summary>
/// <remarks>
///     This class uses <see cref="Substitute.For" /> to create services required when reading from
///     <see cref="TestsBase.GetSUT{T}" />.
///     This class is typically used when the class under test is a concrete class.
/// </remarks>
public abstract class TestsInternal : Neovolve.Streamline.TestsInternal
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TestsInternal" /> class.
    /// </summary>
    /// <param name="services">The services used to create the <see cref="TestsBase.GetSUT{T}" />.</param>
    protected TestsInternal(params object[] services) : base(services)
    {
    }

    /// <inheritdoc />
    /// <remarks>This member uses NSubstitute.For to create the service.</remarks>
    protected override object BuildService(Type type, string key)
    {
        var types = new[] { type };
        var parameters = Array.Empty<object>();

        return Substitute.For(types, parameters);
    }
}