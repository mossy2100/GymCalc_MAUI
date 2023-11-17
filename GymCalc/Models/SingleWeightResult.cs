using GymCalc.Drawables;

namespace GymCalc.Models;

public class SingleWeightResult
{
    internal SingleWeightResult(decimal percent, decimal ideal, GymObject gymObject,
        GymObjectDrawable drawable)
    {
        Percent = percent;
        Ideal = ideal;
        GymObject = gymObject;
        Drawable = drawable;
    }

    public decimal Percent { get; set; }

    public GymObject GymObject { get; set; }

    public decimal Ideal { get; set; }

    public GymObjectDrawable Drawable { get; set; }
}
