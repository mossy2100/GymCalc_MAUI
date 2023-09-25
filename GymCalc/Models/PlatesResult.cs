using GymCalc.Drawables;

namespace GymCalc.Models;

public class PlatesResult
{
    public double Percent { get; set; }

    // public double MaxWeight { get; set; }

    // public double StartingWeight { get; set; }

    // public List<Plate> Plates { get; set; }

    public string PlatesEachSideText { get; set; }

    public double IdealTotal { get; set; }

    public double IdealPlates { get; set; }

    public double ClosestTotal { get; set; }

    public double ClosestPlates { get; set; }

    public List<PlateDrawable> PlateDrawables { get; set; }

    internal PlatesResult(double percent, double maxWeight, double startingWeight,
        string eachSideText, IEnumerable<Plate> plates, List<PlateDrawable> drawables)
    {
        Percent = percent;
        // MaxWeight = maxWeight;
        // StartingWeight = startingWeight;
        // Plates = plates;
        PlatesEachSideText = eachSideText;
        IdealTotal = percent / 100.0 * maxWeight;
        IdealPlates = (IdealTotal - startingWeight) / 2.0;
        ClosestPlates = plates.Sum(p => p.Weight);
        ClosestTotal = startingWeight + 2.0 * ClosestPlates;
        PlateDrawables = drawables;
    }
}
