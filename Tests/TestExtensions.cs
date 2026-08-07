using Beater;
using FluentAssertions.Collections;

namespace Tests;

public static class TestExtensions
{
    public static List<string>? GetTimestamps(this GeneratedSequence? source)
    {
        return source?.Select(sound => $"{sound.Timestamp:0000}:{(sound.SoundDesign?.FriendlyName ?? sound.Name)?.Trim()}").ToList();
    }

    public static string Print(this GeneratedSequence? source)
    {
        var timestamps = source?.GetTimestamps();
        var text = string.Join(",\r\n", timestamps ?? []);
        Console.WriteLine(text);

        return text;
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
        catch (Exception)
        {
            // print out actual timestamps for easier debugging:
            var actual = "\"" + string.Join("\",\r\n\"", should.Subject) + "\"";
            Console.WriteLine($"{Environment.NewLine}Actual:{Environment.NewLine}{actual}");

            throw;
        }
    }

    /// <summary>
    /// Test-only helper to set generation-time state (Timestamp/Iteration) on a SoundDesign fixture.
    /// </summary>
    public static T With<T>(this T sound, Action<T> callback) where T : SoundDesign
    {
        callback(sound);
        return sound;
    }

}
