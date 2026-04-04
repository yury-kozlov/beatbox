using Beater;
using FluentAssertions.Collections;

namespace Tests;

public static class TestExtensions
{
    public static List<string>? GetTimestamps(this Sequence? source)
    {
        return source?.Select(sound => $"{sound.Timestamp:0000}:{(sound.FriendlyName ?? sound.Name)?.Trim()}").ToList();
    }

    /// <summary>
    /// Asserts that the actual timestamp list matches <paramref name="expected"/> exactly, including order.
    /// </summary>
    public static void BeExactSequence(this StringCollectionAssertions should, IEnumerable<string> expected)
    {
        try
        {
            should.BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
        catch (Exception ex)
        {
            // print out actual timestamps for easier debugging:
            var actual = "\"" + string.Join("\",\r\n\"", should.Subject) + "\"";
            Console.WriteLine($"{Environment.NewLine}Actual:{Environment.NewLine}{actual}");

            throw;
        }
    }
}
