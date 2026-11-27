#include <windows.h>

#include <shobjidl_core.h>
#include <shlguid.h>
#include <wrl/client.h>

#include "ContextActions.h"
#include "Clipboard.h"

#include <algorithm>
#include <atomic>
#include <cwchar>
#include <memory>
#include <new>
#include <string>

namespace
{
using Microsoft::WRL::ComPtr;

// Keep this identifier in sync with packaging/AppxManifest.template.xml.
constexpr CLSID kSonnenbergCommandClsid = {
    0xb79f8fcc,
    0xb264,
    0x4d0b,
    {0xb0, 0xc9, 0xeb, 0xda, 0xac, 0xeb, 0xcf, 0xd6}};

constexpr CLSID kCopyPathCommandClsid = {
    0x26b60742, 0x71ef, 0x4a3d, {0xaa, 0x7f, 0xa3, 0x4f, 0x36, 0x92, 0xb4, 0x2b}};
constexpr CLSID kCopyForwardPathCommandClsid = {
    0xf7f1f8dd, 0x3c59, 0x4041, {0x87, 0x53, 0x35, 0x62, 0xa1, 0x8a, 0x56, 0xf6}};

constexpr Action kSubCommands[] = {
    Action::CopyPath, Action::CopyForwardPath, Action::CommandPrompt, Action::CommandPromptAdmin,
    Action::PowerShell, Action::PowerShellAdmin, Action::CountLines, Action::CountNonEmptyLines,
    Action::OpenTargetFolder
};
constexpr ULONG kSubCommandCount = ARRAYSIZE(kSubCommands);
HMODULE g_module = nullptr;

std::atomic<long> g_objectCount = 0;
std::atomic<long> g_serverLocks = 0;

HRESULT CopyString(const wchar_t* source, wchar_t** destination) noexcept
{
    if (destination == nullptr)
    {
        return E_POINTER;
    }

    *destination = nullptr;
    const size_t characterCount = std::wcslen(source) + 1;
    auto* result = static_cast<wchar_t*>(
        CoTaskMemAlloc(characterCount * sizeof(wchar_t)));
    if (result == nullptr)
    {
        return E_OUTOFMEMORY;
    }

    std::wmemcpy(result, source, characterCount);
    *destination = result;
    return S_OK;
}

class ExplorerCommand final : public IExplorerCommand, public IObjectWithSite
{
public:
    explicit ExplorerCommand(Action action = Action::Menu, IUnknown* site = nullptr) noexcept :
        action_(action), site_(site)
    {
        ++g_objectCount;
    }

    ExplorerCommand(const ExplorerCommand&) = delete;
    ExplorerCommand& operator=(const ExplorerCommand&) = delete;

    IFACEMETHODIMP QueryInterface(REFIID interfaceId, void** object) noexcept override
    {
        if (object == nullptr)
        {
            return E_POINTER;
        }

        *object = nullptr;
        if (IsEqualIID(interfaceId, IID_IUnknown) ||
            IsEqualIID(interfaceId, __uuidof(IExplorerCommand)))
        {
            *object = static_cast<IExplorerCommand*>(this);
        }
        else if (IsEqualIID(interfaceId, __uuidof(IObjectWithSite)))
        {
            *object = static_cast<IObjectWithSite*>(this);
        }
        else
        {
            return E_NOINTERFACE;
        }
        AddRef();
        return S_OK;
    }

    IFACEMETHODIMP_(ULONG) AddRef() noexcept override
    {
        return static_cast<ULONG>(++referenceCount_);
    }

    IFACEMETHODIMP_(ULONG) Release() noexcept override
    {
        const ULONG remaining = static_cast<ULONG>(--referenceCount_);
        if (remaining == 0)
        {
            delete this;
        }
        return remaining;
    }

