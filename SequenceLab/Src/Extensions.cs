using System.Diagnostics.CodeAnalysis;

namespace Beater;

public static class Extensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool HasValue([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static bool ContainsSafe<T>(this IEnumerable<T>? source, T item)
    {
        return source is not null && source.Contains(item);
    }

    public static bool HasItems<T>(this IEnumerable<T>? source)
    {
        return source is not null && source.Count() > 0;
    }

    public static string Join<T>(this IEnumerable<T>? source, char delimiter = ',')
    {
        return string.Join(delimiter, source ?? []);
    }

    public static T? SecondOrDefault<T>(this IEnumerable<T>? source)
    {
        return source is null ? default : source.ElementAtOrDefault(1);
    }
}
