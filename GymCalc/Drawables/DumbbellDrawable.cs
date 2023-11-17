using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class DumbbellDrawable : GymObjectDrawable
{
    public const decimal HEIGHT = 50;

    public const decimal WIDTH = 100;

    /// <inheritdoc/>
    public override decimal GetWidth()
    {
        return WIDTH;
    }

    /// <inheritdoc/>
    public override decimal GetHeight()
    {
        return HEIGHT;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var dumbbell = (Dumbbell)GymObject;
        // var rectWidth = dirtyRect.Width;
        // var rectHeight = dirtyRect.Height;

        var height = (float)Height;
        var width = (float)Width;

        // Bar.
        const int barHeight = 20;
        var barTop = (float)((height - barHeight) / 2.0);
        var barLeft = 0; //(float)((width - Width) / 2.0);
        var bar = new RectF(barLeft, barTop, width, barHeight);
        canvas.FillColor = CustomColors.Get("PaleGray");
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = height - 2 * smallPlateTop;
        canvas.FillColor = CustomColors.Get(dumbbell.Color);

        // Left small plate.
        var leftSmallPlate =
            new RectF(barLeft + gapWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.FillRoundedRectangle(leftSmallPlate, cornerRadius);

        // Right small plate.
        var rightSmallPlate = new RectF(width - barLeft - gapWidth - plateWidth, smallPlateTop,
            plateWidth, smallPlateHeight);
        canvas.FillRoundedRectangle(rightSmallPlate, cornerRadius);

        // Left large plate.
        var leftLargePlate =
            new RectF(barLeft + gapWidth * 2 + plateWidth, 0, plateWidth, height);
        canvas.FillRoundedRectangle(leftLargePlate, cornerRadius);

        // Right large plate.
        var rightLargePlate = new RectF(width - barLeft - 2 * (gapWidth + plateWidth), 0,
            plateWidth, height);
        canvas.FillRoundedRectangle(rightLargePlate, cornerRadius);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        var p = (height - barHeight) / 2 + offset;
        canvas.DrawString(weightString, m, p, width - m * 2, barHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
