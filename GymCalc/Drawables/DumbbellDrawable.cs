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

    /// <inheritdoc/>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        DrawBarWithPlates(canvas);
    }
}
