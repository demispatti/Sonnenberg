using System.Windows.Forms;
using Sonnenberg.Core.Models;

namespace Sonnenberg.Core.Interfaces
{
    /// <summary>
    /// Factory for creating context menus based on item context
    /// </summary>
    public interface IMenuFactory
    {
        ContextMenuStrip CreateMenu(ItemContext context, bool isDarkTheme);
    }
}
