#pragma once

#include <windows.h>
#include <shobjidl_core.h>

enum class Action {
    Menu, CopyPath, CopyForwardPath, CommandPrompt, CommandPromptAdmin,
    PowerShell, PowerShellAdmin, CountLines, CountNonEmptyLines, OpenTargetFolder
};

HRESULT GetContextActionState(Action action, IShellItemArray* items, bool allowSlow, EXPCMDSTATE& state) noexcept;
HRESULT InvokeContextAction(Action action, IShellItemArray* items, const wchar_t* modulePath);
