namespace Examples;

using System;
using Examples.External;
using FluentAssertions;
using NSubstitute;
using Xunit;

// As InternalClass and IInternalScope in this example are declared as internal in a different project, we need to:
// 1. The project that declares the internal types to be tested needs to include [assembly:InternalsVisibleTo("<APPLICATION_PROJECT>")] in a .cs file
//      or <InternalsVisibleTo Include="<APPLICATION_PROJECT>" /> in the .csproj file
//      so that the test project can reference the types
// 2. xUnit test classes need to be public and therefore cannot inherit from Test<T> where T is declared as internal. Instead, it can inherit from TestsInternal
//      and then create its own `public TypeToTest SUT => GetSUT<TypeToTest>();` property to access the internal type which is what Tests<T> already does for public types.

public class InternalScopedTypes : TestsInternal
{
    [Fact]
    public void CanCreateAndUseInternalTypes()
    {
        var id = Guid.NewGuid();
        var expected = Guid.NewGuid().ToString();

        // Proxy is used here in a public xUnit test class in order to run against the Test<T> that points to an internal type
        Service<IInternalScope>().GetValue(id).Returns(expected);

        var actual = SUT.GetValue(id);

        actual.Should().Be(expected);
    }

    private InternalClass SUT => GetSUT<InternalClass>();
}