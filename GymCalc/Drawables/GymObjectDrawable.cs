using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : BaseDrawable
{
    /// <summary>
    /// Constant for the minimum width of a gym object graphic.
    /// </summary>
    private const int _MIN_WIDTH = 50;

    /// <summary>
    /// Constant for the maximum width of a gym object graphic.
    /// </summary>
    private const int _MAX_WIDTH = 200;

    /// <summary>
    /// Reference to the gym object this drawable represents.
    /// </summary>
    protected GymObject? GymObject { get; private set; }

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// Static version.
    /// </summary>
    internal static double CalculateWidth(decimal weight, decimal maxWeight)
    {
        return _MIN_WIDTH + (double)weight / (double)maxWeight * (_MAX_WIDTH - _MIN_WIDTH);
    }

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// Instance version.
    /// </summary>
    protected double CalculateWidth()
    {
        return CalculateWidth(GymObject!.Weight, MaxWeight);
    }

    /// <summary>
    /// Construct a new drawable for a given GymObject.
    /// </summary>
    /// <param name="gymObject">A gym object.</param>
    /// <returns>A GymObjectDrawable corresponding to the provided GymObject.</returns>
    /// <exception cref="Exception"></exception>
    internal static GymObjectDrawable Create(GymObject gymObject)
    {
        // Get the type.
        string gymObjectTypeName = gymObject.GetType().Name;
        var drawableTypeName = $"GymCalc.Drawables.{gymObjectTypeName}Drawable";
        var drawableType = Type.GetType(drawableTypeName);
        if (drawableType == null)
        {
            throw new InvalidOperationException(
                $"Could not create a drawable for the {gymObjectTypeName} object.");
        }

        // Create the drawable object.
        object? drawable = Activator.CreateInstance(drawableType);
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
