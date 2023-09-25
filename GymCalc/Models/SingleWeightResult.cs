using GymCalc.Drawables;

namespace GymCalc.Models;

public class SingleWeightResult
{
    public double Percent { get; set; }

    public GymObject GymObject { get; set; }

    public double Ideal { get; set; }

    public GymObjectDrawable Drawable { get; set; }

    internal SingleWeightResult(double percent, double ideal, GymObject gymObject,
        GymObjectDrawable drawable)
    {
        Percent = percent;
        Ideal = ideal;
        GymObject = gymObject;
        Drawable = drawable;
    }
}
