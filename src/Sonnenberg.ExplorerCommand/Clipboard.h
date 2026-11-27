#pragma once

#include <windows.h>
#include <string>

HRESULT CopyTextToClipboard(const std::wstring& text) noexcept;