    IFACEMETHODIMP GetTitle(IShellItemArray*, wchar_t** title) noexcept override
    {
        const bool german = PRIMARYLANGID(GetUserDefaultUILanguage()) == LANG_GERMAN;
        switch (action_)
        {
        case Action::Menu:
            return CopyString(L"Sonnenberg", title);
        case Action::CopyPath:
            return CopyString(german ? L"Pfad kopieren" : L"Copy path", title);
        case Action::CopyForwardPath:
            return CopyString(
                german ? L"Pfad mit Schrägstrichen kopieren" : L"Copy path with forward slashes", title);
        case Action::CommandPrompt:
            return CopyString(german ? L"Eingabeaufforderung hier öffnen" : L"Open Command Prompt here", title);
        case Action::CommandPromptAdmin:
            return CopyString(german ? L"Eingabeaufforderung hier als Administrator öffnen" :
                L"Open Command Prompt here as administrator", title);
        case Action::PowerShell:
            return CopyString(german ? L"PowerShell hier öffnen" : L"Open PowerShell here", title);
        case Action::PowerShellAdmin:
            return CopyString(german ? L"PowerShell hier als Administrator öffnen" :
                L"Open PowerShell here as administrator", title);
        case Action::CountLines:
            return CopyString(german ? L"Zeilen zählen" : L"Count lines", title);
        case Action::CountNonEmptyLines:
            return CopyString(german ? L"Zeilen ohne Leerzeilen zählen" : L"Count non-empty lines", title);
        case Action::OpenTargetFolder:
            return CopyString(german ? L"Zielordner öffnen" : L"Open shortcut target folder", title);
        }
        return E_UNEXPECTED;
    }

    IFACEMETHODIMP SetSite(IUnknown* site) noexcept override
    {
        site_ = site;
        return S_OK;
    }

    IFACEMETHODIMP GetSite(REFIID interfaceId, void** site) noexcept override
    {
        if (site == nullptr)
        {
            return E_POINTER;
        }
        *site = nullptr;
        return site_ ? site_->QueryInterface(interfaceId, site) : E_FAIL;
    }

    IFACEMETHODIMP GetIcon(IShellItemArray*, wchar_t** icon) noexcept override
    {
        if (icon == nullptr)
        {
            return E_POINTER;
        }
        *icon = nullptr;
        wchar_t modulePath[MAX_PATH]{};
        const DWORD length = GetModuleFileNameW(g_module, modulePath, ARRAYSIZE(modulePath));
        if (length == 0 || length >= ARRAYSIZE(modulePath))
        {
            return E_FAIL;
        }
        wchar_t* separator = std::wcsrchr(modulePath, L'\\');
        if (separator == nullptr)
        {
            return E_FAIL;
        }
        wchar_t* fileName = separator + 1;
        constexpr const wchar_t* icons[] = {
            L"Assets\\SonnenbergMenu.ico", L"Assets\\CopyPath.ico", L"Assets\\CopyForwardPath.ico",
            L"Assets\\CommandPrompt.ico", L"Assets\\CommandPromptAdmin.ico",
            L"Assets\\PowerShell.ico", L"Assets\\PowerShellAdmin.ico",
            L"Assets\\CountLines.ico", L"Assets\\CountNonEmptyLines.ico",
            L"Assets\\OpenTargetFolder.ico"
        };
        const wchar_t* relativeIcon = icons[static_cast<unsigned int>(action_)];
        const size_t remaining = ARRAYSIZE(modulePath) - static_cast<size_t>(fileName - modulePath);
        return wcscpy_s(fileName, remaining, relativeIcon) == 0 ? CopyString(modulePath, icon) : E_FAIL;
    }

    IFACEMETHODIMP GetToolTip(IShellItemArray*, wchar_t** toolTip) noexcept override
    {
        return GetTitle(nullptr, toolTip);
    }

