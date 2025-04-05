using System.Text;

namespace Beater;

public class KeyPressed
{
    public ConsoleKey Key;
    public DateTime Time;
    public int PostDelay;
    override public string ToString() => $"{Key} {PostDelay}";
}

public class ConsoleKeyPlayer
{
    public static readonly ConsoleKey EndOfSequence = ConsoleKey.Enter;
    public Action<KeyPressed>? OnKeyPressed;
    public List<KeyPressed> PressedKeys = new();
    public int TotalTime;
    public bool IsEmpty => PressedKeys.Count == 0 || PressedKeys[0].Key == EndOfSequence;
    public string? KeysString;

    private readonly TcpTransport _transport;
    private static Dictionary<ConsoleKey, TransportMessage> _soundsMap = new()
    {
        { ConsoleKey.K, Samples.B1 },
        { ConsoleKey.S, Samples.B2 },
        { EndOfSequence, new() }
    };
    private static TransportMessage _defaultSound = Samples.B1;
    private string? _jsonFilePath;

    public ConsoleKeyPlayer()
    {
        _transport = new TcpTransport();
        OnKeyPressed += (e) =>
        {
            if (e.Key == EndOfSequence)
            {
                return;
            }
            var audioMessage = GetSound(e.Key);
            _transport.Send(audioMessage);
        };
    }

    public ConsoleKeyPlayer Listen()
    {
        Console.WriteLine("Press ENTER to stop listening...");
        KeyPressed? previousKey = null;
        while (true)
        {
            var keyInfo = Console.ReadKey();
            var now = DateTime.Now;
            if (previousKey is not null)
            {
                previousKey.PostDelay = Round(now - previousKey.Time, 100);
                TotalTime += previousKey.PostDelay;
            }
            var keyPressed = new KeyPressed { Key = keyInfo.Key, Time = now };
            PressedKeys.Add(previousKey = keyPressed);
            OnKeyPressed?.Invoke(keyPressed);

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
            var color = Logger.AssignedColor(GetSound(k.Key).SoundName!);
            Logger.WriteColored($"[{keyName}]", color);
            str.Append(keyName);

            var spacesCount = (int)(k.PostDelay / 100.0);
            for (var i = 0; i < spacesCount; i++)
            {
                Console.Write(" ");
                str.Append(' ');
            }
        }
    }

    public Sequence GenerateSequence()
    {
        var iterationsCount = 4;

        // this will be the main loop (acting like a metronome, without any sound):
        var loop = new Sound("") { Strategy = new RepeatStrategy { Count = iterationsCount, Interval = TotalTime } };
        var seq = new Sequence { Leader = loop };

        KeyPressed? previousKey = null;
        Sound? previousSound = null;

        foreach (var k in PressedKeys)
        {
            if (k.Key == EndOfSequence)
            {
                break;
            }
            var soundName = GetSound(k.Key).SoundName!;
            var delayAfterLeader = previousKey?.PostDelay ?? 0;
            var sound = new Sound(soundName) { Strategy = new FollowPreviousSoundStrategy { DelayAfterLeader = delayAfterLeader } };
            loop.Followers.Add(sound);
            previousSound = sound;
            previousKey = k;
        }

        return seq;
    }

    public async Task PlayRepeated(Sequence? seq = null)
    {
        seq ??= GenerateSequence();
        await _transport.PlayRepeated(seq.Generate());
    }

    public async Task PlayRepeated(string sequenceFilePath)
    {
        var json = File.ReadAllText(sequenceFilePath);
        var seq = Sequence.FromJson(json);

        await PlayRepeated(seq);
    }

    private TransportMessage GetSound(ConsoleKey key)
    {
        return _soundsMap.TryGetValue(key, out var msg) ? msg : _defaultSound;
    }

    private static int Round(TimeSpan value, double precision)
    {
        return (int)Math.Round(value.TotalMilliseconds / precision) * (int)precision;
    }

    internal void SaveSequence()
    {
        if (!IsEmpty)
        {
            var seq = GenerateSequence();
            var json = seq.ToJson();
            _jsonFilePath = $"C:/music/samples/sequences/seq{DateTime.Now:yyyy-MM-dd-HHmm}.json";
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
