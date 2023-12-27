using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class DumbbellDrawable : GymObjectDrawable
{
    /// <summary>
    /// Constant width for dumbbell graphics.
    /// </summary>
    private const int _WIDTH = 100;

    /// <summary>
    /// Constant height for dumbbell graphics.
    /// </summary>
    private const int _HEIGHT = 50;

    /// <inheritdoc/>
    protected override double GetWidth()
    {
        return _WIDTH;
    }

    /// <inheritdoc/>
    protected override double GetHeight()
    {
        return _HEIGHT;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var dumbbell = (Dumbbell)GymObject!;
        var height = (float)Height;
        var width = (float)Width;

        // Bar.
        const int barHeight = 20;
        var barTop = (float)((height - barHeight) / 2.0);
        var bar = new RectF(0, barTop, width, barHeight);
        canvas.FillColor = CustomColors.Get("Silver");
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        float smallPlateTop = barTop / 3;
        float smallPlateHeight = height - 2 * smallPlateTop;
        canvas.FillColor = CustomColors.Get(dumbbell.Color) ?? CustomColors.Get("Black");

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
        var weightString = dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        int offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        float p = (height - barHeight) / 2 + offset;
        canvas.DrawString(weightString, m, p, width - m * 2, barHeight, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
