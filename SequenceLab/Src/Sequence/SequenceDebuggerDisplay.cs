namespace Beater;

internal class SequenceDebuggerDisplay
{
    /// <summary>
    /// Example returned value: "k, 1200 k, 1200 k, 600 k, 600 k".
    /// </summary>
    internal static string Get(Sequence sequence)
    {
        var result = new List<Sound>();
        FillDebuggerDisplayStrings(result, sequence);
        return string.Join(", ", result.Select(DebuggerDisplay));
    }

    /// <summary>
    /// Example returned value: "k, 1200 k, 1200 k, 600 k, 600 k".
    /// </summary>
    internal static string Get(Sound leader)
    {
        List<Sound> result = leader is NoSound ? [] : [leader];
        FillDebuggerDisplayStrings(result, leader.Followers);
        return string.Join(", ", result.Select(DebuggerDisplay));
    }

    private static void FillDebuggerDisplayStrings(List<Sound> result, Sequence sequence, int recursionDepth = 0)
    {
        const int maxRecursion = 10;
        if (recursionDepth >= maxRecursion)
        {
            return;
        }
        recursionDepth++;

        foreach (var sound in sequence)
        {
            if (sound is NoSound)
            {
                if (sound.Followers.Any())
                {
                    FillDebuggerDisplayStrings(result, sound.Followers, recursionDepth);
                }
                // don't display system sounds in debugger
                continue;
            }

            if (!result.Any(s => s.Id == sound.Id))
            {
                result.Add(sound);
            }

            FillDebuggerDisplayStrings(result, sound.Followers, recursionDepth);
        }
    }

    private static string DebuggerDisplay(Sound sound)
    {
        if (sound.DelayAfterLeader > 0)
        {
            return $"{sound.DelayAfterLeader} {sound.Name}";
        }
        return sound.Name!;
    }
}
