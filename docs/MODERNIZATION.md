# Windows 11 modernization strategy

## Current state

Sonnenberg is a .NET Framework 4.8 solution built around SharpShell 2.7.2, an in-process legacy shell extension, a Windows service, and a WiX 3 installer. The current implementation can build x86 and x64 assemblies, but it has no ARM64 solution configuration and its installer source still contains placeholder GUIDs and manufacturer data. A successful solution build therefore does not prove that a deployable MSI was produced.

The maintenance branch updates the transitional WiX dependency to the patched 3.14.1 release and replaces the end-of-life log4net 2.0.17 dependency with log4net 3.5.0 across all eleven legacy manifests and their project references. log4net 3.5.0 targets .NET Framework 4.6.2 and is compatible with the projects' .NET Framework 4.8 target.

SharpShell's legacy context-menu handler remains useful through Windows 11's **Show more options** menu. It is not a future-proof route into the primary Windows 11 context menu.

## Target architecture

1. Keep the existing SharpShell handler as a transitional x86/x64 adapter.
2. Move all file classification, path formatting, command selection, and line-counting behavior into `Sonnenberg.Core`, which targets the current .NET 10 LTS baseline and has no Windows UI dependencies.
3. Add a small native x64 and ARM64 COM host that implements `IExplorerCommand`. Register it through an MSIX package manifest (or a sparse package while an unpackaged transition is required).
4. Keep Explorer callbacks fast. Expensive work and UI should be delegated to a separate process through a narrow, versioned protocol.
5. Replace the WiX 3 placeholder installer with signed MSIX packages. Retain MSI only as a transitional installer for the legacy handler.

The native host must be compiled separately for x64 and ARM64 because an in-process Explorer extension must match Explorer's architecture. An AnyCPU managed assembly is not an ARM64 Explorer integration.

## macOS-first development

Portable behavior belongs in `Sonnenberg.Core` and is tested on macOS, Linux, Windows x64, and Windows ARM64. The first extracted seams are path formatting and line counting. Windows-specific projects should become thin adapters and remain testable through interfaces and fakes.

Daily development on macOS can cover:

- core behavior and unit tests;
- command/manifest schema validation;
- package metadata generation;
- protocol and serialization tests;
- static analysis and dependency updates.

Native COM registration, Explorer activation, installer behavior, icon rendering, elevation, and service lifecycle still require Windows.

## Delivery phases

### Phase 0: make the legacy baseline reproducible

- restore pinned NuGet dependencies;
- build x86 and x64 on hosted Windows runners;
- upload unsigned binary artifacts for review;
- run portable core tests on all four supported CI environments;
- document that the WiX installer is not yet release-ready.

### Phase 1: extract behavior

- migrate `CopyPath`, `CountLines`, file classification, and command construction into `Sonnenberg.Core`;
- add focused tests before changing the Explorer host;
- remove unused NUnit/Moq references from production projects.

### Phase 2: modern Explorer adapter (started)

- implement `IExplorerCommand` in a native C++ host for x64 and ARM64;
- keep the host limited to selection parsing and command dispatch;
- register both architectures in MSIX manifests;
- use package identity for the primary Windows 11 context menu.

The first adapter now implements a single **Copy path** command without third-party dependencies. CI compiles and loads the DLL natively on x64 and ARM64, exercises its COM class factory and command metadata, and publishes unsigned DLLs with SHA-256 checksums. `packaging/AppxManifest.template.xml` records the intended packaged COM and File Explorer registration, but deliberately retains explicit identity, publisher, architecture, entry-point, and asset placeholders. It is not installable until Phase 3 supplies those values and signing.

### Phase 3: packaging and signing

- produce x64 and ARM64 MSIX packages in GitHub Actions;
- upload unsigned PR artifacts, but publish only signed release artifacts;
- add certificate-subject and package-identity checks;
- retire the WiX 3 package after the migration window.

## CI support matrix

| Component | macOS | Linux | Windows x64 | Windows ARM64 |
| --- | --- | --- | --- | --- |
| `Sonnenberg.Core` build/tests | yes | yes | yes | yes |
| Legacy .NET Framework solution | no | no | x86 + x64 | no |
| Modern native Explorer host | source work | static checks | build + smoke test | build + smoke test |
| Explorer integration test | no | no | manual | manual |

GitHub-hosted runners are free for public repositories. The workflow intentionally calls the ARM64 output a portable-core artifact; it does not claim that the current SharpShell DLL is an ARM64 shell extension.

## Merge gates

- all portable tests pass on every matrix entry;
- native x64 and ARM64 builds load successfully and pass the COM smoke tests;
- x86 and x64 legacy builds pass;
- no installer is called release-ready while legacy `YOUR_*` or modern `REPLACE_WITH_*` metadata remains;
- the manual Windows 11 checklist passes on physical or otherwise representative x64 and ARM64 machines before a packaged release.

## References

- [Microsoft: integrate a packaged desktop app with File Explorer](https://learn.microsoft.com/windows/apps/desktop/modernize/integrate-packaged-app-with-file-explorer)
- [Microsoft: desktop-to-UWP/MSIX extensions](https://learn.microsoft.com/windows/apps/desktop/modernize/desktop-to-uwp-extensions)
- [Microsoft: .NET Framework system requirements](https://learn.microsoft.com/dotnet/framework/get-started/system-requirements)
- [GitHub: hosted runner images and Windows ARM64 labels](https://docs.github.com/actions/reference/runners/github-hosted-runners)
- [Apache log4net release notes](https://logging.apache.org/log4net/release-notes.html)
- [NuGet: log4net 3.5.0](https://www.nuget.org/packages/log4net/3.5.0)
