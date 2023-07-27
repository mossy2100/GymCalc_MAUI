namespace GymCalc.Graphics;

/// <summary>
/// General purpose layout stuff.
/// </summary>
internal static class PageLayout
{
    internal const double Spacing = 10;

    internal const double DoubleSpacing = 2 * Spacing;

    /// <summary>
    /// Get the number of columns to use for laying out a page.
    /// If portrait, 1. If landscape, 2.
    /// </summary>
    internal static int GetNumColumns()
    {
        return DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait
            ? 1
            : 2;
    }
}
