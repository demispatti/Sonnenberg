cmake_minimum_required(VERSION 3.25)

project(SonnenbergExplorerCommand LANGUAGES CXX)

if(NOT WIN32)
  message(FATAL_ERROR "Sonnenberg.ExplorerCommand is a Windows-only COM host")
endif()

add_library(SonnenbergExplorerCommand SHARED
  ExplorerCommand.cpp
  ExplorerCommand.def
)

target_compile_features(SonnenbergExplorerCommand PRIVATE cxx_std_20)
target_compile_definitions(SonnenbergExplorerCommand PRIVATE
  UNICODE
  _UNICODE
  WIN32_LEAN_AND_MEAN
  NOMINMAX
)
target_compile_options(SonnenbergExplorerCommand PRIVATE
  /W4
  /WX
  /permissive-
  /guard:cf
  /sdl
)
target_link_options(SonnenbergExplorerCommand PRIVATE /guard:cf /DYNAMICBASE /NXCOMPAT)
target_link_libraries(SonnenbergExplorerCommand PRIVATE Ole32 Shell32 User32)

add_executable(SonnenbergExplorerCommandSmokeTests
  ../../tests/Sonnenberg.ExplorerCommand.SmokeTests/ExplorerCommandSmokeTests.cpp
)
target_compile_features(SonnenbergExplorerCommandSmokeTests PRIVATE cxx_std_20)
target_compile_definitions(SonnenbergExplorerCommandSmokeTests PRIVATE
  UNICODE
  _UNICODE
  WIN32_LEAN_AND_MEAN
  NOMINMAX
)
target_compile_options(SonnenbergExplorerCommandSmokeTests PRIVATE /W4 /WX /permissive- /guard:cf /sdl)
target_link_options(SonnenbergExplorerCommandSmokeTests PRIVATE /guard:cf /DYNAMICBASE /NXCOMPAT)
target_link_libraries(SonnenbergExplorerCommandSmokeTests PRIVATE Ole32 Shell32)

enable_testing()
add_test(
  NAME SonnenbergExplorerCommandSmokeTests
  COMMAND SonnenbergExplorerCommandSmokeTests $<TARGET_FILE:SonnenbergExplorerCommand>
)
