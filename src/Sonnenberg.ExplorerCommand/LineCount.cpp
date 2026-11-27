#include "LineCount.h"
#include <cstdio>

bool CountLines(std::istream& input, bool omitEmpty, std::uint64_t& count)
{
    count = 0;
    if (!input)
    {
        return false;
    }

    unsigned char prefix[4]{};
    input.read(reinterpret_cast<char*>(prefix), sizeof(prefix));
    const auto prefixSize = input.gcount();
    int width = 1;
    int bom = 0;
    bool bigEndian = false;
    if (prefixSize >= 4 && prefix[0] == 0xff && prefix[1] == 0xfe && prefix[2] == 0 && prefix[3] == 0)
    {
        width = bom = 4;
    }
    else if (prefixSize >= 4 && prefix[0] == 0 && prefix[1] == 0 && prefix[2] == 0xfe && prefix[3] == 0xff)
    {
        width = bom = 4;
        bigEndian = true;
    }
    else if (prefixSize >= 2 && prefix[0] == 0xff && prefix[1] == 0xfe)
    {
        width = bom = 2;
    }
    else if (prefixSize >= 2 && prefix[0] == 0xfe && prefix[1] == 0xff)
    {
        width = bom = 2;
        bigEndian = true;
    }
    else if (prefixSize >= 3 && prefix[0] == 0xef && prefix[1] == 0xbb && prefix[2] == 0xbf)
    {
        bom = 3;
    }
    input.clear();
    input.seekg(bom);
    if (!input)
    {
        return false;
    }

    bool hasCharacters = false;
    bool afterCarriageReturn = false;
    unsigned int unit = 0;
    int index = 0;
    for (int byte = input.get(); byte != EOF; byte = input.get())
    {
        const int shift = 8 * (bigEndian ? width - index - 1 : index);
        unit |= static_cast<unsigned int>(byte) << shift;
        if (++index != width)
        {
            continue;
        }
        if (unit == 0 && width == 1)
        {
            return false;
        }
        if (unit == L'\r' || unit == L'\n')
        {
            const bool partOfCrLf = afterCarriageReturn && unit == L'\n';
            if (!partOfCrLf && (!omitEmpty || hasCharacters))
            {
                ++count;
            }
            hasCharacters = false;
        }
        else
        {
            hasCharacters = true;
        }
        afterCarriageReturn = unit == L'\r';
        unit = 0;
        index = 0;
    }
    if (hasCharacters)
    {
        ++count;
    }
    return input.eof() && !input.bad() && index == 0;
}