    IFACEMETHODIMP GetCanonicalName(GUID* commandName) noexcept override
    {
        if (commandName == nullptr)
        {
            return E_POINTER;
        }
        *commandName = action_ == Action::CopyPath ? kCopyPathCommandClsid :
            action_ == Action::CopyForwardPath ? kCopyForwardPathCommandClsid : kSonnenbergCommandClsid;
        if (action_ != Action::Menu && action_ != Action::CopyPath && action_ != Action::CopyForwardPath)
        {
            commandName->Data1 += static_cast<unsigned long>(action_);
        }
        return S_OK;
    }

    IFACEMETHODIMP GetState(
        IShellItemArray* selection,
        BOOL allowSlow,
        EXPCMDSTATE* state) noexcept override
    {
        if (state == nullptr)
        {
            return E_POINTER;
        }
        DWORD count = 0;
        const bool hasSelection = selection != nullptr && SUCCEEDED(selection->GetCount(&count)) && count != 0;
        if (action_ == Action::Menu || action_ == Action::CopyPath || action_ == Action::CopyForwardPath)
        {
            *state = hasSelection || site_ ? ECS_ENABLED : ECS_DISABLED;
            return S_OK;
        }

        ComPtr<IShellItemArray> currentFolder;
        if (!hasSelection && SUCCEEDED(GetCurrentFolder(currentFolder.GetAddressOf())))
        {
            selection = currentFolder.Get();
        }
        return GetContextActionState(action_, selection, allowSlow != FALSE, *state);
    }

    IFACEMETHODIMP Invoke(IShellItemArray* selection, IBindCtx*) noexcept override
    try
    {
        if (action_ == Action::Menu)
        {
            return E_NOTIMPL;
        }
        DWORD itemCount = 0;
        HRESULT result = selection ? selection->GetCount(&itemCount) : S_OK;
        if (FAILED(result))
        {
            return result;
        }

        ComPtr<IShellItemArray> currentFolder;
        if (itemCount == 0)
        {
            result = GetCurrentFolder(currentFolder.GetAddressOf());
            if (FAILED(result))
            {
                return result;
            }
            selection = currentFolder.Get();
            result = selection->GetCount(&itemCount);
            if (FAILED(result))
            {
                return result;
            }
        }

        if (action_ != Action::CopyPath && action_ != Action::CopyForwardPath)
        {
            wchar_t modulePath[MAX_PATH]{};
            const DWORD length = GetModuleFileNameW(g_module, modulePath, ARRAYSIZE(modulePath));
            return length != 0 && length < ARRAYSIZE(modulePath) ?
                InvokeContextAction(action_, selection, modulePath) : E_FAIL;
        }

        std::wstring paths;
        for (DWORD index = 0; index < itemCount; ++index)
        {
            IShellItem* item = nullptr;
            result = selection->GetItemAt(index, &item);
            if (FAILED(result))
            {
                return result;
            }

            wchar_t* path = nullptr;
            result = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
            item->Release();
            const std::unique_ptr<wchar_t, decltype(&CoTaskMemFree)> ownedPath(path, CoTaskMemFree);
            if (FAILED(result))
            {
                return result;
            }

            if (!paths.empty())
            {
                paths.append(L"\r\n");
            }
            paths.append(path);
        }

        if (paths.empty())
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
        }

        if (action_ == Action::CopyForwardPath)
        {
            std::replace(paths.begin(), paths.end(), L'\\', L'/');
        }
        return CopyTextToClipboard(paths);
    }
    catch (const std::bad_alloc&)
    {
        return E_OUTOFMEMORY;
    }
    catch (...)
    {
        return E_FAIL;
    }

