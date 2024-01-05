using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : BaseDrawable
{
    /// <summary>
    /// Constant for the minimum width of a gym object graphic.
    /// </summary>
    protected const int MIN_WIDTH = 50;

    /// <summary>
    /// Constant for the maximum width of a gym object graphic.
    /// </summary>
    protected const int MAX_WIDTH = 180;

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
        return MIN_WIDTH + (double)weight / (double)maxWeight * (MAX_WIDTH - MIN_WIDTH);
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
    /// <exception cref="InvalidOperationException"></exception>
    internal static GymObjectDrawable Create(GymObject gymObject)
    {
        // Get the type.
        string gymObjectTypeName = gymObject.GetType().Name;
        var drawableTypeName = $"GymCalc.Drawables.{gymObjectTypeName}Drawable";
        var drawableType = Type.GetType(drawableTypeName);
        if (drawableType == null)
        {
            throw new InvalidOperationException(
                $"Could not find the drawable type for the {gymObjectTypeName} object.");
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

    /// <summary>
    /// Common method for drawing dumbbells and fixed barbells.
    /// </summary>
    /// <param name="canvas"></param>
    /// <exception cref="InvalidOperationException">if the gym object isn't yet set.</exception>
    protected void DrawBarWithPlates(ICanvas canvas)
    {
        if (GymObject == null)
        {
            throw new InvalidOperationException("Gym object not set.");
        }

        var height = (float)Height;
        var width = (float)Width;

        // Bar.
        const int barHeight = 20;
        var barTop = (float)((height - barHeight) / 2.0);
        var bar = new RectF(0, barTop, width, barHeight);
        canvas.FillColor = Palette.GetColor("Silver");
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        float smallPlateTop = barTop / 3;
        float smallPlateHeight = height - 2 * smallPlateTop;
        canvas.FillColor = Palette.GetColor(GymObject.Color);

        // Left small plate.
        var leftSmallPlate = new RectF(gapWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.FillRoundedRectangle(leftSmallPlate, cornerRadius);

        // Right small plate.
        var rightSmallPlate = new RectF(width - gapWidth - plateWidth, smallPlateTop, plateWidth,
            smallPlateHeight);
        canvas.FillRoundedRectangle(rightSmallPlate, cornerRadius);

        // Left large plate.
        var leftLargePlate = new RectF(gapWidth * 2 + plateWidth, 0, plateWidth, height);
        canvas.FillRoundedRectangle(leftLargePlate, cornerRadius);

        // Right large plate.
        var rightLargePlate = new RectF(width - 2 * (gapWidth + plateWidth), 0, plateWidth, height);
        canvas.FillRoundedRectangle(rightLargePlate, cornerRadius);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = (float)TextSize.MEDIUM;
        canvas.FontColor = Colors.Black;
        var weightString = GymObject.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        int offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        float p = (height - barHeight) / 2 + offset;
        canvas.DrawString(weightString, m, p, width - m * 2, barHeight, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
