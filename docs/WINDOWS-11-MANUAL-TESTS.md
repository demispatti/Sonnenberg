# Windows 11 Explorer integration test

Run this checklist on Windows 11 x64 and ARM64 with the corresponding MSIX. Use a disposable local account or a VM snapshot where possible.

## Before testing

1. Record Windows edition, build number, CPU architecture, display scaling, and Explorer theme.
2. Download the artifact from the corresponding successful GitHub Actions run.
3. Verify the artifact name and SHA-256 checksum supplied with the test request.
4. Confirm that the architecture in the artifact name matches the test PC.
5. Confirm that no previous Sonnenberg version is installed. Restart Explorer or reboot after uninstalling an older build.

## Install and registration

1. Trust the accompanying test certificate and install the matching MSIX using the [README instructions](../README.md#modernization).
2. Confirm Windows shows the expected publisher, product name, version, and architecture.
3. Restart Explorer if requested; a full reboot should not normally be required.
4. Open Task Manager and verify Explorer remains responsive and does not repeatedly restart.

## Context-menu behavior

Test a normal file, a text file, a folder, a shortcut, and a selection with multiple items.

1. Right-click each target and verify Sonnenberg appears in the primary Windows 11 menu for the modern package.
2. Open the Sonnenberg submenu and verify both path formats are available exactly once.
3. Paste the result of each command into Notepad and compare it to the selection. Check spaces, umlauts, apostrophes, and non-Latin characters.
4. Repeat with light and dark themes at 100% and 150% display scaling.

## Reliability and security

1. Rapidly open the menu 20 times and verify Explorer does not hang or leak visible processes.
2. Test an inaccessible folder and a very long path; verify a clear error is shown and Explorer stays alive.
3. Verify commands do not execute content from the selected filename and do not open a shell with unexpected elevation.
4. Sign out and back in, then reboot. Confirm registration persists and Explorer starts normally.
5. Uninstall the MSIX and remove its test certificate from Trusted People. Confirm the Sonnenberg submenu disappears from the main menu.

## Report

Attach screenshots of the primary and legacy menus and include Windows build, architecture, artifact run URL, SHA-256, pass/fail for every step, and any Event Viewer entries under **Windows Logs → Application** related to Explorer or Sonnenberg.
