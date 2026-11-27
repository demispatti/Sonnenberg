#include <windows.h>

#include <shobjidl_core.h>

#include <iostream>
#include <string>
#include <filesystem>
#include <sstream>
#include <wrl/client.h>

#include "LineCount.h"

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
    if (argumentCount != 3)
    {
        std::wcerr << L"Usage: SonnenbergExplorerCommandSmokeTests <command-dll> <text-fixture>\n";
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
    std::istringstream lines("first\r\n\r\nsecond\nthird");
    std::uint64_t count = 0;
    passed &= Check(CountLines(lines, false, count) && count == 4, L"count CRLF, blank and final lines");
    lines.clear();
    lines.seekg(0);
    passed &= Check(CountLines(lines, true, count) && count == 3, L"omit empty lines");
    const char utf16[] = "\xff\xfe" "A\0\r\0\n\0\n\0B\0";
    std::istringstream unicode(std::string(utf16, sizeof(utf16) - 1));
    passed &= Check(CountLines(unicode, true, count) && count == 2, L"count UTF-16 with BOM");
    std::istringstream binaryContent(std::string("A\0B", 3));
    passed &= Check(!CountLines(binaryContent, false, count), L"reject binary content");
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
            using Microsoft::WRL::ComPtr;
            const auto makeSelection = [](const wchar_t* path) {
                ComPtr<IShellItem> item;
                ComPtr<IShellItemArray> selection;
                const auto nativePath = std::filesystem::path(path).lexically_normal().make_preferred();
                HRESULT result = SHCreateItemFromParsingName(nativePath.c_str(), nullptr, IID_PPV_ARGS(item.GetAddressOf()));
                if (SUCCEEDED(result))
                {
                    result = SHCreateShellItemArrayFromShellItem(item.Get(), IID_PPV_ARGS(selection.GetAddressOf()));
                }
                if (FAILED(result))
                {
                    std::wcerr << L"Cannot create test selection: " << nativePath.c_str()
                        << L" (0x" << std::hex << result << L")\n";
                }
                return selection;
            };
            const auto folderPath = std::filesystem::path(arguments[1]).parent_path();
            const auto folder = makeSelection(folderPath.c_str());
            const auto textFile = makeSelection(arguments[2]);
            const auto binary = makeSelection(arguments[1]);
            passed &= Check(folder && textFile && binary, L"create file and folder selections");
            const bool german = PRIMARYLANGID(GetUserDefaultUILanguage()) == LANG_GERMAN;
            const wchar_t* titles[] = {
                german ? L"Pfad kopieren" : L"Copy path",
                german ? L"Pfad mit Schrägstrichen kopieren" : L"Copy path with forward slashes",
                german ? L"Eingabeaufforderung hier öffnen" : L"Open Command Prompt here",
                german ? L"Eingabeaufforderung hier als Administrator öffnen" :
                    L"Open Command Prompt here as administrator",
                german ? L"PowerShell hier öffnen" : L"Open PowerShell here",
                german ? L"PowerShell hier als Administrator öffnen" : L"Open PowerShell here as administrator",
                german ? L"Zeilen zählen" : L"Count lines",
                german ? L"Zeilen ohne Leerzeilen zählen" : L"Count non-empty lines",
                german ? L"Zielordner öffnen" : L"Open shortcut target folder"
            };
            for (size_t index = 0; index < ARRAYSIZE(titles); ++index)
            {
                IExplorerCommand* verb = nullptr;
                passed &= Check(submenu->Next(1, &verb, nullptr) == S_OK && verb != nullptr,
                    L"read submenu item");
                if (verb != nullptr)
                {
                    wchar_t* verbTitle = nullptr;
                    passed &= Check(SUCCEEDED(verb->GetTitle(nullptr, &verbTitle)) &&
                        verbTitle != nullptr && std::wstring(verbTitle) == titles[index], L"submenu item title");
                    CoTaskMemFree(verbTitle);
                    passed &= Check(verb->Invoke(nullptr, nullptr) == E_INVALIDARG, L"reject missing selection");
                    const auto checkState = [verb](IShellItemArray* selection, bool enabled) {
                        EXPCMDSTATE visibility = ECS_DISABLED;
                        return SUCCEEDED(verb->GetState(selection, TRUE, &visibility)) &&
                            visibility == static_cast<EXPCMDSTATE>(enabled ? ECS_ENABLED : ECS_HIDDEN);
                    };
                    passed &= Check(checkState(folder.Get(), index < 6), L"folder command visibility");
                    passed &= Check(checkState(textFile.Get(), index < 2 || index == 6 || index == 7),
                        L"text file command visibility");
                    passed &= Check(checkState(binary.Get(), index < 2), L"binary file command visibility");
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
