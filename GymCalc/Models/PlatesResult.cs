using GymCalc.Drawables;

namespace GymCalc.Models;

public class PlatesResult
{
    internal PlatesResult(decimal percent, decimal maxTotalWeight, decimal totalStartingWeight,
        int nStacks, string eachSideText, IEnumerable<Plate> closestPlates,
        PlatesDrawable drawable)
    {
        Percent = percent;
        EachSideText = eachSideText;
        IdealTotalWeight = percent / 100m * maxTotalWeight;
        IdealPlatesWeight = (IdealTotalWeight - totalStartingWeight) / nStacks;
        ClosestPlatesWeight = closestPlates.Sum(p => p.Weight);
        ClosestTotalWeight = ClosestPlatesWeight * nStacks + totalStartingWeight;
        Drawable = drawable;
    }

    public decimal Percent { get; set; }

    public string EachSideText { get; set; }

    public decimal IdealTotalWeight { get; set; }

    public decimal IdealPlatesWeight { get; set; }

    public decimal ClosestTotalWeight { get; set; }

    public decimal ClosestPlatesWeight { get; set; }

    public PlatesDrawable Drawable { get; set; }
}
