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

    public static void Log(SequenceMessage msg, DateTime startedAt, int? sinceStart = null)
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

        // text inside square brackets will be colored:
        var comment = msg.Name.IsNullOrEmpty() ? $"[{msg.Comment}]" : $"[{msg.Name}] {msg.Comment}";

        WriteColoredLine($"{now:H:mm:ss}:{now.Millisecond:000}, sinceStart: {sinceStart:0000}, schedule: {msg.Timestamp:0000}, {comment}", GetColor(msg));
    }

    private static ConsoleColor GetColor(SequenceMessage msg)
    {
        if (msg.Sound?.Name is null || msg.Sound.IsSilenced)
        {
            return ConsoleColor.DarkGray;
        }
        if (msg.Leads)
        {
            return ConsoleColor.Green;
        }

        return AssignedColor(msg.Sound.Name);
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

    public static void WriteColoredLine(string message, ConsoleColor color)
    {
        WriteColored(message, color);
        Console.WriteLine();
    }
}
