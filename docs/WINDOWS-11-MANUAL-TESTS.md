# Windows 11 Explorer integration test

Use a Windows 11 PC that is not the development machine. Run the checklist once on x64 and once on ARM64 when native packages exist. Use a disposable local account or a VM snapshot where possible.

## Before testing

1. Record Windows edition, build number, CPU architecture, display scaling, and Explorer theme.
2. Download the artifact from the corresponding successful GitHub Actions run.
3. Verify the artifact name and SHA-256 checksum supplied with the test request.
4. Confirm that no previous Sonnenberg version is installed. Restart Explorer or reboot after uninstalling an older build.
5. Do not install an unsigned artifact unless the test owner has explicitly explained how the test certificate is trusted and how to remove it afterward.

## Install and registration

1. Install the x64 package on x64 Windows or the ARM64 package on ARM64 Windows.
2. Confirm Windows shows the expected publisher, product name, version, and architecture.
3. Restart Explorer if requested; a full reboot should not normally be required.
4. Open Task Manager and verify Explorer remains responsive and does not repeatedly restart.

## Context-menu behavior

Test a normal file, a text file, a folder, a directory background, a shortcut to a file, and a shortcut to a folder.

1. Right-click each target and verify Sonnenberg appears in the primary Windows 11 menu for the modern package.
2. For the transitional SharpShell build, verify it appears under **Show more options** and record that as expected behavior.
3. Check that commands shown for each target are appropriate and that no duplicates appear.
4. Copy both Windows-style and forward-slash paths; paste into Notepad and compare them with the selected item.
5. Open PowerShell and Command Prompt; verify the working directory and that paths containing spaces, umlauts, apostrophes, and non-Latin characters work.
6. Count lines in files containing CRLF, LF, empty lines, whitespace-only lines, UTF-8 text, and a large file. Compare the results with a trusted editor.
7. Repeat with light/dark themes and 100%/150% display scaling; verify icons and text remain legible.

## Reliability and security

1. Rapidly open the menu 20 times and verify Explorer does not hang or leak visible processes.
2. Test an inaccessible folder and a very long path; verify a clear error is shown and Explorer stays alive.
3. Verify commands do not execute content from the selected filename and do not open a shell with unexpected elevation.
4. Sign out and back in, then reboot. Confirm registration persists and Explorer starts normally.
5. Uninstall Sonnenberg. Confirm all menu entries disappear and no service, startup entry, package, or test certificate remains.

## Report

Attach screenshots of the primary and legacy menus and include Windows build, architecture, artifact run URL, SHA-256, pass/fail for every step, and any Event Viewer entries under **Windows Logs → Application** related to Explorer or Sonnenberg.
