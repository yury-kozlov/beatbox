using System.Text.RegularExpressions;

namespace Beater;

public class Logger
{
    private static Dictionary<string, ConsoleColor> _assignedColors = new();
    private static Stack<ConsoleColor> _availableColors = new([
        ConsoleColor.Red,
        ConsoleColor.Yellow,
        ConsoleColor.Blue,
        ConsoleColor.Magenta,
        ConsoleColor.Cyan,
        ConsoleColor.DarkRed,
        ConsoleColor.DarkYellow,
        ConsoleColor.DarkBlue,
        ConsoleColor.DarkMagenta,
        ConsoleColor.DarkCyan,
    ]);

    public static void Log(Sound sound, DateTime startedAt, int? sinceStart = null)
    {
        DateTime now;
        if (sinceStart.HasValue)
        {
            // show expected time based on already calculated timestamp
            // (timestamp of each message is known beforehand when all messages are sent at once)
            now = startedAt.AddMilliseconds(sinceStart.Value);
        }
        else
        {
            // show actual time based on current time
            now = DateTime.Now;
            sinceStart = (int)(now - startedAt).TotalMilliseconds;
        }

        var name = sound.Name;
        if (sound is Metronome m)
        {
            name = m.FriendlyName;
        }
        else if (sound is LoopEnd e)
        {
            name = e.FriendlyName;
        }
        else if (sound is SequenceStart st)
        {
            name = st.FriendlyName;
        }

        var tags = sound.Tags?.Count > 0 ? " " + sound.Tags.Join() : "";

        // text inside square brackets will be colored:
        var comment = name.IsNullOrEmpty() ? $"[{sound.Comment}]" : $"[{name}] {sound.Comment}{tags}";
        var sequenceName = sound.Sequence?.Name ?? "";

        WriteColored($"{now:H:mm:ss}:{now.Millisecond:000}, seq: [{sequenceName,5}] ", AssignedColor(sequenceName));

        if (sinceStart.HasValue)
        {
            // don't show schedule because they are equal
            WriteColored($"timestamp: {sinceStart:0000}, {comment}", GetColor(sound));
        }
        else
        {
            WriteColored($"sinceStart: {sinceStart:0000}, schedule: {sound.Timestamp:0000}, {comment}", GetColor(sound));
        }

        Console.WriteLine();
    }

    private static ConsoleColor GetColor(Sound sound)
    {
        if (sound.Name is null || sound.IsSilenced)
        {
            return ConsoleColor.DarkGray;
        }
        if (sound.IsLeader())
        {
            return ConsoleColor.Green;
        }

        return AssignedColor(sound.Name);
    }

    public static ConsoleColor AssignedColor(string token)
    {
        if (_assignedColors.TryGetValue(token, out var color))
        {
            return color;
        }
        if (_availableColors.TryPop(out color))
        {
            _assignedColors[token] = color;
        }
        else
        {
            // we are out of colors, reset and start again:
            _availableColors = new(_assignedColors.Values);
        }
        return color;
    }

    /// <summary>
    // [text inside] square brackets will be colored.
    /// </summary>
    public static void WriteColored(string message, ConsoleColor color)
    {
        var pieces = Regex.Split(message, @"(\[[^\]]*\])");
        for (int i = 0; i < pieces.Length; i++)
        {
            string piece = pieces[i];

            if (piece.StartsWith("[") && piece.EndsWith("]"))
            {
                Console.ForegroundColor = color;
                piece = piece.Substring(1, piece.Length - 2);
            }

            Console.Write(piece);
            Console.ResetColor();
        }
    }
}
