using System.Diagnostics.CodeAnalysis;

namespace Beater;

public static class Extensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)]this string? value)
    {
        return string.IsNullOrEmpty(value);
    }
}
