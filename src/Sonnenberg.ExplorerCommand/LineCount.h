#pragma once

#include <cstdint>
#include <istream>

bool CountLines(std::istream& input, bool omitEmpty, std::uint64_t& count);
