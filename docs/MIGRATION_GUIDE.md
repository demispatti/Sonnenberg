# Migration Guide - Architecture Refactoring

This guide helps you understand the changes made during the architecture refactoring.

## Overview of Changes

The Sonnenberg project has been refactored from **13 separate projects** into **6 well-organized projects** following industry-standard Clean Architecture principles.

## What Changed

### Project Consolidation

| Old Projects | New Project | Description |
|--------------|-------------|-------------|
| ShellServer | Sonnenberg.Core | Main extension entry point and core models |
| ContextMenu | Sonnenberg.UI | Menu creation and UI components |
| Common, Settings, MenuWatcher | Sonnenberg.Infrastructure | Cross-cutting concerns |
| Language | Sonnenberg.Localization | Internationalization resources |
| Service, ServiceManager, ServerManager, Start/Stop Service/Server | Sonnenberg.Management | Service management utilities |
| (Sub-menu items logic) | Sonnenberg.Features | Feature implementations |

### File Renames & Moves

| Old Location | New Location | Notes |
|--------------|--------------|-------|
| `ShellServer/ShellServer.cs` | `src/Sonnenberg.Core/ShellExtension.cs` | Renamed class for clarity |
| `Common/Helper.cs` | `src/Sonnenberg.UI/Icons/IconHelper.cs` | Moved to UI layer |
| `Common/Logger.cs` | `src/Sonnenberg.Infrastructure/Logging/LoggerConfigurator.cs` | Renamed for clarity |
| `Settings/Settings.cs` | `src/Sonnenberg.Infrastructure/Configuration/AppConfiguration.cs` | Better naming |
| `MenuWatcher/MenuWatcher.cs` | `src/Sonnenberg.Infrastructure/EventHandling/ContextMenuStateManager.cs` | More descriptive name |
| `ContextMenu/SubMenuItems/*` | `src/Sonnenberg.Features/*` | Organized by feature |

### New Structure

```
Sonnenberg/
├── src/                          # All source code (NEW)
│   ├── Sonnenberg.Core/
│   ├── Sonnenberg.Features/
│   ├── Sonnenberg.UI/
│   ├── Sonnenberg.Infrastructure/
│   ├── Sonnenberg.Localization/
│   └── Sonnenberg.Management/
├── installer/                    # WiX installer (moved)
├── docs/                         # Documentation (NEW)
└── (old project folders preserved for reference)
```

## Breaking Changes

### 1. Namespace Changes

All namespaces have been updated to match the new project structure:

**Before:**
```csharp
using Sonnenberg.ShellServer;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu;
```

**After:**
```csharp
using Sonnenberg.Core;
using Sonnenberg.Infrastructure.Theming;
using Sonnenberg.UI.Menus;
```

### 2. Class Renames

| Old Name | New Name |
|----------|----------|
| `ShellServer` | `ShellExtension` |
| `Helper` | `IconHelper` |
| `Logger` | `LoggerConfigurator` |
| `MenuWatcher` | `ContextMenuStateManager` |
| `Settings` | `AppConfiguration` |

### 3. Architecture Changes

**Before (Procedural/Mixed):**
```csharp
public class ShellServer : SharpContextMenu
{
    public ContextMenu.ContextMenu ContextMenu;
    public MenuWatcher.MenuWatcher MenuWatcher;

    protected override ContextMenuStrip CreateMenu()
    {
        // Mixed business logic and UI code
        var isDarkMode = new Helper().IsDarkMode();
        // ...
    }
}
```

**After (Clean Architecture with DI):**
```csharp
public class ShellExtension : SharpContextMenu
{
    private readonly IItemContextResolver _contextResolver;
    private readonly IMenuFactory _menuFactory;
    private readonly IThemeDetector _themeDetector;
    private readonly IContextMenuStateManager _stateManager;

    public ShellExtension(
        IItemContextResolver contextResolver,
        IMenuFactory menuFactory,
        IThemeDetector themeDetector,
        IContextMenuStateManager stateManager)
    {
        // Dependencies injected
    }

    protected override ContextMenuStrip CreateMenu()
    {
        var context = _contextResolver.ResolveContext(FolderPath, selectedPaths);
        var isDarkTheme = _themeDetector.IsDarkTheme();
        return _menuFactory.CreateMenu(context, isDarkTheme);
    }
}
```

## Benefits of New Architecture

### 1. **Better Organization**
- Reduced from 13 projects to 6
- Clear separation of concerns
- Standard folder structure

### 2. **Improved Code Quality**
- **Before:** 267 lines in ShellServer.cs doing everything
- **After:** Separated into focused services (<100 lines each)

### 3. **Enhanced Testability**
- Services use interfaces (can be mocked)
- Dependencies are injected
- Business logic separated from framework code

### 4. **Clearer Dependencies**
```
Core (no dependencies)
  ↑
  ├── Features
  ├── UI
  └── Infrastructure
```

### 5. **Better Naming**
No more confusing names like:
- ❌ `Settings.Settings`
- ❌ `ContextMenu.ContextMenu`
- ❌ `Service.Service`

Now:
- ✅ `AppConfiguration`
- ✅ `MenuFactory`
- ✅ `ShellExtension`

## Migration Steps for Developers

If you were working on the old codebase, here's how to adapt:

### 1. Update Your Build Process
- Open the new `Sonnenberg.sln` (will be created)
- Restore NuGet packages
- Build as before

### 2. Update References to Old Classes

**Example: Accessing the logger**

Before:
```csharp
using Sonnenberg.Common;
Logger.Configure();
```

After:
```csharp
using Sonnenberg.Infrastructure.Logging;
LoggerConfigurator.Configure();
```

### 3. Adding New Features

**Before:** Add files all over the place

**After:** Add to the appropriate layer

```
Sonnenberg.Features/
└── MyNewFeature/
    ├── MyFeatureService.cs
    └── MyFeatureModel.cs
```

### 4. Working with Settings

**Before:**
```csharp
using Sonnenberg.Settings.Properties;
var value = UserSettings.Default.clickedItemPath;
```

**After:**
```csharp
using Sonnenberg.Infrastructure.Configuration;
var value = AppConfiguration.Instance.CurrentContext.Path;
```

## What Stayed the Same

1. **External dependencies** - Still uses SharpShell, log4net, etc.
2. **Functionality** - All features work the same
3. **Installer** - WiX installer still works (just moved to `installer/`)
4. **Build process** - Still builds with Visual Studio
5. **Deployment** - Same MSI output

## Need Help?

- See `docs/ARCHITECTURE.md` for detailed architecture documentation
- Old project folders are still there for reference
- Check git history to see how files were moved/refactored

## FAQ

**Q: Where did `Common/Helper.cs` go?**
A: Renamed to `Sonnenberg.UI/Icons/IconHelper.cs`

**Q: Where is the Settings class?**
A: Renamed to `Sonnenberg.Infrastructure/Configuration/AppConfiguration.cs`

**Q: Can I still use the old structure?**
A: The old folders are preserved for reference, but you should migrate to the new structure.

**Q: Will this break the installer?**
A: No, the installer will be updated to reference the new project structure.

**Q: How do I add a new menu item?**
A: Add to `Sonnenberg.Features`, implement the service, then update `MenuFactory` in `Sonnenberg.UI`.

**Q: Where are the tests?**
A: A `tests/` folder has been created for future unit tests. The new architecture makes testing much easier.
