using System.Collections.Generic;
using System.Windows.Forms;
using Sonnenberg.Core.Models;

namespace Sonnenberg.UI.Menus
{
    /// <summary>
    /// Provides menu items for different features
    /// </summary>
    public interface IMenuItemProvider
    {
        ToolStripMenuItem CreateMainMenuItem(string text, bool isDarkTheme);
        IEnumerable<ToolStripMenuItem> CreateCopyPathItems(ItemContext context, bool isDarkTheme);
        IEnumerable<ToolStripMenuItem> CreateShellLauncherItems(ItemContext context, bool isDarkTheme);
        IEnumerable<ToolStripMenuItem> CreateLineCounterItems(ItemContext context, bool isDarkTheme);
    }
}
