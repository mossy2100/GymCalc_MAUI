using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class DumbbellDrawable : GymObjectDrawable
{
    public DumbbellDrawable()
    {
        Height = 50;
        Width = 100;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var dumbbell = (Dumbbell)GymObject;
        var rectWidth = dirtyRect.Width;
        var rectHeight = dirtyRect.Height;

        // Bar.
        const int barHeight = 20;
        var barTop = (float)((rectHeight - barHeight) / 2.0);
        var barLeft = (float)((rectWidth - Width) / 2.0);
        var bar = new RectF(barLeft, barTop, (float)Width, barHeight);
        canvas.FillColor = CustomColors.Get("PaleGray");
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = rectHeight - 2 * smallPlateTop;
        canvas.FillColor = CustomColors.Get(dumbbell.Color);

        // Left small plate.
        var leftSmallPlate =
            new RectF(barLeft + gapWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.FillRoundedRectangle(leftSmallPlate, cornerRadius);

        // Right small plate.
        var rightSmallPlate = new RectF(rectWidth - barLeft - gapWidth - plateWidth, smallPlateTop,
            plateWidth, smallPlateHeight);
        canvas.FillRoundedRectangle(rightSmallPlate, cornerRadius);

        // Left large plate.
        var leftLargePlate =
            new RectF(barLeft + gapWidth * 2 + plateWidth, 0, plateWidth, rectHeight);
        canvas.FillRoundedRectangle(leftLargePlate, cornerRadius);

        // Right large plate.
        var rightLargePlate = new RectF(rectWidth - barLeft - 2 * (gapWidth + plateWidth), 0,
            plateWidth, rectHeight);
        canvas.FillRoundedRectangle(rightLargePlate, cornerRadius);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        var p = (rectHeight - barHeight) / 2 + offset;
        canvas.DrawString(weightString, m, p, rectWidth - m * 2, barHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
