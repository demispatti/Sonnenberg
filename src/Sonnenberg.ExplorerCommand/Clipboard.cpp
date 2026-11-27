#include "Clipboard.h"
#include <cwchar>

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