    IFACEMETHODIMP GetFlags(EXPCMDFLAGS* flags) noexcept override
    {
        if (flags == nullptr)
        {
            return E_POINTER;
        }
        *flags = action_ == Action::Menu ? ECF_HASSUBCOMMANDS : ECF_DEFAULT;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** commands) noexcept override;

private:
    HRESULT GetCurrentFolder(IShellItemArray** folder) noexcept
    {
        if (!site_)
        {
            return E_INVALIDARG;
        }

        ComPtr<IServiceProvider> services;
        HRESULT result = site_.As(&services);
        if (FAILED(result))
        {
            return result;
        }

        ComPtr<IFolderView> view;
        result = services->QueryService(__uuidof(IFolderView), IID_PPV_ARGS(view.GetAddressOf()));
        if (FAILED(result))
        {
            ComPtr<IShellBrowser> browser;
            result = services->QueryService(SID_STopLevelBrowser, IID_PPV_ARGS(browser.GetAddressOf()));
            if (FAILED(result))
            {
                return result;
            }
            ComPtr<IShellView> activeView;
            result = browser->QueryActiveShellView(activeView.GetAddressOf());
            if (FAILED(result))
            {
                return result;
            }
            result = activeView.As(&view);
            if (FAILED(result))
            {
                return result;
            }
        }
        return view->GetFolder(IID_PPV_ARGS(folder));
    }

    ~ExplorerCommand() noexcept
    {
        --g_objectCount;
    }

    std::atomic<ULONG> referenceCount_ = 1;
    const Action action_;
    ComPtr<IUnknown> site_;
};

class SubCommandEnumerator final : public IEnumExplorerCommand
{
public:
    explicit SubCommandEnumerator(IUnknown* site, ULONG position = 0) noexcept :
        position_(position), site_(site)
    {
        ++g_objectCount;
    }

    IFACEMETHODIMP QueryInterface(REFIID interfaceId, void** object) noexcept override
    {
        if (object == nullptr)
        {
            return E_POINTER;
        }
        *object = nullptr;
        if (interfaceId != IID_IUnknown && interfaceId != __uuidof(IEnumExplorerCommand))
        {
            return E_NOINTERFACE;
        }
        *object = static_cast<IEnumExplorerCommand*>(this);
        AddRef();
        return S_OK;
    }

    IFACEMETHODIMP_(ULONG) AddRef() noexcept override { return ++references_; }

    IFACEMETHODIMP_(ULONG) Release() noexcept override
    {
        const ULONG remaining = --references_;
        if (remaining == 0)
        {
            delete this;
        }
        return remaining;
    }

    IFACEMETHODIMP Next(ULONG count, IExplorerCommand** commands, ULONG* fetched) noexcept override
    {
        if (commands == nullptr || (fetched == nullptr && count != 1))
        {
            return E_POINTER;
        }
        ULONG supplied = 0;
        if (fetched != nullptr)
        {
            *fetched = 0;
        }
        while (supplied < count && position_ + supplied < kSubCommandCount)
        {
            auto* command = new (std::nothrow) ExplorerCommand(kSubCommands[position_ + supplied], site_.Get());
            if (command == nullptr)
            {
                while (supplied > 0)
                {
                    commands[--supplied]->Release();
                    commands[supplied] = nullptr;
                }
                return E_OUTOFMEMORY;
            }
            commands[supplied++] = command;
        }
        position_ += supplied;
        if (fetched != nullptr)
        {
            *fetched = supplied;
        }
        return supplied == count ? S_OK : S_FALSE;
    }

    IFACEMETHODIMP Skip(ULONG count) noexcept override
    {
        const ULONG skipped = std::min(count, kSubCommandCount - position_);
        position_ += skipped;
        return skipped == count ? S_OK : S_FALSE;
    }

    IFACEMETHODIMP Reset() noexcept override
    {
        position_ = 0;
        return S_OK;
    }

    IFACEMETHODIMP Clone(IEnumExplorerCommand** commands) noexcept override
    {
        if (commands == nullptr)
        {
            return E_POINTER;
        }
        *commands = new (std::nothrow) SubCommandEnumerator(site_.Get(), position_);
        return *commands != nullptr ? S_OK : E_OUTOFMEMORY;
    }

private:
    ~SubCommandEnumerator() noexcept { --g_objectCount; }

