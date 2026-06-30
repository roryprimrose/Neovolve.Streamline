---
name: update-dependencies
description: Updates this repository's dependencies — the global.json .NET SDK version (when present), NuGet package versions in Directory.Packages.props, and GitHub Actions versions in .github/workflows. Use when asked to update dependencies, bump packages, upgrade the SDK, refresh GitHub Actions, or check for outdated dependencies.
---

# Update dependencies

Updates three dependency surfaces in `Neovolve.Streamline`, then validates with a build and test run. Make one logical commit per surface unless the user asks otherwise. Never push without being asked.

## 1. .NET SDK (global.json)

1. If a [global.json](../../../global.json) exists, read its `sdk.version`; if it does not exist yet, skip this surface.
2. Find the latest patch of that feature band: `dotnet sdk check`, or list installed with `dotnet --list-sdks`. Prefer the latest stable feature band that CI targets (`10.x`).
3. Update `sdk.version`; keep `rollForward` and any `test.runner` block unchanged.

## 2. NuGet packages (central versions)

All versions are centrally pinned in [Directory.Packages.props](../../../Directory.Packages.props).

1. List outdated packages: `dotnet list package --outdated`. Add `--include-transitive` to review pinned transitives.
2. Bump `Version` attributes in `Directory.Packages.props`. Keep any per-TFM pins aligned to their target framework conditions.
3. Do not add or remove a pin for a package nothing references — only update versions that are actually consumed.

## 3. GitHub Actions

Pinned in `.github/workflows/*.yml` (e.g. `actions/checkout`, `actions/setup-dotnet`, `gittools/actions`).

1. For each `uses: owner/repo@vN`, find the latest release tag: `gh release view --repo owner/repo` or `gh api repos/owner/repo/releases/latest --jq .tag_name` (without `gh`, fetch `https://github.com/owner/repo/releases/latest`). Compare against the pinned tag.
2. Bump the major tag (e.g. `@v7`) when a newer one exists. Keep `dotnet-version` ranges matching the repository's target frameworks unless an SDK bump changes them.

## 4. Validate

```pwsh
dotnet build Neovolve.Streamline.slnx
dotnet test Neovolve.Streamline.UnitTests\Neovolve.Streamline.UnitTests.csproj
dotnet test Neovolve.Streamline.NSubstitute.UnitTests\Neovolve.Streamline.NSubstitute.UnitTests.csproj
```

Report what changed (old → new per item) and any majors skipped for being breaking. Keep any temporary files under a `.scratch/` folder.
