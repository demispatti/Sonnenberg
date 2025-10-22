# Sonnenberg Architecture

This document describes the refactored architecture of the Sonnenberg Windows Explorer extension.

## Overview

The project has been refactored from a custom structure with 13 separate projects into a clean, industry-standard architecture with 6 well-organized projects following **Clean Architecture** principles.

## Architecture Principles

1. **Separation of Concerns** - Each layer has a specific responsibility
2. **Dependency Inversion** - Core depends on abstractions, not implementations
3. **Single Responsibility** - Each class has one reason to change
4. **Testability** - Code is designed to be easily testable
5. **Maintainability** - Clear structure makes the codebase easy to understand and modify

## Project Structure

```
Sonnenberg/
├── src/
│   ├── Sonnenberg.Core/              # Domain & Application Core
│   ├── Sonnenberg.Features/          # Feature implementations
│   ├── Sonnenberg.UI/                # Presentation layer
│   ├── Sonnenberg.Infrastructure/    # Cross-cutting concerns
│   ├── Sonnenberg.Localization/      # i18n resources
│   └── Sonnenberg.Management/        # Service management utilities
├── installer/                         # WiX installer project
└── docs/                              # Documentation
```

## Layer Details

### 1. Sonnenberg.Core

**Purpose:** Contains the domain models, core interfaces, and application entry point.

**Key Components:**
- `ShellExtension.cs` - Main entry point (formerly ShellServer.cs)
- `Models/` - Domain models (ItemContext, ItemType, ShortcutInfo)
- `Interfaces/` - Abstractions for services
- `Services/` - Core business logic (ItemContextResolver)

**Dependencies:** None (pure C#)

**Responsibilities:**
- Define domain models
- Define service contracts
- Resolve item context (determine what was clicked in Explorer)
- Integrate with Windows Shell via SharpShell

### 2. Sonnenberg.Features

**Purpose:** Implementation of specific features.

**Structure:**
```
Sonnenberg.Features/
├── CopyPath/
│   └── PathCopyService.cs
├── LineCounter/
│   ├── LineCounterService.cs
│   └── LineCountResult.cs
└── ShellLauncher/
    ├── ShellLauncherService.cs
    └── ShellType.cs
```

**Dependencies:**
- Sonnenberg.Core (interfaces)
- log4net

**Features:**
- **CopyPath** - Copies file/folder paths to clipboard (with/without forward slashes)
- **LineCounter** - Counts lines in text files (with/without blank lines)
- **ShellLauncher** - Launches CMD/PowerShell in specific directories

### 3. Sonnenberg.UI

**Purpose:** User interface components (menus, icons, theming).

**Structure:**
```
Sonnenberg.UI/
├── Menus/
│   ├── MenuFactory.cs
│   └── IMenuItemProvider.cs
├── Icons/
│   └── IconHelper.cs
└── Theming/
```

**Dependencies:**
- Sonnenberg.Core (interfaces, models)
- Sonnenberg.Features (services)
- System.Windows.Forms

**Responsibilities:**
- Create context menus based on item type
- Manage menu items and icons
- Handle dark/light theme

### 4. Sonnenberg.Infrastructure

**Purpose:** Cross-cutting concerns and technical infrastructure.

**Structure:**
```
Sonnenberg.Infrastructure/
├── Logging/
│   └── LoggerConfigurator.cs
├── Configuration/
│   └── AppConfiguration.cs
├── EventHandling/
│   └── ContextMenuStateManager.cs
└── Theming/
    └── WindowsThemeDetector.cs
```

**Dependencies:**
- Sonnenberg.Core (interfaces)
- log4net
- Gma.System.MouseKeyHook

**Responsibilities:**
- Configure logging (log4net)
- Manage application configuration
- Detect Windows theme (dark/light mode)
- Track context menu state (reset when menu closes)

### 5. Sonnenberg.Localization

**Purpose:** Internationalization and localization resources.

**Contents:**
- Resource files for strings (English, German, etc.)
- Localized message templates

**Dependencies:** None

### 6. Sonnenberg.Management

**Purpose:** Service management and registration utilities.

**Structure:**
```
Sonnenberg.Management/
├── ServiceControl/
│   └── (Windows Service management)
└── Registration/
    └── (Shell extension registration)
```

**Dependencies:**
- System.ServiceProcess
- SharpShell

**Responsibilities:**
- Start/stop Windows service
- Register/unregister shell extension
- Installation utilities

## Migration from Old Structure

### Before (13 Projects)
```
- ShellServer
- ContextMenu
- Common
- Settings
- MenuWatcher
- Language
- Service
- ServiceManager
- ServerManager
- StartService
- StopService
- StartServer
- StopServer
```

### After (6 Projects)
```
- Sonnenberg.Core           (ShellServer + models)
- Sonnenberg.Features       (business logic extracted)
- Sonnenberg.UI             (ContextMenu refactored)
- Sonnenberg.Infrastructure (Common, Settings, MenuWatcher)
- Sonnenberg.Localization   (Language)
- Sonnenberg.Management     (Service, managers, start/stop)
```

## Key Improvements

### 1. Better Naming
- ❌ `ShellServer` → ✅ `ShellExtension`
- ❌ `Common` → ✅ `Infrastructure`
- ❌ `MenuWatcher` → ✅ `ContextMenuStateManager`

### 2. Clear Responsibilities
- Each project has a single, well-defined purpose
- No more duplicate names (Settings.Settings, ContextMenu.ContextMenu)
- Business logic separated from UI

### 3. Dependency Flow
```
┌─────────────────────────────────────────┐
│         Sonnenberg.Core                 │
│    (Interfaces & Domain Models)         │
└─────────────────────────────────────────┘
           ▲              ▲              ▲
           │              │              │
    ┌──────┴─────┐  ┌────┴─────┐  ┌─────┴──────┐
    │  Features  │  │    UI    │  │Infrastructure│
    └────────────┘  └──────────┘  └─────────────┘
```

### 4. Testability
- Services use dependency injection
- Clear interfaces for mocking
- Business logic separated from framework code

### 5. Maintainability
- Standard folder structure
- Clear naming conventions
- Comprehensive documentation

## Building the Project

The project still uses Visual Studio and builds the same way as before:

1. Open `Sonnenberg.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (F6)
4. Build the installer project for deployment

## Testing

The refactored architecture makes it easier to write unit tests:

```csharp
// Example: Testing the LineCounterService
var service = new LineCounterService();
var result = service.CountLines("test.txt", excludeBlankLines: true);
Assert.AreEqual(42, result.LineCount);
```

## Future Enhancements

With this new architecture, it's now easier to:

1. Add new features (just create a new folder in Features)
2. Add unit tests (services are injectable)
3. Support more shells (Git Bash, WSL, etc.)
4. Add more file operations
5. Implement a settings UI

## References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [SharpShell Documentation](https://github.com/dwmkerr/sharpshell)
