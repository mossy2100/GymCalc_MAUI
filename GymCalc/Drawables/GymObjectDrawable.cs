using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : IDrawable
{
    public const int MIN_WIDTH = 50;

    public const int MAX_WIDTH = 200;

    public GymObject? GymObject { get; set; }

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
        return CalculateWidth(GymObject!.Weight, MaxWeight);
    }

    /// <summary>
    /// Construct a new drawable for a given GymObject.
    /// </summary>
    /// <param name="gymObject">A gym object.</param>
    /// <returns>A GymObjectDrawable corresponding to the provided GymObject.</returns>
    /// <exception cref="Exception"></exception>
    public static GymObjectDrawable Create(GymObject gymObject)
    {
        // Get the type.
        var gymObjectTypeName = gymObject.GetType().Name;
        var drawableTypeName = $"GymCalc.Drawables.{gymObjectTypeName}Drawable";
        var drawableType = Type.GetType(drawableTypeName);
        if (drawableType == null)
        {
            throw new InvalidOperationException(
                $"Could not create a drawable for the {gymObjectTypeName} object.");
        }

        // Create the drawable object.
        var drawable = Activator.CreateInstance(drawableType);
        if (drawable == null)
        {
            throw new InvalidOperationException(
                $"Could not create a drawable for the {gymObjectTypeName} object.");
        }
        var gymObjectDrawable = (GymObjectDrawable)drawable;
        gymObjectDrawable.GymObject = gymObject;
        return gymObjectDrawable;
    }
}