    std::atomic<ULONG> references_ = 1;
    ULONG position_;
    ComPtr<IUnknown> site_;
};

HRESULT ExplorerCommand::EnumSubCommands(IEnumExplorerCommand** commands) noexcept
{
    if (commands == nullptr)
    {
        return E_POINTER;
    }
    *commands = nullptr;
    if (action_ != Action::Menu)
    {
        return E_NOTIMPL;
    }
    *commands = new (std::nothrow) SubCommandEnumerator(site_.Get());
    return *commands != nullptr ? S_OK : E_OUTOFMEMORY;
}

class ExplorerCommandClassFactory final : public IClassFactory
{
public:
    ExplorerCommandClassFactory() noexcept
    {
        ++g_objectCount;
    }

    ExplorerCommandClassFactory(const ExplorerCommandClassFactory&) = delete;
    ExplorerCommandClassFactory& operator=(const ExplorerCommandClassFactory&) = delete;

    IFACEMETHODIMP QueryInterface(REFIID interfaceId, void** object) noexcept override
    {
        if (object == nullptr)
        {
            return E_POINTER;
        }

        *object = nullptr;
        if (IsEqualIID(interfaceId, IID_IUnknown) ||
            IsEqualIID(interfaceId, IID_IClassFactory))
        {
            *object = static_cast<IClassFactory*>(this);
            AddRef();
            return S_OK;
        }
        return E_NOINTERFACE;
    }

    IFACEMETHODIMP_(ULONG) AddRef() noexcept override
    {
        return static_cast<ULONG>(++referenceCount_);
    }

    IFACEMETHODIMP_(ULONG) Release() noexcept override
    {
        const ULONG remaining = static_cast<ULONG>(--referenceCount_);
        if (remaining == 0)
        {
            delete this;
        }
        return remaining;
    }

    IFACEMETHODIMP CreateInstance(
        IUnknown* outer,
        REFIID interfaceId,
        void** object) noexcept override
    {
        if (object == nullptr)
        {
            return E_POINTER;
        }
        *object = nullptr;

        if (outer != nullptr)
        {
            return CLASS_E_NOAGGREGATION;
        }

        auto* command = new (std::nothrow) ExplorerCommand();
        if (command == nullptr)
        {
            return E_OUTOFMEMORY;
        }

        const HRESULT result = command->QueryInterface(interfaceId, object);
        command->Release();
        return result;
    }

    IFACEMETHODIMP LockServer(BOOL lock) noexcept override
    {
        if (lock)
        {
            ++g_serverLocks;
        }
        else
        {
            --g_serverLocks;
        }
        return S_OK;
    }

private:
    ~ExplorerCommandClassFactory() noexcept
    {
        --g_objectCount;
    }

    std::atomic<ULONG> referenceCount_ = 1;
};
} // namespace

extern "C" HRESULT __stdcall DllCanUnloadNow()
{
    return g_objectCount.load() == 0 && g_serverLocks.load() == 0 ? S_OK : S_FALSE;
}

extern "C" HRESULT __stdcall DllGetClassObject(
    REFCLSID classId,
    REFIID interfaceId,
    void** object)
{
    if (object == nullptr)
    {
        return E_POINTER;
    }
    *object = nullptr;

    if (!IsEqualCLSID(classId, kSonnenbergCommandClsid))
    {
        return CLASS_E_CLASSNOTAVAILABLE;
    }

    auto* factory = new (std::nothrow) ExplorerCommandClassFactory();
    if (factory == nullptr)
    {
        return E_OUTOFMEMORY;
    }

    const HRESULT result = factory->QueryInterface(interfaceId, object);
    factory->Release();
    return result;
}

BOOL WINAPI DllMain(HINSTANCE instance, DWORD reason, void*) noexcept
{
    if (reason == DLL_PROCESS_ATTACH)
    {
        g_module = instance;
        DisableThreadLibraryCalls(instance);
    }
    return TRUE;
}
