namespace Beater;

public static class TestExtensions
{
    public static List<string>? GetTimestamps(this List<Sound>? source)
    {
        return source?.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}").ToList();
    }
}
