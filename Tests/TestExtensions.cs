namespace Beater;

public static class TestExtensions
{
    public static List<string>? GetTimestamps(this Sequence? source)
    {
        return source?.Select(sound => $"{sound.Timestamp:0000}:{sound.Name?.Trim()}").ToList();
    }
}
