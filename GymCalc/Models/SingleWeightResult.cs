using GymCalc.Drawables;

namespace GymCalc.Models;

public class SingleWeightResult
{
    public double Percent { get; set; }

    // public double MaxWeight { get; set; }

    public GymObject GymObject { get; set; }

    public double Ideal { get; set; }

    // public double Closest { get; set; }

    public GymObjectDrawable Drawable { get; set; }

    internal SingleWeightResult(double percent, double idealWeight, GymObject gymObject,
        GymObjectDrawable drawable)
    {
        Percent = percent;
        // MaxWeight = maxWeight;
        GymObject = gymObject;
        Ideal = idealWeight; //percent / 100.0 * maxWeight;
        // Closest = gymObject.Weight;
        Drawable = drawable;
    }
}
