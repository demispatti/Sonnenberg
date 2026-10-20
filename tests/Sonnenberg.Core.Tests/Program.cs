using Sonnenberg.Core;

var failures = new List<string>();

Check("counts empty lines", 3, () =>
{
    using var input = new StringReader("first\n\nthird\n");
    return LineCounter.Count(input, omitEmptyLines: false);
});

Check("omits only empty lines", 3, () =>
{
    using var input = new StringReader("first\n\n   \nthird\n");
    return LineCounter.Count(input, omitEmptyLines: true);
});

Check("converts Windows path separators", "C:/work/file.txt", () =>
    PathFormatter.Format(@"C:\work\file.txt", useForwardSlashes: true));

Check("preserves Windows path separators", @"C:\work\file.txt", () =>
    PathFormatter.Format(@"C:\work\file.txt", useForwardSlashes: false));

CheckThrows<ArgumentNullException>("rejects a null reader", () => LineCounter.Count(null!, false));
CheckThrows<ArgumentNullException>("rejects a null path", () => PathFormatter.Format(null!, true));

if (failures.Count > 0)
{
    Console.Error.WriteLine($"{failures.Count} portable core test(s) failed:");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine("6 portable core tests passed.");
return 0;

void Check<T>(string name, T expected, Func<T> action)
{
    try
    {
        var actual = action();
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            failures.Add($"{name}: expected '{expected}', got '{actual}'");
        }
    }
    catch (Exception exception)
    {
        failures.Add($"{name}: unexpected {exception.GetType().Name}: {exception.Message}");
    }
}

void CheckThrows<TException>(string name, Action action) where TException : Exception
{
    try
    {
        action();
        failures.Add($"{name}: expected {typeof(TException).Name}");
    }
    catch (TException)
    {
    }
    catch (Exception exception)
    {
        failures.Add($"{name}: expected {typeof(TException).Name}, got {exception.GetType().Name}");
    }
}
