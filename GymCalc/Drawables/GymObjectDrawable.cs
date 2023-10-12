using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : IDrawable
{
    public GymObject GymObject { get; set; }

    public double Width => GetWidth();

    public double Height => GetHeight();

    public const double MIN_WIDTH = 50;

    public const double MAX_WIDTH = 200;

    internal double MaxWeight { get; set; }

    public abstract double GetWidth();

    public abstract double GetHeight();

    /// <inheritdoc />
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// </summary>
    public static double CalculateWidth(double weight, double maxWeight)
    {
        return MIN_WIDTH + (weight / maxWeight) * (MAX_WIDTH - MIN_WIDTH);
    }

    /// <summary>
    /// Construct a new drawable for a given GymObject.
    /// </summary>
    /// <param name="gymObject"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static GymObjectDrawable Create(GymObject gymObject)
    {
        var drawableTypeName = "GymCalc.Drawables." + gymObject.GetType().Name + "Drawable";
        var drawable = (GymObjectDrawable)Activator.CreateInstance(null, drawableTypeName)!
            .Unwrap();
        if (drawable == null)
        {
            throw new Exception("Could not create drawable.");
        }
        drawable.GymObject = gymObject;
        return drawable;
    }
}
