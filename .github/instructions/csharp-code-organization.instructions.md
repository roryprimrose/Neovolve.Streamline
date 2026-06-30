---
description: 'C# code organization conventions for the Neovolve.Streamline library.'
applyTo: '**/*.cs'
---

# C# Code Organization

These conventions complement the formatting rules enforced by `Neovolve.Streamline.slnx.DotSettings`;
where this file is silent, follow the existing style in the codebase (file-scoped namespaces, Allman
braces, `_camelCase` private fields, xmldoc on public members).

## One type per file

Each `.cs` file must contain exactly **one** top-level type declaration (class, interface,
struct, enum, or record). Nested types that are private implementation details of their
containing type are permitted; any type that is `internal` or wider must live in its own file.

The file name must match the type name.

### Generic and non-generic counterparts

When a generic type shares its base name with a non-generic counterpart, the two types still go in
separate files. To disambiguate the file names, suffix the generic type's file with its type
parameter name(s):

| Type | File |
| --- | --- |
| `TestsBase` | `TestsBase.cs` |
| `Tests<T>` | `TestsT.cs` |

This suffix is only required when both forms exist. A generic type with no non-generic counterpart
keeps the plain name (for example `Tests<T>` lives in `Tests.cs` because there is no non-generic
`Tests`). For a type with multiple parameters, concatenate the parameter names (for example
`IConverter<TIn, TOut>` → `IConverterTInTOut.cs`).

## No nested calls in arguments or indexers

Do not use a method or function call directly as an argument to another method or constructor, or
as the expression inside an indexer (`[...]`). Assign the call's result to a well-named local first,
then pass or index with that local. This keeps each call site readable and makes every intermediate
value inspectable in the debugger.

This rule covers call results used as a method/constructor argument (including array-size and
collection-index expressions). It does **not** force locals for a call that is the whole expression
(such as a `return` value or a single-expression lambda body), nor for member-access or operator
chains like `value.ToString()` or `a + b`.

```csharp
// Avoid: a call nested inside an indexer or as an argument to another call.
return services[GetCacheKey(type, key)];
var parameterValues = parameters.Select(ResolveService).ToArray();
return BuildSUT<T>(GetConstructor<T>(), parameterValues);

// Prefer: name the intermediate value, then index or pass it.
var cacheKey = GetCacheKey(type, key);
return services[cacheKey];

var constructor = GetConstructor<T>();
return BuildSUT<T>(constructor, parameterValues);
```

When a single-expression lambda would otherwise nest a call inside an argument, promote it to a
named method so the body can introduce the local.
