#include <windows.h>

#include <shobjidl_core.h>

#include <algorithm>
#include <atomic>
#include <cwchar>
#include <memory>
#include <new>
#include <string>

namespace
{
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

enum class Action { Menu, CopyPath, CopyForwardPath };
constexpr Action kSubCommands[] = {Action::CopyPath, Action::CopyForwardPath};
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

HRESULT CopyTextToClipboard(const std::wstring& text) noexcept
{
    const HWND owner = CreateWindowExW(
        0, L"STATIC", nullptr, 0, 0, 0, 0, 0, HWND_MESSAGE, nullptr, nullptr, nullptr);
    if (owner == nullptr)
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    if (!OpenClipboard(owner))
    {
        const HRESULT result = HRESULT_FROM_WIN32(GetLastError());
        DestroyWindow(owner);
        return result;
    }

    HRESULT result = S_OK;
    HGLOBAL memory = nullptr;

    if (!EmptyClipboard())
    {
        result = HRESULT_FROM_WIN32(GetLastError());
    }
    else
    {
        const size_t byteCount = (text.size() + 1) * sizeof(wchar_t);
        memory = GlobalAlloc(GMEM_MOVEABLE, byteCount);
        if (memory == nullptr)
        {
            result = E_OUTOFMEMORY;
        }
        else
        {
            auto* buffer = static_cast<wchar_t*>(GlobalLock(memory));
            if (buffer == nullptr)
            {
                result = HRESULT_FROM_WIN32(GetLastError());
            }
            else
            {
                std::wmemcpy(buffer, text.c_str(), text.size() + 1);
                GlobalUnlock(memory);

                if (SetClipboardData(CF_UNICODETEXT, memory) == nullptr)
                {
                    result = HRESULT_FROM_WIN32(GetLastError());
                }
                else
                {
                    memory = nullptr; // Ownership transferred to the clipboard.
                }
            }
        }
    }

    if (memory != nullptr)
    {
        GlobalFree(memory);
    }
    CloseClipboard();
    DestroyWindow(owner);
    return result;
}

class ExplorerCommand final : public IExplorerCommand
{
public:
    explicit ExplorerCommand(Action action = Action::Menu) noexcept : action_(action)
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
        }
        return E_UNEXPECTED;
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
        const wchar_t* relativeIcon = action_ == Action::Menu ? L"Assets\\Sonnenberg.ico" :
            action_ == Action::CopyPath ? L"Assets\\CopyPath.ico" : L"Assets\\CopyForwardPath.ico";
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
        *commandName = action_ == Action::Menu ? kSonnenbergCommandClsid :
            action_ == Action::CopyPath ? kCopyPathCommandClsid : kCopyForwardPathCommandClsid;
        return S_OK;
    }

    IFACEMETHODIMP GetState(
        IShellItemArray* selection,
        BOOL,
        EXPCMDSTATE* state) noexcept override
    {
        if (state == nullptr)
        {
            return E_POINTER;
        }
        *state = selection == nullptr ? ECS_DISABLED : ECS_ENABLED;
        return S_OK;
    }

    IFACEMETHODIMP Invoke(IShellItemArray* selection, IBindCtx*) noexcept override
    try
    {
        if (action_ == Action::Menu)
        {
            return E_NOTIMPL;
        }
        if (selection == nullptr)
        {
            return E_INVALIDARG;
        }

        DWORD itemCount = 0;
        HRESULT result = selection->GetCount(&itemCount);
        if (FAILED(result))
        {
            return result;
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
    ~ExplorerCommand() noexcept
    {
        --g_objectCount;
    }

    std::atomic<ULONG> referenceCount_ = 1;
    const Action action_;
};

class SubCommandEnumerator final : public IEnumExplorerCommand
{
public:
    explicit SubCommandEnumerator(ULONG position = 0) noexcept : position_(position)
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
            auto* command = new (std::nothrow) ExplorerCommand(kSubCommands[position_ + supplied]);
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
        *commands = new (std::nothrow) SubCommandEnumerator(position_);
        return *commands != nullptr ? S_OK : E_OUTOFMEMORY;
    }

private:
    ~SubCommandEnumerator() noexcept { --g_objectCount; }

    std::atomic<ULONG> references_ = 1;
    ULONG position_;
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
    *commands = new (std::nothrow) SubCommandEnumerator();
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
