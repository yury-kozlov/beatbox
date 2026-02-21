namespace Beater;

public class SimpleFraction
{
    public int Numerator;
    public int Denominator;
    public override string ToString() => $"{Numerator}/{Denominator}";
}

public class Numbers
{
    public static SimpleFraction GetFraction(string number)
    {
        var parts = number.Split('/');
        if (parts.Length == 2
            && int.TryParse(parts.First(), out var numerator)
            && int.TryParse(parts.Last(), out var denominator))
        {
            return new SimpleFraction { Numerator = numerator, Denominator = denominator };
        }

        throw new ApplicationException($"Simple fraction for {number} is not mapped");
    }

    public static bool IsXOutOf(string? xOutOfY, int x)
    {
        if (xOutOfY.IsNullOrEmpty())
        {
            return false;
        }

        // count calls within loop (for example, every 3rd time within repeated range of 4)
        var fraction = Numbers.GetFraction(xOutOfY);
        var playEveryX = fraction.Numerator;
        var range = fraction.Denominator;
        var iterationNumber = (x - 1) / range;
        var calledTimesInRange = x - (range * iterationNumber);

        return calledTimesInRange == playEveryX;
    }
}
