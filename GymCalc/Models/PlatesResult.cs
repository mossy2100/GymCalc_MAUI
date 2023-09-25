using GymCalc.Drawables;

namespace GymCalc.Models;

public class PlatesResult
{
    public double Percent { get; set; }

    public string EachSideText { get; set; }

    public double IdealTotal { get; set; }

    public double IdealPlates { get; set; }

    public double ClosestTotal { get; set; }

    public double ClosestPlates { get; set; }

    public List<PlateDrawable> PlateDrawables { get; set; }

    internal PlatesResult(double percent, double maxWeight, double startingWeight,
        string eachSideText, IEnumerable<Plate> plates, List<PlateDrawable> drawables)
    {
        Percent = percent;
        EachSideText = eachSideText;
        IdealTotal = percent / 100.0 * maxWeight;
        IdealPlates = (IdealTotal - startingWeight) / 2.0;
        ClosestPlates = plates.Sum(p => p.Weight);
        ClosestTotal = startingWeight + 2.0 * ClosestPlates;
        PlateDrawables = drawables;
    }
}
