#include <windows.h>
#include <shellapi.h>

#include "Clipboard.h"
#include "LineCount.h"

#include <cstdint>
#include <filesystem>
#include <fstream>
#include <memory>
#include <string>



int WINAPI wWinMain(HINSTANCE, HINSTANCE, PWSTR, int)
try
{
    int count = 0;
    const std::unique_ptr<wchar_t*, decltype(&LocalFree)> arguments(
        CommandLineToArgvW(GetCommandLineW(), &count), LocalFree);
    if (!arguments || count != 3)
    {
        return 0;
    }
    const std::wstring_view action(arguments.get()[1]);
    if (action != L"--count-lines" && action != L"--count-nonempty-lines")
    {
        return 2;
    }

    const bool german = PRIMARYLANGID(GetUserDefaultUILanguage()) == LANG_GERMAN;
    const auto path = arguments.get()[2];
    std::uint64_t lines = 0;
    std::ifstream input(std::filesystem::path(path), std::ios::binary);
    if (!CountLines(input, action == L"--count-nonempty-lines", lines))
    {
        MessageBoxW(nullptr, german ? L"Die Datei konnte nicht als Text gelesen werden." :
            L"The file could not be read as text.", L"Sonnenberg", MB_OK | MB_ICONERROR);
        return 1;
    }
    const auto result = std::filesystem::path(path).filename().wstring() + L"\n" +
        std::to_wstring(lines) + (german ? L" Zeilen" : L" lines");
    const auto prompt = result + (german ? L"\n\nErgebnis in die Zwischenablage kopieren?" :
        L"\n\nCopy the result to the clipboard?");
    if (MessageBoxW(nullptr, prompt.c_str(), L"Sonnenberg", MB_YESNO | MB_ICONINFORMATION) == IDYES &&
        FAILED(CopyTextToClipboard(std::to_wstring(lines))))
    {
        MessageBoxW(nullptr, german ? L"Die Zwischenablage ist nicht verfügbar." :
            L"The clipboard is unavailable.", L"Sonnenberg", MB_OK | MB_ICONERROR);
        return 1;
    }
    return 0;
}
catch (...)
{
    return 1;
}
