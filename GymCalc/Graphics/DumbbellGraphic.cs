namespace GymCalc.Graphics;

internal class DumbbellGraphic : IDrawable
{
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
