using System;
using log4net;
using Microsoft.Win32;
using Sonnenberg.Core.Interfaces;

namespace Sonnenberg.Infrastructure.Theming
{
    /// <summary>
    /// Detects whether Windows is using dark or light theme
    /// </summary>
    public class WindowsThemeDetector : IThemeDetector
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WindowsThemeDetector));
        private const string ThemeRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string ThemeValueName = "SystemUsesLightTheme";

        public bool IsDarkTheme()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(ThemeRegistryPath))
                {
                    var value = key?.GetValue(ThemeValueName);
                    if (value != null)
                    {
                        // Value is 0 for dark theme, 1 for light theme
                        return value.ToString() == "0";
                    }

                    // Default to light theme if registry key doesn't exist
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Failed to detect Windows theme, defaulting to light theme", ex);
                return false;
            }
        }
    }
}
