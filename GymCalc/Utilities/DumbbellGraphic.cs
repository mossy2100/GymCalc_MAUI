namespace GymCalc.Utilities;

public class DumbbellGraphic : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Bar.
        var barHeight = 20;
        var barTop = (height - barHeight) / 2;
        var barGradient = GetBarGradient();
        RectF bar = new RectF(0, barTop, width, barHeight);
        canvas.SetFillPaint(barGradient, bar);
        canvas.FillRectangle(bar);

        // Plates.
        var gapWidth = 1;
        var plateWidth = 10;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = height - 2 * smallPlateTop;
        var cornerRadius = 2;
        var plateGradient = GetPlateGradient();

        // Left small plate.
        RectF leftSmallPlate = new RectF(gapWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.SetFillPaint(plateGradient, leftSmallPlate);
        canvas.FillRoundedRectangle(leftSmallPlate, cornerRadius);

        // Right small plate.
        RectF rightSmallPlate = new RectF(width - gapWidth - plateWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.SetFillPaint(plateGradient, rightSmallPlate);
        canvas.FillRoundedRectangle(rightSmallPlate, cornerRadius);

        // Left large plate.
        RectF leftLargePlate = new RectF(gapWidth * 2 + plateWidth, 0, plateWidth, height);
        canvas.SetFillPaint(plateGradient, leftLargePlate);
        canvas.FillRoundedRectangle(leftLargePlate, cornerRadius);

        // Right large plate.
        RectF rightLargePlate = new RectF(width - 2 * (gapWidth + plateWidth), 0, plateWidth, height);
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
            EndPoint = new Point(0, 1)
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
            EndPoint = new Point(0, 1)
        };
        linearGradientPaint.AddOffset(0.5f, Colors.Gray);
        return linearGradientPaint;
    }
}
