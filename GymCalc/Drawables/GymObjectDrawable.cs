using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : IDrawable
{
    public const int MIN_WIDTH = 50;

    public const int MAX_WIDTH = 200;

    public GymObject GymObject { get; set; }

    public double Width => GetWidth();

    public double Height => GetHeight();

    internal decimal MaxWeight { get; set; }

    /// <inheritdoc/>
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    public abstract double GetWidth();

    public abstract double GetHeight();

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// Static version.
    /// </summary>
    public static double CalculateWidth(decimal weight, decimal maxWeight)
    {
        return MIN_WIDTH + (double)weight / (double)maxWeight * (MAX_WIDTH - MIN_WIDTH);
    }

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// Instance version.
    /// </summary>
    public double CalculateWidth()
    {
        return CalculateWidth(GymObject.Weight, MaxWeight);
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
