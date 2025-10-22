using System;
using System.IO;
using log4net;

namespace Sonnenberg.Features.LineCounter
{
    /// <summary>
    /// Service for counting lines in text files
    /// </summary>
    public class LineCounterService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LineCounterService));

        public LineCountResult CountLines(string filePath, bool excludeBlankLines = false)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            try
            {
                var lineCount = excludeBlankLines
                    ? CountNonBlankLines(filePath)
                    : CountAllLines(filePath);

                return new LineCountResult
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    LineCount = lineCount,
                    ExcludedBlankLines = excludeBlankLines
                };
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to count lines in file: {filePath}", ex);
                throw;
            }
        }

        private int CountAllLines(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return lines.Length;
        }

        private int CountNonBlankLines(string filePath)
        {
            var count = 0;
            using (var reader = File.OpenText(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
