using System.Text;

namespace Beater;

public class KeyPressed
{
    public ConsoleKey Key;
    public DateTime Time;
    public int PostDelay;
    override public string ToString() => $"{Key} {PostDelay}";
}

public class ConsoleKeyPlayer : IDisposable
{
    private static readonly ConsoleKey EndOfSequence = ConsoleKey.Enter;
    private static readonly ConsoleKey ResetKey = ConsoleKey.Delete;
    private static readonly List<ConsoleKey> IgnoredKeys = new() {
        ConsoleKey.VolumeDown,
        ConsoleKey.VolumeUp,
        ConsoleKey.VolumeMute,
    };

    private Action<KeyPressed>? OnKeyPressed;
    private List<KeyPressed> PressedKeys = new();
    private int TotalTime;
    public bool IsEmpty => PressedKeys.Count == 0 || PressedKeys[0].Key == EndOfSequence;
    public string? KeysString;

    private readonly TcpTransport _transport;
    private static Dictionary<ConsoleKey, TransportMessage> _transportMessageMap = new()
    {
        { ConsoleKey.K, Samples.K },
        { ConsoleKey.S, Samples.S },
        { ConsoleKey.D1, Samples.TS1 },
        { ConsoleKey.D2, Samples.TS2 },
        { ConsoleKey.D3, Samples.TS3 },
        { ConsoleKey.D4, Samples.TS4 },
        { ConsoleKey.D5, Samples.TS5 },
        { ConsoleKey.D6, Samples.TS6 },
        { ConsoleKey.D7, Samples.TS7 },
        { ConsoleKey.D8, Samples.TS8 },
        { ConsoleKey.D9, Samples.TS9 },
        { EndOfSequence, new() }
    };

    private static TransportMessage _defaultSound = Samples.K;
    private string? _jsonFilePath;

    public ConsoleKeyPlayer()
    {
        _transport = new TcpTransport();
        OnKeyPressed += (e) =>
        {
            if (e.Key == EndOfSequence || e.Key == ResetKey)
            {
                return;
            }
            var soundName = GetSoundName(e.Key);
            _transport.Send(TransportMessage.ToPlayMessage(soundName));
        };
        Console.CancelKeyPress += OnShutdown;
    }

    private void OnShutdown(object? sender, ConsoleCancelEventArgs e)
    {
        Dispose();
        Console.WriteLine("Shutting down..");
        Environment.Exit(0);
    }

    public void Dispose() => _transport?.Dispose();

    public ConsoleKeyPlayer Listen()
    {
        Console.WriteLine("Press ENTER to stop listening...");
        Console.WriteLine("Press DEL to reset and start over...");

        KeyPressed? previousKey = null;
        while (true)
        {
            var keyInfo = Console.ReadKey();
            if (IgnoredKeys.Contains(keyInfo.Key))
            {
                continue;
            }
            var now = DateTime.Now;
            if (previousKey is not null)
            {
                previousKey.PostDelay = Round(now - previousKey.Time, 100);
                TotalTime += previousKey.PostDelay;
            }
            var keyPressed = new KeyPressed { Key = keyInfo.Key, Time = now };
            PressedKeys.Add(previousKey = keyPressed);
            OnKeyPressed?.Invoke(keyPressed);

            if (keyInfo.Key == ResetKey)
            {
                Console.WriteLine("\r\nResetting...");
                PressedKeys.Clear();
                TotalTime = 0;
                previousKey = null;
                continue;
            }

            if (keyInfo.Key == EndOfSequence)
            {
                break;
            }
        }
        Console.WriteLine();
        return this;
    }

    public void PrintFormatted()
    {
        var str = new StringBuilder();
        foreach (var k in PressedKeys)
        {
            if (k.Key == EndOfSequence)
            {
                Console.WriteLine(TotalTime);
                KeysString = str.ToString();
                return;
            }

            var keyName = k.Key.ToString();
            var color = Logger.AssignedColor(GetSoundName(k.Key));
            Logger.WriteColored($"[{keyName}]", color);
            str.Append(keyName);

            var spacesCount = GetSpacesCount(k.PostDelay);
            for (var i = 0; i < spacesCount; i++)
            {
                Console.Write(" ");
                str.Append(' ');
            }
        }
    }

    public static string GetFormattedDelays(params int[] delays)
    {
        var str = new StringBuilder();
        var totalTime = delays.Sum();
        var scaleFactor = totalTime < 3000 ? 3000.0 / totalTime : 1; // scale to 3000ms total (line of 30 chars)
        foreach (var delay in delays)
        {
            var spacesCount = GetSpacesCount((int)(delay * scaleFactor));
            for (var i = 0; i < spacesCount; i++)
            {
                str.Append(' ');
            }
            str.Append("X");
        }
        return str.ToString();
    }

    private static int GetSpacesCount(int postDelay) => (int)(postDelay / 100.0);

    /// <summary>
    /// Generates sequence from pressed keys interactively.
    /// </summary>
    public SequenceDesign GenerateInteractively()
    {
        var seq = new SequenceDesign(SequenceCodeGenerator.NewSequenceName())
        {
            Strategy = new RepeatStrategy { Count = 4, Interval = TotalTime }
        };

        KeyPressed? previousKey = null;
        SoundDesign? leader = null;

        foreach (var k in PressedKeys)
        {
            if (k.Key == EndOfSequence)
            {
                break;
            }

            var sound = GetSound(k.Key);
            if (leader is null)
            {
                leader = sound; // first sound
            }
            else
            {
                sound.Strategy = new FollowPreviousSoundStrategy { DelayAfterLeader = previousKey?.PostDelay ?? 0 };
                leader.Followers.Add(sound);
            }

            previousKey = k;
        }

        if (leader is not null)
        {
            seq.Leader = leader;
        }

        return seq;
    }

    public async Task PlayRepeated(SequenceDesign? design = null)
    {
        design ??= GenerateInteractively();
        var sequence = SequenceGenerator.Generate(design);
        await _transport.PlayRepeated(sequence);
    }

    public async Task PlayRepeated(string sequenceFilePath)
    {
        var json = File.ReadAllText(sequenceFilePath);
        var seq = SequenceDesign.FromJson(json);

        await PlayRepeated(seq);
    }

    private string GetSoundName(ConsoleKey key)
    {
        var transportMessage = _transportMessageMap.TryGetValue(key, out var msg) ? msg : _defaultSound;
        return transportMessage.SoundName!;
    }

    private SoundDesign GetSound(ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.K => new Kick(),
            ConsoleKey.S => new Snare(),
            _ => new SoundDesign(GetSoundName(key)),
        };
    }

    private static int Round(TimeSpan value, double precision)
    {
        return (int)Math.Round(value.TotalMilliseconds / precision) * (int)precision;
    }

    internal void SaveSequence()
    {
        if (!IsEmpty)
        {
            var seq = GenerateInteractively();
            var json = seq.ToJson();
            _jsonFilePath = $"C:/music/samples/sequences/{seq.Name}.json";
            File.WriteAllText(_jsonFilePath, json);
        }
    }

    internal void GenerateCodeFrom(string sequenceFilePath)
    {
        SequenceCodeGenerator.GenerateCodeFromSequenceJson(sequenceFilePath, KeysString);
    }

    internal void SaveCode()
    {
        if (_jsonFilePath is null)
        {
            SaveSequence();
        }
        if (File.Exists(_jsonFilePath))
        {
            GenerateCodeFrom(_jsonFilePath);
        }
    }
}
