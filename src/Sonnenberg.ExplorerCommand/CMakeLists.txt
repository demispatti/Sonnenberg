cmake_minimum_required(VERSION 3.25)

project(SonnenbergExplorerCommand LANGUAGES CXX)

if(NOT WIN32)
  message(FATAL_ERROR "Sonnenberg.ExplorerCommand is a Windows-only COM host")
endif()

set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

set(line_extensions_file "${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Resources/Files/LineCountingExtensions.txt")
set_property(DIRECTORY APPEND PROPERTY CMAKE_CONFIGURE_DEPENDS "${line_extensions_file}")
file(STRINGS "${line_extensions_file}" line_extensions)
list(TRANSFORM line_extensions REPLACE "^(.+)$" "L\"\\1\",")
list(JOIN line_extensions "\n" line_extensions_source)
file(GENERATE OUTPUT "${CMAKE_CURRENT_BINARY_DIR}/LineCountingExtensions.inc" CONTENT "${line_extensions_source}\n")

add_library(SonnenbergExplorerCommand SHARED
  Clipboard.cpp
  ContextActions.cpp
  ExplorerCommand.cpp
  ExplorerCommand.def
)

target_compile_features(SonnenbergExplorerCommand PRIVATE cxx_std_20)
target_include_directories(SonnenbergExplorerCommand PRIVATE "${CMAKE_CURRENT_BINARY_DIR}")
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
  /utf-8
  /EHsc
)
target_link_options(SonnenbergExplorerCommand PRIVATE /guard:cf /DYNAMICBASE /NXCOMPAT)
target_link_libraries(SonnenbergExplorerCommand PRIVATE Ole32 Shell32 Shlwapi User32 Uuid)

file(GENERATE OUTPUT "${CMAKE_CURRENT_BINARY_DIR}/PackageEntry.rc"
  CONTENT "1 ICON \"${CMAKE_CURRENT_SOURCE_DIR}/../../packaging/Assets/Sonnenberg.ico\"\n")
add_executable(Sonnenberg WIN32 PackageEntry.cpp Clipboard.cpp LineCount.cpp "${CMAKE_CURRENT_BINARY_DIR}/PackageEntry.rc")
target_compile_features(Sonnenberg PRIVATE cxx_std_20)
target_compile_options(Sonnenberg PRIVATE /W4 /WX /guard:cf /sdl /utf-8 /EHsc)
target_link_options(Sonnenberg PRIVATE /guard:cf /DYNAMICBASE /NXCOMPAT)
target_link_libraries(Sonnenberg PRIVATE Shell32 User32)

add_custom_target(SonnenbergMsix
  COMMAND powershell.exe -NoProfile -ExecutionPolicy Bypass
    -File "${CMAKE_CURRENT_SOURCE_DIR}/../../packaging/Build-MSIX.ps1"
    -NativeOutputDirectory "$<TARGET_FILE_DIR:Sonnenberg>"
    -Architecture "${CMAKE_VS_PLATFORM_NAME}"
    -OutputDirectory "${CMAKE_CURRENT_BINARY_DIR}/msix" -TestSign
  DEPENDS Sonnenberg SonnenbergExplorerCommand
  VERBATIM
)

add_executable(SonnenbergExplorerCommandSmokeTests
  LineCount.cpp
  ../../tests/Sonnenberg.ExplorerCommand.SmokeTests/ExplorerCommandSmokeTests.cpp
)
target_compile_features(SonnenbergExplorerCommandSmokeTests PRIVATE cxx_std_20)
target_include_directories(SonnenbergExplorerCommandSmokeTests PRIVATE "${CMAKE_CURRENT_SOURCE_DIR}")
target_compile_definitions(SonnenbergExplorerCommandSmokeTests PRIVATE
  UNICODE
  _UNICODE
  WIN32_LEAN_AND_MEAN
  NOMINMAX
)
target_compile_options(SonnenbergExplorerCommandSmokeTests PRIVATE /W4 /WX /permissive- /guard:cf /sdl /utf-8 /EHsc)
target_link_options(SonnenbergExplorerCommandSmokeTests PRIVATE /guard:cf /DYNAMICBASE /NXCOMPAT)
target_link_libraries(SonnenbergExplorerCommandSmokeTests PRIVATE Ole32 Shell32)

enable_testing()
add_test(
  NAME SonnenbergExplorerCommandSmokeTests
  COMMAND SonnenbergExplorerCommandSmokeTests $<TARGET_FILE:SonnenbergExplorerCommand>
    "${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Resources/Files/TextFilesExtensions.json"
)
