namespace GymCalc.Services;

public static class GeoService
{
    /// <summary>
    /// Get the user's locale (a.k.a. "culture" in .NET parlance) as a language code string.
    /// </summary>
    /// <returns>The locale as a language code string.</returns>
    public static string GetLocale()
    {
        return CultureInfo.CurrentCulture.Name;
    }

    /// <summary>
    /// See if the user is from the United States.
    /// </summary>
    /// <returns>If they are.</returns>
    public static bool IsUserFromUnitedStates()
    {
        return GetLocale() == "en-US";
    }
}
