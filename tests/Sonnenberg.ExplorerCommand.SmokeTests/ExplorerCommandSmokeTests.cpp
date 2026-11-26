#include <windows.h>

#include <shobjidl_core.h>

#include <iostream>
#include <string>

namespace
{
constexpr CLSID kCopyPathCommandClsid = {
    0xb79f8fcc,
    0xb264,
    0x4d0b,
    {0xb0, 0xc9, 0xeb, 0xda, 0xac, 0xeb, 0xcf, 0xd6}};

using DllCanUnloadNowFunction = HRESULT(__stdcall*)();
using DllGetClassObjectFunction = HRESULT(__stdcall*)(REFCLSID, REFIID, void**);

bool Check(bool condition, const wchar_t* message)
{
    if (!condition)
    {
        std::wcerr << L"FAILED: " << message << L'\n';
    }
    return condition;
}
} // namespace

int wmain(int argumentCount, wchar_t** arguments)
{
    if (argumentCount != 2)
    {
        std::wcerr << L"Usage: SonnenbergExplorerCommandSmokeTests <command-dll>\n";
        return 2;
    }

    const HRESULT initializeResult = CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
    if (!Check(SUCCEEDED(initializeResult), L"COM initialization"))
    {
        return 1;
    }

    HMODULE module = LoadLibraryExW(arguments[1], nullptr, LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR);
    if (!Check(module != nullptr, L"load command DLL"))
    {
        CoUninitialize();
        return 1;
    }

    const auto canUnloadNow = reinterpret_cast<DllCanUnloadNowFunction>(
        GetProcAddress(module, "DllCanUnloadNow"));
    const auto getClassObject = reinterpret_cast<DllGetClassObjectFunction>(
        GetProcAddress(module, "DllGetClassObject"));

    bool passed = true;
    passed &= Check(canUnloadNow != nullptr, L"DllCanUnloadNow export");
    passed &= Check(getClassObject != nullptr, L"DllGetClassObject export");

    IClassFactory* factory = nullptr;
    if (getClassObject != nullptr)
    {
        passed &= Check(
            SUCCEEDED(getClassObject(
                kCopyPathCommandClsid,
                IID_IClassFactory,
                reinterpret_cast<void**>(&factory))),
            L"create class factory");
    }

    IExplorerCommand* command = nullptr;
    if (factory != nullptr)
    {
        passed &= Check(
            SUCCEEDED(factory->CreateInstance(
                nullptr,
                __uuidof(IExplorerCommand),
                reinterpret_cast<void**>(&command))),
            L"create IExplorerCommand");
        factory->Release();
    }

    if (command != nullptr)
    {
        wchar_t* title = nullptr;
        passed &= Check(SUCCEEDED(command->GetTitle(nullptr, &title)), L"read title");
        passed &= Check(title != nullptr && std::wstring(title) == L"Sonnenberg", L"title value");
        CoTaskMemFree(title);

        GUID canonicalName{};
        passed &= Check(
            SUCCEEDED(command->GetCanonicalName(&canonicalName)) &&
                IsEqualGUID(canonicalName, kCopyPathCommandClsid),
            L"canonical command GUID");

        EXPCMDSTATE state = ECS_ENABLED;
        passed &= Check(
            SUCCEEDED(command->GetState(nullptr, FALSE, &state)) && state == ECS_DISABLED,
            L"disable command without selection");

        EXPCMDFLAGS flags = ECF_DEFAULT;
        passed &= Check(
            SUCCEEDED(command->GetFlags(&flags)) && flags == ECF_HASSUBCOMMANDS,
            L"submenu flags");

        IEnumExplorerCommand* submenu = nullptr;
        passed &= Check(SUCCEEDED(command->EnumSubCommands(&submenu)) && submenu != nullptr,
            L"enumerate submenu");
        if (submenu != nullptr)
        {
            const bool german = PRIMARYLANGID(GetUserDefaultUILanguage()) == LANG_GERMAN;
            const wchar_t* titles[] = {
                german ? L"Pfad kopieren" : L"Copy path",
                german ? L"Pfad mit Schrägstrichen kopieren" : L"Copy path with forward slashes"
            };
            for (const wchar_t* expected : titles)
            {
                IExplorerCommand* verb = nullptr;
                passed &= Check(submenu->Next(1, &verb, nullptr) == S_OK && verb != nullptr,
                    L"read submenu item");
                if (verb != nullptr)
                {
                    wchar_t* verbTitle = nullptr;
                    passed &= Check(SUCCEEDED(verb->GetTitle(nullptr, &verbTitle)) &&
                        verbTitle != nullptr && std::wstring(verbTitle) == expected, L"submenu item title");
                    CoTaskMemFree(verbTitle);
                    passed &= Check(verb->Invoke(nullptr, nullptr) == E_INVALIDARG, L"reject missing selection");
                    verb->Release();
                }
            }
            IExplorerCommand* extra = nullptr;
            passed &= Check(submenu->Next(1, &extra, nullptr) == S_FALSE, L"end of submenu");
            if (extra != nullptr)
            {
                extra->Release();
            }
            submenu->Release();
        }
        command->Release();
    }

    if (canUnloadNow != nullptr)
    {
        passed &= Check(canUnloadNow() == S_OK, L"server unload after release");
    }

    FreeLibrary(module);
    CoUninitialize();

    if (!passed)
    {
        return 1;
    }

    std::wcout << L"Native Explorer command smoke tests passed.\n";
    return 0;
}
