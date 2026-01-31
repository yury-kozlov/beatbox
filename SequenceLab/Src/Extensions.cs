using System.Diagnostics.CodeAnalysis;

namespace Beater;

public static class Extensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)]this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool HasValue([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static bool ContainsSafe<T>(this List<T>? source, T item)
    {
        return source is not null && source.Contains(item);
    }

    public static bool HasItems<T>(this List<T>? source)
    {
        return source is not null && source.Count > 0;
    }

    public static string Join<T>(this List<T>? source, char delimiter = ',')
    {
        return string.Join(delimiter, source ?? []);
    }
}
