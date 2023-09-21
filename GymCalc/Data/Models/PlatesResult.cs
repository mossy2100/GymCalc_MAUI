using GymCalc.Graphics.Drawables;

namespace GymCalc.Data.Models;

public class PlatesResult
{
    public double Percent { get; set; }

    public double MaxWeight { get; set; }

    public double BarWeight { get; set; }

    public List<Plate> Plates { get; set; }

    public string PlatesEachSideText { get; set; }

    public double IdealTotal { get; set; }

    public double IdealPlates { get; set; }

    public double ClosestTotal { get; set; }

    public double ClosestPlates { get; set; }

    public List<PlateDrawable> PlateDrawables { get; set; }

    public PlatesResult(double percent, double maxWeight, double barWeight, string eachSideText,
        List<Plate> plates, List<PlateDrawable> drawables)
    {
        Percent = percent;
        MaxWeight = maxWeight;
        BarWeight = barWeight;
        Plates = plates;
        PlatesEachSideText = eachSideText;
        IdealTotal = percent / 100.0 * maxWeight;
        IdealPlates = (IdealTotal - barWeight) / 2.0;
        ClosestPlates = plates.Sum(p => p.Weight);
        ClosestTotal = barWeight + 2.0 * ClosestPlates;
        PlateDrawables = drawables;
    }
}
