using Galaxon.Maui.Utilities;

namespace GymCalc.Graphics;

/// <summary>
/// General purpose layout stuff.
/// </summary>
internal static class PageLayout
{
    internal const double Spacing = 8;

    internal const double HalfSpacing = Spacing / 2;

    internal const double DoubleSpacing = 2 * Spacing;

    internal const double TripleSpacing = 3 * Spacing;

    /// <summary>
    /// Get the number of columns to use for laying out a page.
    /// If portrait, 1. If landscape, 2.
    /// TODO If I add support for tablets, this result will be larger.
    /// Another approach would be to set the number of layout columns based on the width,
    /// e.g. nCols = Floor(deviceWidth / 300)
    /// </summary>
    internal static int GetNumColumns()
    {
        return MauiUtility.GetOrientation() == DisplayOrientation.Portrait ? 1 : 2;
    }
}
