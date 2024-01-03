using Galaxon.Core.Strings;

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

    /// <summary>
    /// Get the correct spelling of a word for the user's locale.
    /// Add more words as needed. May also need flags for isFromCanada, isFromAustralia, etc.
    /// Should accept the source word in either US or UK spelling.
    /// </summary>
    public static string GetSpelling(string word)
    {
        // Check if the user is using US English, and assume UK English otherwise (I realise this
        // isn't correct, as Canadian, Australian, etc. spellings can also vary from UK, but it
        // will do for now).
        bool isFromUS = IsUserFromUnitedStates();

        // Get the source word's string case.
        EStringCase wordCase = word.GetCase();

        // Get the locale-specific spelling in lower-case.
        string result;
        switch (word.ToLower())
        {
            case "color":
            case "colour":
                result = isFromUS ? "color" : "colour";
                break;

            default:
                // Word not found, so just give it back unaltered.
                return word;
        }

        // Apply the original string case as needed.
        return result.SetCase(wordCase);
    }
}
