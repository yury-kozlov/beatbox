using System.Diagnostics.CodeAnalysis;

namespace Beater;

public static class TestExtensions
{
    public static List<string>? GetTimestamps(this List<SequenceMessage>? source)
    {
        return source?.Select(msg => $"{msg.Timestamp:0000}:{msg.Name?.Trim()}").ToList();
    }
}
