namespace Sonnenberg.Core.Interfaces
{
    /// <summary>
    /// Detects the current Windows theme (dark/light mode)
    /// </summary>
    public interface IThemeDetector
    {
        bool IsDarkTheme();
    }
}
