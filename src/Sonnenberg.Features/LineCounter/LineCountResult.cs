namespace Sonnenberg.Features.LineCounter
{
    /// <summary>
    /// Result of a line counting operation
    /// </summary>
    public class LineCountResult
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int LineCount { get; set; }
        public bool ExcludedBlankLines { get; set; }
    }
}
