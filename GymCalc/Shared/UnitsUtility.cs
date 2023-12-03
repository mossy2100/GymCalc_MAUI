using System.Globalization;
using Galaxon.Core.Types;
using GymCalc.Constants;

namespace GymCalc.Shared;

public static class UnitsUtility
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
    internal static Units GetDefault()
    {
        string sUnits = Preferences.Default.Get("Units", "");
        if (sUnits == Units.Pounds.GetDescription())
        {
            return Units.Pounds;
        }
        if (sUnits == Units.Kilograms.GetDescription())
        {
            return Units.Kilograms;
        }
        // See if they are from the US; or, at least, if their phone is set up for US.
        return CultureInfo.CurrentCulture.Name == "en-US" ? Units.Pounds : Units.Kilograms;
    }
}
