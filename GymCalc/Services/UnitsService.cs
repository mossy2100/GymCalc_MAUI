using Galaxon.Core.Types;
using GymCalc.Enums;

namespace GymCalc.Services;

internal static class UnitsService
{
    /// <summary>
    /// The number of kilograms in a pound.
    /// This is the exact value.
    /// <see href="https://en.wikipedia.org/wiki/Pound_(mass)#Current_use"/>
    /// </summary>
    public const decimal KG_PER_LB = 0.45359237m;

    /// <summary>
    /// The number of pounds in 1 kilogram, which is about 2.204623 lb/kg.
    /// </summary>
    public const decimal LB_PER_KG = 1 / KG_PER_LB;

    /// <summary>
    /// Get the user's preferred units of mass. Defaults to pounds in the US, kilograms elsewhere.
    /// </summary>
    /// <returns>The user's preferred units of mass.</returns>
    internal static EUnits GetDefaultUnits()
    {
        // Check if the units are already set in the user's preferences.
        string sUnits = Preferences.Default.Get("Units", "");

        // If so, try to convert the value from a string to an EUnits value.
        if (!string.IsNullOrEmpty(sUnits))
        {
            if (XEnum.TryParse(sUnits, out EUnits units)
                && units is EUnits.Kilograms or EUnits.Pounds)
            {
                return units;
            }
        }

        // The units are not specified in their preferences, so, check if they are from the US (or,
        // at least, if their phone is set up for US), and if so, default to pounds.
        // For everyone else, default to kilograms.
        return GeoService.IsUserFromUnitedStates() ? EUnits.Pounds : EUnits.Kilograms;
    }

    internal static string GetDefaultUnitsSymbol()
    {
        return GetDefaultUnits().GetDescription();
    }
}
