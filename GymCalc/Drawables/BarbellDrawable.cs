using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class BarbellDrawable : GymObjectDrawable
{
    public const int HEIGHT = 50;

    public const int WIDTH = 200;

    /// <inheritdoc/>
    public override double GetWidth()
    {
        return WIDTH;
    }

    /// <inheritdoc/>
    public override double GetHeight()
    {
        return HEIGHT;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var barbell = (Barbell)GymObject!;
        var height = (float)Height;
        var width = (float)Width;

        // Bar.
        const int barHeight = 20;
        var barTop = (float)((height - barHeight) / 2.0);
        var bar = new RectF(0, barTop, width, barHeight);
        canvas.FillColor = CustomColors.Get("PaleGray");
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = height - 2 * smallPlateTop;
        canvas.FillColor = CustomColors.Get("OffBlack");

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
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = barbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        var p = (height - barHeight) / 2 + offset;
        canvas.DrawString(weightString, m, p, width - m * 2, barHeight, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}