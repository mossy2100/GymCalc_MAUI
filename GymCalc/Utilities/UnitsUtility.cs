using Galaxon.Core.Enums;
using GymCalc.Constants;

namespace GymCalc.Utilities;

public static class UnitsUtility
{
    /// <summary>
    /// This is the exact value.
    /// <see href="https://en.wikipedia.org/wiki/Pound_(mass)#Current_use" />
    /// </summary>
    public const double KG_PER_LB = 0.45359237;

    /// <summary>
    /// About 2.204623 lb/kg.
    /// </summary>
    public const double LB_PER_KG = 1 / KG_PER_LB;

    internal static Units GetDefault()
    {
        var sUnits = Preferences.Default.Get("Units", Units.Kilograms.GetDescription());
        return sUnits == Units.Pounds.GetDescription() ? Units.Pounds : Units.Kilograms;
    }
}
