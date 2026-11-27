#include "ContextActions.h"

#include <shellapi.h>
#include <shlguid.h>
#include <shlobj.h>
#include <shlwapi.h>
#include <wrl/client.h>

#include <cwchar>
#include <filesystem>
#include <memory>
#include <string>

namespace
{
using Microsoft::WRL::ComPtr;
using TaskString = std::unique_ptr<wchar_t, decltype(&CoTaskMemFree)>;

bool IsCountAction(Action action) noexcept
{
    return action == Action::CountLines || action == Action::CountNonEmptyLines;
}

bool IsTextPath(const wchar_t* path) noexcept
{
    const wchar_t* extension = PathFindExtensionW(path);
    PERCEIVED perceivedType = PERCEIVED_TYPE_UNSPECIFIED;
    PERCEIVEDFLAG flags = 0;
    if (SUCCEEDED(AssocGetPerceivedType(extension, &perceivedType, &flags, nullptr)) &&
        perceivedType == PERCEIVED_TYPE_TEXT)
    {
        return true;
    }

    constexpr const wchar_t* codeAndData[] = {
#include "LineCountingExtensions.inc"
    };
    for (const wchar_t* candidate : codeAndData)
    {
        if (_wcsicmp(extension, candidate) == 0)
        {
            return true;
        }
    }
    return false;
}

HRESULT ResolveShortcut(const wchar_t* file, wchar_t (&target)[MAX_PATH], WIN32_FIND_DATAW& data) noexcept
{
    ComPtr<IShellLinkW> link;
    HRESULT result = CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER,
        IID_PPV_ARGS(link.GetAddressOf()));
    if (FAILED(result))
    {
        return result;
    }
    ComPtr<IPersistFile> persist;
    result = link.As(&persist);
    if (SUCCEEDED(result))
    {
        result = persist->Load(file, STGM_READ | STGM_SHARE_DENY_NONE);
    }
    if (FAILED(result))
    {
        return result;
    }
    result = link->GetPath(target, ARRAYSIZE(target), &data, 0);
    return result == S_OK && *target != L'\0' ? S_OK : HRESULT_FROM_WIN32(ERROR_PATH_NOT_FOUND);
}

bool IsShortcut(const wchar_t* path) noexcept
{
    return _wcsicmp(PathFindExtensionW(path), L".lnk") == 0;
}

HRESULT Launch(const wchar_t* executable, const wchar_t* directory, const wchar_t* args, bool admin) noexcept
{
    SHELLEXECUTEINFOW request{};
    request.cbSize = sizeof(request);
    request.fMask = SEE_MASK_NOASYNC | SEE_MASK_FLAG_NO_UI;
    request.lpVerb = admin ? L"runas" : L"open";
    request.lpFile = executable;
    request.lpParameters = args;
    request.lpDirectory = directory;
    request.nShow = SW_SHOWNORMAL;
    return ShellExecuteExW(&request) ? S_OK : HRESULT_FROM_WIN32(GetLastError());
}
}

