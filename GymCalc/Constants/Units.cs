namespace GymCalc.Constants;

internal static class Units
{
    internal const string Kilograms = "kg";

    internal const string Pounds = "lb";

    /// <summary>
    /// This is the exact value.
    /// <see href="https://en.wikipedia.org/wiki/Pound_(mass)#Current_use" />
    /// </summary>
    internal const double KilogramsPerPound = 0.45359237;

    /// <summary>
    /// About 2.204623 lb/kg.
    /// This will not be the exact value because of the limitations of the double type.
    /// </summary>
    internal const double PoundsPerKilogram = 1 / KilogramsPerPound;

    internal static string GetPreferred()
    {
        return Preferences.Default.Get("Units", Kilograms);
    }
}
