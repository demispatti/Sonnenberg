using System.Collections.Generic;

namespace Sonnenberg.Core.Models
{
    /// <summary>
    /// Represents the context of an item clicked in Windows Explorer
    /// </summary>
    public class ItemContext
    {
        public ItemType Type { get; set; }
        public string Path { get; set; }
        public string ContainingFolder { get; set; }
        public string ShellStartUpDirectory { get; set; }
        public ShortcutInfo ShortcutInfo { get; set; }
        public IEnumerable<string> SelectedPaths { get; set; }
    }
}
