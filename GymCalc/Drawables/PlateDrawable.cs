using Galaxon.Maui.Utilities;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

internal class PlateDrawable : GymObjectDrawable
{
    /// <summary>
    /// Constant height for plates.
    /// </summary>
    internal const int HEIGHT = 30;

    /// <summary>
    /// Constant for corner radius of plate graphic.
    /// </summary>
    private const int _CORNER_RADIUS = 4;

    /// <inheritdoc/>
    protected override double GetWidth()
    {
        return CalculateWidth();
    }

    /// <inheritdoc/>
    protected override double GetHeight()
    {
        return HEIGHT;
    }

    /// <summary>
    /// Used by both this class and PlatesDrawable.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="plate"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    internal static void DrawPlate(ICanvas canvas, Plate plate, float x, float y, float w, float h)
    {
        // Get the color.
        Color bgColor = Palette.GetColor(plate.Color);

        // Plate background.
        canvas.FillColor = bgColor.AddLuminosity(-0.1f);
        var plateBackground = new RectF(x, y, w, h);
        canvas.FillRoundedRectangle(plateBackground, _CORNER_RADIUS);

        // Plate edge.
        canvas.FillColor = bgColor;
        float innerHeight = h - 2 * _CORNER_RADIUS;
        var plateEdge = new RectF(x, y + _CORNER_RADIUS, w, innerHeight);
        canvas.FillRectangle(plateEdge);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = (float)FontSizes.MEDIUM;
        canvas.FontColor = bgColor.GetTextColor();
        var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        int offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, x, y + _CORNER_RADIUS + offset, w, innerHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    /// <inheritdoc/>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var plate = (Plate)GymObject!;
        var w = (float)Width;
        var h = (float)Height;
        float x = (dirtyRect.Width - w) / 2f;
        DrawPlate(canvas, plate, x, 0, w, h);
    }
}
