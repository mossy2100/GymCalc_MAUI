namespace GymCalc.Graphics;

internal static class Palette
{
    /// <summary>
    /// Lookup table to map colors to names.
    /// </summary>
    public static readonly Dictionary<string, string> Colors = new ()
    {
        { "Red", "#cc0022" },
        { "Orange", "#ff6c00" },
        { "Gold", "#d9a300" },
        { "Yellow", "#f2f224" },
        { "Lime", "#ace500" },
        { "Green", "#0fb300" },
        { "Cyan", "#80eaff" },
        { "Blue", "#0080ff" },
        { "Indigo", "#0033cc" },
        { "Violet", "#7e5ce5" },
        { "Purple", "#9e19a7" },
        { "Pink", "#ff4cff" },
        { "White", "#eee" },
        { "Silver", "#c0c0c0" },
        { "Gray", "#808080" },
        { "Black", "#333" }
    };

    /// <summary>
    /// Get a color object given a color name.
    /// </summary>
    /// <param name="name">The color name.</param>
    /// <returns>The corresponding Color object or null if not found.</returns>
    internal static Color? Get(string? name)
    {
        return (name != null && Colors.TryGetValue(name, out string? hex))
            ? Color.Parse(hex)
            : null;
    }
}
