using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class PlateDrawable : GymObjectDrawable
{
    public const double HEIGHT = 30;

    private const int _CORNER_RADIUS = 4;

    /// <inheritdoc />
    public override double GetWidth()
    {
        return CalculateWidth();
    }

    /// <inheritdoc />
    public override double GetHeight()
    {
        return HEIGHT;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var plate = (Plate)GymObject;
        // var rectWidth = dirtyRect.Width;
        // var plateWidth = (float)(MIN_WIDTH + (plate.Weight / MaxWeight) * (rectWidth - MIN_WIDTH));
        // CalculateWidth(MAX_WIDTH, MaxWeight);

        var height = (float)Height;
        var width = (float)Width;

        // Get the color.
        var bgColor = CustomColors.Get(plate.Color);

        // Plate background.
        canvas.FillColor = bgColor.AddLuminosity(-0.1f);
        // var plateX = (rectWidth - Width) / 2f;
        var plateBackground = new RectF(0, 0, width, height);
        canvas.FillRoundedRectangle(plateBackground, _CORNER_RADIUS);

        // Plate edge.
        canvas.FillColor = bgColor;
        var innerHeight = height - 2 * _CORNER_RADIUS;
        var plateEdge = new RectF(0, _CORNER_RADIUS, width, innerHeight);
        canvas.FillRectangle(plateEdge);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = bgColor.GetTextColor();
        var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 0, _CORNER_RADIUS + offset, width, innerHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
