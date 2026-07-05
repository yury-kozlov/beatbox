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

    public static bool HasItems<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source is not null && source.Any();
    }

    public static string Join<T>(this IEnumerable<T>? source, char delimiter = ',')
    {
        return string.Join(delimiter, source ?? []);
    }

    public static T? SecondOrDefault<T>(this IEnumerable<T>? source)
    {
        return source is null ? default : source.ElementAtOrDefault(1);
    }

    public static void MoveBefore<T>(this List<T> source, int targetIndex, int indexToMove)
    {
        if (indexToMove == targetIndex)
        {
            return;
        }

        var item = source[indexToMove];
        source.RemoveAt(indexToMove);

        if (indexToMove < targetIndex)
        {
            targetIndex--;
        }

        source.Insert(targetIndex, item);
    }
}
