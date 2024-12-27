using System.Net.Sockets;

namespace Beater;

/// <summary>
/// based on https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient
/// </summary>
internal class BeatBox : IDisposable
{
    private TcpClient? _client;
    private NetworkStream? _channel;

    public BeatBox Init()
    {
        _client = new TcpClient("localhost", 3000);
        _channel = _client.GetStream();

        GC.Collect();
        return this;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task Run()
    {
        Console.WriteLine("experiment 3");
        var sequence = new Sequence
        {
            Leader = new Sound("b1")
            {
                Strategy = new RepeatStrategy { Every = 16, Interval = 500 },
                Followers = new() {
                  new Sound("ts1") {
                      Strategy = new RepeatStrategy { DelayAfterLeader = 150, Every = 2, Interval = 80 },
                  },
                new Sound("ts2") { Strategy = new PlayOnceStrategy { PlayEveryX = 4 } },
                // new Sound("ts3") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 0, PlayEveryX = 2 } },
                new Sound("b2") { Strategy = new PlayOnceStrategy { DelayAfterLeader = 250, PlayEveryX = 4 } },
               },
            },
        };

        var sequenceMessages = sequence.Generate();
        await _channel.Play(sequenceMessages);

        Console.WriteLine("experiment 1");
        for (int i = 0; i < 10; i++)
        {
            _channel!.Write(SoundMessage.Ts1, 0, SoundMessage.Ts1.Length); // second file

            await Repeat(count: 3, waitMs: 500);

            await RepeatLinear(count: 5, initialDelayMs: 80, incr: -10);

            await Task.Delay(130);

            Console.WriteLine("");
        }

        Console.WriteLine();
    }

    private async Task Repeat(int count, int waitMs)
    {
        for (int i = 0; i < count; i++)
        {
            Console.Write(i);
            _channel!.Write(SoundMessage.Beat1, 0, SoundMessage.Beat1.Length);
            _channel!.Write(SoundMessage.Ts1, 0, SoundMessage.Ts1.Length);
            await Task.Delay(waitMs);
        }
    }

    //private async Task RepeatLinearFactor(int delayMs, double factor)
    //{
    //    while (delayMs > 0)
    //    {
    //        _stream!.Write(_message, 0, _message.Length);
    //        await Task.Delay(delayMs);

    //    }
    //        delayMs += incr;
    //        if (delayMs <= 0)
    //        {
    //            delayMs = 20; // min delay
    //        }
    //}

    private async Task RepeatLinear(int count, int initialDelayMs, int incr)
    {
        var delay = initialDelayMs;
        for (int i = 0; i < count; i++)
        {
            Console.Write(".");
            _channel!.Write(SoundMessage.Beat1, 0, SoundMessage.Beat1.Length);
            await Task.Delay(delay);
            delay += incr;
            if (delay <= 0)
            {
                delay = 20; // min delay
            }
        }
    }
}
