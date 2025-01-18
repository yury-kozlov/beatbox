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

    public static void Log(SequenceMessage msg)
    {
        var now = DateTime.Now;

        // text inside square brackets with be colored:
        var comment = msg.Name.IsNullOrEmpty() ? $"[{msg.Comment}]" : $"[{msg.Name}] {msg.Comment}";
        
        WriteColored($"{now:H:mm:ss}:{now.Millisecond:000} {msg.Timestamp:0000} {comment}", GetColor(msg));
    }

    private static ConsoleColor GetColor(SequenceMessage msg)
    {
        if (msg.Sound is null || msg.Sound.IsSilenced)
        {
            return ConsoleColor.DarkGray;
        }
        if (msg.Leads)
        {
            return ConsoleColor.Green;
        }
        if (_assignedColors.TryGetValue(msg.Sound.Name, out var color))
        {
            return color;
        }
        if (_availableColors.TryPop(out color))
        {
            _assignedColors[msg.Sound.Name] = color;
        };
        return color;
    }

    private static void WriteColored(string message, ConsoleColor color)
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
        Console.WriteLine();
    }
}
