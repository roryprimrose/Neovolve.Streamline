# Neovolve.Streamline
Neovolve.Streamline provides NuGet packages to simplify the unit testing setup (Arrange in AAA unit testing) of classes and their dependencies. The `Neovolve.Streamline` package provides the base logic to creating a SUT (System Under Test) along with any service dependencies that it requires. Other packages (such as `Neovolve.Streamline.NSubstitute`) provide the bridge between `Neovolve.Streamline` and another tool that creates the service dependencies.

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/LICENSE.md)&nbsp;[![Actions Status](https://github.com/roryprimrose/Neovolve.Streamline/workflows/CI/badge.svg)](https://github.com/roryprimrose/Neovolve.Streamline/actions)

|                         Package |                                                                                                               NuGet                                                                                                               |
| ------------------------------: | :-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: |
|             Neovolve.Streamline |                     [![Nuget](https://img.shields.io/nuget/v/Neovolve.Streamline.svg) ![Nuget](https://img.shields.io/nuget/dt/Neovolve.Streamline.svg)](https://www.nuget.org/packages/Neovolve.Streamline)                      |
| Neovolve.Streamline.NSubstitute | [![Nuget](https://img.shields.io/nuget/v/Neovolve.Streamline.NSubstitute.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.Streamline.NSubstitute.svg)](https://www.nuget.org/packages/Neovolve.Streamline.NSubstitute) |

- [Use Case](#use-case)
- [Advantages](#advantages)
- [Examples using NSubstitute](#examples-using-nsubstitute)
- [Supporters](#supporters)
## Use Case

Consider the following class and the setup required to unit test it.

```csharp
public class Something
{
    public Something(
        IFirst first,
        ISecond second,
        IThird third,
        IFourth fourth,
        IFifth fifth)
    {
    }

    public void FirstAction()
    {
    }

    public void SecondAction()
    {
    }

    public void ThirdAction()
    {
    }

    public void FourthAction()
    {
    }

    public void FifthAction()
    {
    }
}
```

There are five dependencies to this test class and five members to unit test. A test Arrange for any of these unit tests may look something like the following (using [NSubstitute](https://nsubstitute.github.io/) as the mocking library).

```csharp
using NSubstitute;

public class SomethingTests
{
    [Fact]
    public void FirstActionDoesXYZWhenABC()
    {
        var first = Substitute.For<IFirst>();
        var second = Substitute.For<ISecond>();
        var third = Substitute.For<IThird>();
        var fourth = Substitute.For<IFourth>();
        var fifth = Substitute.For<IFifth>();

        // Continue Arrange to configure these service for their behaviours

        var sut = new Something(first, second, third, fourth, fifth);

        // Act
        sut.FirstAction();

        // Assert
        first.Received().FirstAction();
    }
}
```

If you consider that each one of the actions has five business scenarios to validate, that is at least 25 unit test methods that duplicate the above Arrange code. The `Neovolve.Streamline.NSubstitute` package can simplify this by automatically creating both the service depedencies and the SUT itself.

For example, the above Arrange can be reduced to just the following:

```csharp
using NSubstitute;

public class SomethingTests : Tests<Something>
{
    [Fact]
    public void FirstActionDoesXYZWhenABC()
    {
        // Configure these service for their behaviours

        // Act
        SUT.FirstAction();

        // Assert
        Service<IFirst>().Received().FirstAction();
    }
}
```

## Bring your own service
Sometimes a unit test will have its own specific service that should not be created by the Streamline package. In this case, you can use the `Use()`, `Use(service)` or `Use(service, key)` methods to store your own service instance.

```csharp

public class MyFirst : IFirst
{
}

using NSubstitute;

public class SomethingTests : Tests<Something>
{
    [Fact]
    public void FirstActionDoesXYZWhenABC()
    {
        // This will create the MyFirst service instance as both MyFirst and IFirst
        Use<MyFirst>();

        // Configure these service for their behaviours

        // Act
        SUT.FirstAction();

        // Assert
        Service<IFirst>().Received().FirstAction();
    }
}
```

## Advantages

This package brings several advantages.

 - Service dependencies are automatically created
 - The SUT instance is automatically created with any required service dependencies
 - Adding, removing or re-ordering constructor parameters have limited or no impact on existing unit tests
 - Services and the SUT are automatically disposed via `IDisposable` and/or `IAsyncDisposable`
    - **NOTE:** This requires that either the test framework implicitly supports disposal or manual integration between the test framework and disposal is required.
    - xUnit implicitly supports `IDisposable` but does not yet support `IAsyncDisposable` until v3 is released. In the meantime, the test class can use `IAsyncLifetime` to support `IAsyncDisposable`. See https://github.com/xunit/xunit/issues/2017 for further information.

## Examples using NSubstitute

[SUT with multiple parameters](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/MultipleParameters.cs)  
[SUT with single parameter](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/SingleParameter.cs)  
[SUT with no constructor parameters](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/NoConstructorParameters.cs)  
[SUT with custom services](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/CustomServices.cs)  
[SUT using keyed services](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/KeyedServices.cs)  
[SUT declared as internal having public interface](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/InternalTypePublicInterface.cs)  
[SUT declared as internal](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/InternalScopedTypes.cs)  
[Using test class constructor parameters](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/TestClassConstructorParameters.cs)  
[SUT as partial mock](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/PartialSUTMock.cs)  
[SUT as full mock](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/FullSUTMock.cs)  
[SUT as abstract class](https://github.com/roryprimrose/Neovolve.Streamline/blob/main/Examples/AbstractType.cs)