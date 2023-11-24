using System.Globalization;
using Galaxon.Maui.Utilities;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class PlateDrawable : GymObjectDrawable
{
    public const int HEIGHT = 30;

    public const int CORNER_RADIUS = 4;

    /// <inheritdoc/>
    public override double GetWidth()
    {
        return CalculateWidth();
    }

    /// <inheritdoc/>
    public override double GetHeight()
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
    public static void DrawPlate(ICanvas canvas, Plate plate, float x, float y, float w, float h)
    {
        // Get the color.
        var bgColor = CustomColors.Get(plate.Color) ?? CustomColors.Get("OffBlack");

        // Plate background.
        canvas.FillColor = bgColor!.AddLuminosity(-0.1f);
        var plateBackground = new RectF(x, y, w, h);
        canvas.FillRoundedRectangle(plateBackground, CORNER_RADIUS);

        // Plate edge.
        canvas.FillColor = bgColor;
        var innerHeight = h - 2 * CORNER_RADIUS;
        var plateEdge = new RectF(x, y + CORNER_RADIUS, w, innerHeight);
        canvas.FillRectangle(plateEdge);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = bgColor.GetTextColor();
        var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, x, y + CORNER_RADIUS + offset, w, innerHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var plate = (Plate)GymObject!;
        var w = (float)Width;
        var h = (float)Height;
        var x = (dirtyRect.Width - w) / 2f;
        DrawPlate(canvas, plate, x, 0, w, h);
    }
}
