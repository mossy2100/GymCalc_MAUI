using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class PlateDrawable : GymObjectDrawable
{
    public const double HEIGHT = 30;

    public const int CORNER_RADIUS = 4;

    /// <inheritdoc />
    public override double GetWidth()
    {
        return CalculateWidth(GymObject.Weight, MaxWeight);
    }

    /// <inheritdoc />
    public override double GetHeight()
    {
        return HEIGHT;
    }

    public static void DrawPlate(ICanvas canvas, Plate plate, float x, float y, float w, float h)
    {
        // Get the color.
        var bgColor = CustomColors.Get(plate.Color);

        // Plate background.
        canvas.FillColor = bgColor.AddLuminosity(-0.1f);
        var plateBackground = new RectF(x, y, w, h);
        canvas.FillRoundedRectangle(plateBackground, CORNER_RADIUS);

        // Plate edge.
        canvas.FillColor = bgColor;
        var innerHeight = h - 2 * CORNER_RADIUS;
        var plateEdge = new RectF(x, y + CORNER_RADIUS, w,
            innerHeight);
        canvas.FillRectangle(plateEdge);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = bgColor.GetTextColor();
        var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, x, y + CORNER_RADIUS + offset,
            w, innerHeight, HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var plate = (Plate)GymObject;
        var w = (float)Width;
        var h = (float)Height;
        var x = (dirtyRect.Width - w) / 2f;
        DrawPlate(canvas, plate, x, 0, w, h);

        // // Get the color.
        // var bgColor = CustomColors.Get(plate.Color);
        //
        // // Plate background.
        // canvas.FillColor = bgColor.AddLuminosity(-0.1f);
        // //
        // var plateBackground = new RectF(0, 0, width, height);
        // canvas.FillRoundedRectangle(plateBackground, CORNER_RADIUS);
        //
        // // Plate edge.
        // canvas.FillColor = bgColor;
        // var innerHeight = height - 2 * CORNER_RADIUS;
        // var plateEdge = new RectF(0, CORNER_RADIUS, width, innerHeight);
        // canvas.FillRectangle(plateEdge);
        //
        // // Weight label.
        // canvas.Font = Font.DefaultBold;
        // canvas.FontSize = 16;
        // canvas.FontColor = bgColor.GetTextColor();
        // var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        // var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        // canvas.DrawString(weightString, 0, CORNER_RADIUS + offset, width, innerHeight,
        //     HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
