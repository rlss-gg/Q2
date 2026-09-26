namespace Q2.App.Common;

public static class Elo
{
    public const double KFactor = 20.0;

    public const double MaxDifference = 400.0;

    public static double Difference(double a, double b)
    {
        return Math.Clamp(a - b, -MaxDifference, MaxDifference);
    }

    public static double Expected(double a, double b)
    {
        return 1.0 / (1.0 + Math.Pow(10.0, Difference(b, a) / MaxDifference));
    }

    public static (double A, double B) CalculateChange(double a, double b, bool aWon)
    {
        var expected = Expected(a, b);
        var change = KFactor * ((aWon ? 1.0 : 0.0) - expected);

        return (change, (change * -1.0));
    }
}
