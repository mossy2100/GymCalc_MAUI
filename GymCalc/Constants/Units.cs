namespace GymCalc.Constants;

internal static class Units
{
    internal const string DEFAULT = "default";

    internal const string ALL = "all";

    internal const string KILOGRAMS = "kg";

    internal const string POUNDS = "lb";

    /// <summary>
    /// This is the exact value.
    /// <see href="https://en.wikipedia.org/wiki/Pound_(mass)#Current_use" />
    /// </summary>
    internal const double KG_PER_LB = 0.45359237;

    /// <summary>
    /// About 2.204623 lb/kg.
    /// This will not be the exact value because of the limitations of the double type.
    /// </summary>
    internal const double LB_PER_KG = 1 / KG_PER_LB;

    internal static string GetPreferred()
    {
        return Preferences.Default.Get("Units", KILOGRAMS);
    }

    /// <summary>
    /// Check if a units string is valid.
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static bool IsValid(string units)
    {
        var validUnitsOptions = new string[] { DEFAULT, ALL, KILOGRAMS, POUNDS };
        return validUnitsOptions.Contains(units);
    }
}
