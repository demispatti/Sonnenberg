namespace Sonnenberg.Core.Models
{
    /// <summary>
    /// Contains information about a Windows shortcut (.lnk file)
    /// </summary>
    public class ShortcutInfo
    {
        public string TargetPath { get; set; }
        public string TargetFolder { get; set; }
        public ItemType TargetType { get; set; }
    }
}
