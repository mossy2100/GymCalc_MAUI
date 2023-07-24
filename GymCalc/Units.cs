namespace GymCalc;

internal static class Units
{
    internal const string Kilograms = "kg";

    internal const string Pounds = "lb";

    internal const double PoundsPerKilogram = 2.2;

    internal static string GetUnits()
    {
        return Preferences.Default.Get("Units", Kilograms);
    }
}
