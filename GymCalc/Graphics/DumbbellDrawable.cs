using System.Globalization;
using GymCalc.Data.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics;

internal class DumbbellDrawable : IDrawable
{
    private readonly Dumbbell _dumbbell;

    public DumbbellDrawable(Dumbbell dumbbell)
    {
        _dumbbell = dumbbell;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Bar.
        const int barHeight = 20;
        var barTop = (height - barHeight) / 2;
        var barGradient = GetBarGradient();
        var bar = new RectF(0, barTop, width, barHeight);
        canvas.SetFillPaint(barGradient, bar);
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = height - 2 * smallPlateTop;
        var plateGradient = GetPlateGradient();

        // Left small plate.
        var leftSmallPlate = new RectF(gapWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.SetFillPaint(plateGradient, leftSmallPlate);
        canvas.FillRoundedRectangle(leftSmallPlate, cornerRadius);

        // Right small plate.
        var rightSmallPlate = new RectF(width - gapWidth - plateWidth, smallPlateTop, plateWidth,
            smallPlateHeight);
        canvas.SetFillPaint(plateGradient, rightSmallPlate);
        canvas.FillRoundedRectangle(rightSmallPlate, cornerRadius);

        // Left large plate.
        var leftLargePlate = new RectF(gapWidth * 2 + plateWidth, 0, plateWidth, height);
        canvas.SetFillPaint(plateGradient, leftLargePlate);
        canvas.FillRoundedRectangle(leftLargePlate, cornerRadius);

        // Right large plate.
        var rightLargePlate = new RectF(width - 2 * (gapWidth + plateWidth), 0, plateWidth, height);
        canvas.SetFillPaint(plateGradient, rightLargePlate);
        canvas.FillRoundedRectangle(rightLargePlate, cornerRadius);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 20;
        canvas.FontColor = Colors.Black;
        var weightString = _dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        canvas.DrawString(weightString, m, (height - barHeight) / 2, width - (m * 2), barHeight,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    /// <summary>
    /// Create the steel bar gradient.
    /// </summary>
    /// <returns></returns>
    private static Brush GetBarGradient()
    {
        var linearGradientPaint = new LinearGradientPaint
        {
            StartColor = Color.Parse("#aaa"),
            EndColor = Color.Parse("#aaa"),
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1),
        };
        linearGradientPaint.AddOffset(0.5f, Colors.White);
        return linearGradientPaint;
    }

    /// <summary>
    /// Create the black plate gradient.
    /// </summary>
    /// <returns></returns>
    private static LinearGradientPaint GetPlateGradient()
    {
        var linearGradientPaint = new LinearGradientPaint
        {
            StartColor = Color.Parse("#333"),
            EndColor = Color.Parse("#333"),
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1),
        };
        linearGradientPaint.AddOffset(0.5f, Colors.Gray);
        return linearGradientPaint;
    }
}
