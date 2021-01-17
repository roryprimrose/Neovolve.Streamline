# Neovolve.Streamline
Provides NuGet packages to make testing easy for classes with dependencies. The Neovolve.Streamline package provides the base logic to creating a SUT (System Under Test) along with any service dependencies that it requires. Other packages (such as Neovolve.Streamline.NSubstitute) provide the bridge between Neovolve.Streamline and another tool that creates the service dependencies.

## Examples

[SUT with single dependency](Examples/SingleParameter.cs)  
[SUT declared as internal](Examples/InternalScopedTypes.cs)