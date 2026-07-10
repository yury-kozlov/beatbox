namespace Beater;

internal class SequenceDebuggerDisplay
{
    /// <summary>
    /// Example returned value: "k, 1200 k, 1200 k, 600 k, 600 k".
    /// </summary>
    internal static string Get(Sequence sequence)
    {
        var result = new List<string>();
        FillDebuggerDisplayStrings(result, sequence);
        return string.Join(", ", result);
    }

    private static void FillDebuggerDisplayStrings(List<string> result, Sequence sequence, int recursionDepth = 0)
    {
        const int maxRecursion = 10;
        if (recursionDepth >= maxRecursion)
        {
            return;
        }
        recursionDepth++;

        foreach (var sound in sequence.Where(s => s is not NoSound))
        {
            if (sound.DelayAfterLeader > 0)
            {
                result.Add($"{sound.DelayAfterLeader} {sound.Name}");
            }
            else
            {
                result.Add(sound.Name!);
            }
            FillDebuggerDisplayStrings(result, sound.Followers, recursionDepth);
        }
    }
}
