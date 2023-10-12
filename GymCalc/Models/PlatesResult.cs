using GymCalc.Constants;
using GymCalc.Drawables;

namespace GymCalc.Models;

public class PlatesResult
{
    public double Percent { get; set; }

    public string EachSideText { get; set; }

    public double IdealTotalWeight { get; set; }

    public double IdealPlatesWeight { get; set; }

    public double ClosestTotalWeight { get; set; }

    public double ClosestPlatesWeight { get; set; }

    // public List<PlateDrawable> PlateDrawables { get; set; }

    public PlatesDrawable Drawable { get; set; }

    internal PlatesResult(double percent, double maxTotalWeight, double totalStartingWeight,
        int nStacks, string eachSideText, IEnumerable<Plate> closestPlates,
        PlatesDrawable drawable)
    {
        Percent = percent;
        EachSideText = eachSideText;
        IdealTotalWeight = percent / 100.0 * maxTotalWeight;
        IdealPlatesWeight = (IdealTotalWeight - totalStartingWeight) / nStacks;
        ClosestPlatesWeight = closestPlates.Sum(p => p.Weight);
        ClosestTotalWeight = ClosestPlatesWeight * nStacks + totalStartingWeight;
        Drawable = drawable;
    }
}
