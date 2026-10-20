#include <windows.h>

#include <shobjidl_core.h>

#include <atomic>
#include <cwchar>
#include <new>
#include <string>

namespace
{
// Keep this identifier in sync with packaging/AppxManifest.template.xml.
constexpr CLSID kCopyPathCommandClsid = {
    0xb79f8fcc,
    0xb264,
    0x4d0b,
    {0xb0, 0xc9, 0xeb, 0xda, 0xac, 0xeb, 0xcf, 0xd6}};

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
    if (!OpenClipboard(nullptr))
    {
        return HRESULT_FROM_WIN32(GetLastError());
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
    return result;
}

class ExplorerCommand final : public IExplorerCommand
{
public:
    ExplorerCommand() noexcept
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
        return CopyString(L"Copy path", title);
    }

    IFACEMETHODIMP GetIcon(IShellItemArray*, wchar_t** icon) noexcept override
    {
        if (icon == nullptr)
        {
            return E_POINTER;
        }
        *icon = nullptr;
        return E_NOTIMPL;
    }

    IFACEMETHODIMP GetToolTip(IShellItemArray*, wchar_t** toolTip) noexcept override
    {
        return CopyString(L"Copy the selected file-system path", toolTip);
    }

    IFACEMETHODIMP GetCanonicalName(GUID* commandName) noexcept override
    {
        if (commandName == nullptr)
        {
            return E_POINTER;
        }
        *commandName = kCopyPathCommandClsid;
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
    {
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
            if (FAILED(result))
            {
                return result;
            }

            if (!paths.empty())
            {
                paths.append(L"\r\n");
            }
            paths.append(path);
            CoTaskMemFree(path);
        }

        if (paths.empty())
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
        }

        return CopyTextToClipboard(paths);
    }

    IFACEMETHODIMP GetFlags(EXPCMDFLAGS* flags) noexcept override
    {
        if (flags == nullptr)
        {
            return E_POINTER;
        }
        *flags = ECF_DEFAULT;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** commands) noexcept override
    {
        if (commands == nullptr)
        {
            return E_POINTER;
        }
        *commands = nullptr;
        return E_NOTIMPL;
    }

private:
    ~ExplorerCommand() noexcept
    {
        --g_objectCount;
    }

    std::atomic<ULONG> referenceCount_ = 1;
};

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

extern "C" HRESULT __stdcall DllCanUnloadNow() noexcept
{
    return g_objectCount.load() == 0 && g_serverLocks.load() == 0 ? S_OK : S_FALSE;
}

extern "C" HRESULT __stdcall DllGetClassObject(
    REFCLSID classId,
    REFIID interfaceId,
    void** object) noexcept
{
    if (object == nullptr)
    {
        return E_POINTER;
    }
    *object = nullptr;

    if (!IsEqualCLSID(classId, kCopyPathCommandClsid))
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
        DisableThreadLibraryCalls(instance);
    }
    return TRUE;
}
