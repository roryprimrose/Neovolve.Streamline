# Neovolve.Streamline
Provides NuGet packages to make testing easy for classes with dependencies. The Neovolve.Streamline package provides the base logic to creating a SUT (System Under Test) along with any service dependencies that it requires. Other packages (such as Neovolve.Streamline.NSubstitute) provide the bridge between Neovolve.Streamline and another tool that creates the service dependencies.

## Examples

[SUT with multiple parameters](Examples/MultipleParameters.cs)  
[SUT with single parameter](Examples/SingleParameter.cs)  
[SUT with no constructor parameters](Examples/NoConstructorParameters.cs)  
[SUT with custom services](Examples/CustomServices.cs)  
[SUT using keyed services](Examples/KeyedServices.cs)  
[SUT declared as internal](Examples/InternalScopedTypes.cs)  
[Using test class constructor parameters](Examples/TestClassConstructorParameters.cs)  
