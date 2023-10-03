using Sonnenberg.Settings.Properties;

namespace Sonnenberg.Settings
{
    public class Settings
    {
        public static void ResetSettings()
        {
            UserSettings.Default.clickedItemType = "";
            UserSettings.Default.clickedItemPath = "";
            UserSettings.Default.clickedItemContainingFolder = "";
            UserSettings.Default.shellStartUpDirectory = "";
            UserSettings.Default.shortcutTarget = "";
            UserSettings.Default.shortcutTargetFolder = "";
            UserSettings.Default.shortcutTargetType = "";
            UserSettings.Default.hasMenuWatcherSubscribed = false;
            UserSettings.Default.hasClickedItemType = false;
        }
    }
}