HRESULT GetContextActionState(Action action, IShellItemArray* items, bool allowSlow, EXPCMDSTATE& state) noexcept
{
    state = ECS_HIDDEN;
    DWORD count = 0;
    ComPtr<IShellItem> item;
    SFGAOF attributes = 0;
    if (!items || FAILED(items->GetCount(&count)) || count != 1 ||
        FAILED(items->GetItemAt(0, item.GetAddressOf())) ||
        FAILED(item->GetAttributes(SFGAO_FILESYSTEM | SFGAO_FOLDER, &attributes)) ||
        (attributes & SFGAO_FILESYSTEM) == 0)
    {
        return S_OK;
    }

    wchar_t* rawPath = nullptr;
    const HRESULT result = item->GetDisplayName(SIGDN_FILESYSPATH, &rawPath);
    const TaskString path(rawPath, CoTaskMemFree);
    if (FAILED(result) || !path)
    {
        return S_OK;
    }
    const bool shortcut = IsShortcut(path.get());
    if (action == Action::OpenTargetFolder)
    {
        state = shortcut ? ECS_ENABLED : ECS_HIDDEN;
        return S_OK;
    }
    if (IsCountAction(action))
    {
        state = !shortcut && (attributes & SFGAO_FOLDER) == 0 && IsTextPath(path.get()) ?
            ECS_ENABLED : ECS_HIDDEN;
        return S_OK;
    }
    if (shortcut && (attributes & SFGAO_FOLDER) == 0)
    {
        if (!allowSlow)
        {
            return E_PENDING;
        }
        wchar_t target[MAX_PATH]{};
        WIN32_FIND_DATAW data{};
        state = SUCCEEDED(ResolveShortcut(path.get(), target, data)) &&
            (data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0 ? ECS_ENABLED : ECS_HIDDEN;
        return S_OK;
    }
    state = (attributes & SFGAO_FOLDER) != 0 ? ECS_ENABLED : ECS_HIDDEN;
    return S_OK;
}

HRESULT InvokeContextAction(Action action, IShellItemArray* items, const wchar_t* modulePath)
{
    DWORD count = 0;
    ComPtr<IShellItem> item;
    if (!items || FAILED(items->GetCount(&count)) || count != 1 ||
        FAILED(items->GetItemAt(0, item.GetAddressOf())))
    {
        return E_INVALIDARG;
    }
    wchar_t* rawPath = nullptr;
    const HRESULT result = item->GetDisplayName(SIGDN_FILESYSPATH, &rawPath);
    const TaskString path(rawPath, CoTaskMemFree);
    if (FAILED(result) || !path)
    {
        return FAILED(result) ? result : E_INVALIDARG;
    }

    if (IsCountAction(action))
    {
        EXPCMDSTATE state = ECS_HIDDEN;
        if (FAILED(GetContextActionState(action, items, false, state)) || state != ECS_ENABLED)
        {
            return E_INVALIDARG;
        }
        const auto executable = std::filesystem::path(modulePath).parent_path() / L"Sonnenberg.exe";
        std::wstring args = action == Action::CountLines ? L"--count-lines \"" : L"--count-nonempty-lines \"";
        args.append(path.get()).append(L"\"");
        return Launch(executable.c_str(), nullptr, args.c_str(), false);
    }

    std::filesystem::path directory(path.get());
    if (IsShortcut(path.get()))
    {
        wchar_t target[MAX_PATH]{};
        WIN32_FIND_DATAW data{};
        const HRESULT resolved = ResolveShortcut(path.get(), target, data);
        if (FAILED(resolved))
        {
            return resolved;
        }
        directory = target;
        const DWORD attributes = GetFileAttributesW(directory.c_str());
        if (attributes == INVALID_FILE_ATTRIBUTES)
        {
            return HRESULT_FROM_WIN32(GetLastError());
        }
        if ((attributes & FILE_ATTRIBUTE_DIRECTORY) == 0 && action == Action::OpenTargetFolder)
        {
            directory = directory.parent_path();
        }
    }

    const DWORD attributes = GetFileAttributesW(directory.c_str());
    if (attributes == INVALID_FILE_ATTRIBUTES || (attributes & FILE_ATTRIBUTE_DIRECTORY) == 0)
    {
        return HRESULT_FROM_WIN32(ERROR_DIRECTORY);
    }
    if (action == Action::OpenTargetFolder)
    {
        PIDLIST_ABSOLUTE pidl = nullptr;
        const HRESULT parsed = SHParseDisplayName(directory.c_str(), nullptr, &pidl, 0, nullptr);
        if (FAILED(parsed))
        {
            return parsed;
        }
        SHELLEXECUTEINFOW request{};
        request.cbSize = sizeof(request);
        request.fMask = SEE_MASK_IDLIST | SEE_MASK_FLAG_NO_UI | SEE_MASK_NOASYNC;
        request.lpVerb = L"explore";
        request.lpIDList = pidl;
        request.nShow = SW_SHOWNORMAL;
        const HRESULT opened = ShellExecuteExW(&request) ? S_OK : HRESULT_FROM_WIN32(GetLastError());
        CoTaskMemFree(pidl);
        return opened;
    }

    wchar_t systemDirectory[MAX_PATH]{};
    const UINT length = GetSystemDirectoryW(systemDirectory, ARRAYSIZE(systemDirectory));
    if (length == 0 || length >= ARRAYSIZE(systemDirectory))
    {
        return E_FAIL;
    }
    const bool commandPrompt = action == Action::CommandPrompt || action == Action::CommandPromptAdmin;
    const bool admin = action == Action::CommandPromptAdmin || action == Action::PowerShellAdmin;
    const auto executable = std::filesystem::path(systemDirectory) /
        (commandPrompt ? L"cmd.exe" : L"WindowsPowerShell\\v1.0\\powershell.exe");
    return Launch(executable.c_str(), directory.c_str(), commandPrompt ? L"/D" : L"-NoLogo -NoProfile", admin);
}
