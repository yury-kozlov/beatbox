namespace Beater;

public class SimpleFraction
{
    public int Numerator;
    public int Denominator;
}

public class Numbers
{
    private static Dictionary<double, SimpleFraction> _map = new(){
        { 0.25, new SimpleFraction{ Numerator = 1, Denominator = 4 } },
        { 0.5, new SimpleFraction{ Numerator = 1, Denominator = 2 } },
        { 0.75, new SimpleFraction{ Numerator = 3, Denominator = 4 } },
    };

    public static bool IsInteger(double number)
    {
        return number % 1 == 0;
    }

    public static SimpleFraction GetFraction(double number)
    {
        if (_map.TryGetValue(number, out var fraction))
        {
            return fraction;
        }
        throw new ApplicationException($"Simple fraction for {number} is not mapped");
    }
}
