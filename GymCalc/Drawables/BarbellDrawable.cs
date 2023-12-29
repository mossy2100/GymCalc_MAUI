namespace GymCalc.Drawables;

public class BarbellDrawable : GymObjectDrawable
{
    /// <summary>
    /// Constant height for barbell graphics.
    /// </summary>
    private const int _HEIGHT = 50;

    /// <inheritdoc/>
    protected override double GetWidth()
    {
        return MAX_WIDTH;
    }

    /// <inheritdoc/>
    protected override double GetHeight()
    {
        return _HEIGHT;
    }

    /// <inheritdoc/>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        DrawBarWithPlates(canvas);
    }
}
