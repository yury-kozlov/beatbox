using System.Text.RegularExpressions;

namespace Beater;

public class LogPosition
{
    /// <summary>
    /// Console buffer row (line number) the sound was logged to.
    /// NOTE: this is a buffer coordinate, not a window/viewport one - it's not bounded by WindowHeight.
    /// </summary>
    public int Row;

    /// <summary>
    /// Console column within that row where MarkLine should draw its indicator.
    /// </summary>
    public int Column;
}

public class Logger
{
    /// <summary>
    /// Guards Console cursor/color state: Log runs on the send loop while MarkLine can fire concurrently
    /// from the listener thread as acks arrive - without this lock their writes/cursor moves could interleave.
    /// </summary>
    private static readonly object _consoleLock = new();
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

    /// <summary>
    /// Logs the sound and returns the column of the last space in the "H:mm:ss:fff, seq: " prefix -
    /// the slot where MarkLine should insert its indicator once the sound finishes playing.
    /// </summary>
    public static LogPosition Log(GeneratedSound sound, DateTime startedAt, int? sinceStart = null)
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
        if (sound.SoundDesign is Metronome or LoopEnd or SequenceStart or SequenceEnd)
        {
            name = sound.SoundDesign.FriendlyName;
        }

        var tags = sound.SoundDesign.Tags?.Count > 0 ? " " + sound.SoundDesign.Tags.Join() : "";

        // text inside square brackets will be colored:
        var comment = name.IsNullOrEmpty() ? $"[{sound.Comment}]" : $"[{name}] {sound.Comment}{tags}";
        var sequenceName = sound.SoundDesign.Sequence?.Name ?? "";

        var prefix = $"{now:H:mm:ss}:{now.Millisecond:000}, seq: ";
        lock (_consoleLock)
        {
            WriteColored($"{prefix}[{sequenceName,5}] ", AssignedColor(sequenceName));

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

        return new LogPosition
        {
            Row = Console.CursorTop - 1,
            Column = prefix.Length - 1, // the space right before the "[" that opens the sequence name
        };
    }

    /// <summary>
    /// Recolors/marks a specific column of an already-printed line to indicate the sound finished playing,
    /// without printing a new line. No-op if the line has scrolled out of the console buffer.
    /// </summary>
    public static void MarkLine(int row, int column, char marker, ConsoleColor color)
    {
        lock (_consoleLock)
        {
            if (row < 0 || row >= Console.BufferHeight)
            {
                return;
            }

            var left = Console.CursorLeft;
            var top = Console.CursorTop;

            Console.SetCursorPosition(column, row);
            Console.ForegroundColor = color;
            Console.Write(marker);
            Console.ResetColor();
            Console.SetCursorPosition(left, top);
        }
    }

    private static ConsoleColor GetColor(GeneratedSound sound)
    {
        if (sound.Name is null || sound.IsSilenced)
        {
            return ConsoleColor.DarkGray;
        }
        if (sound.SoundDesign.IsLeader())
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
