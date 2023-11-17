using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : IDrawable
{
    public const decimal MIN_WIDTH = 50;

    public const decimal MAX_WIDTH = 200;

    public GymObject GymObject { get; set; }

    public decimal Width => GetWidth();

    public decimal Height => GetHeight();

    internal decimal MaxWeight { get; set; }

    /// <inheritdoc/>
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    public abstract decimal GetWidth();

    public abstract decimal GetHeight();

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// Static version.
    /// </summary>
    public static decimal CalculateWidth(decimal weight, decimal maxWeight)
    {
        return MIN_WIDTH + weight / maxWeight * (MAX_WIDTH - MIN_WIDTH);
    }

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// Instance version.
    /// </summary>
    public decimal CalculateWidth()
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
