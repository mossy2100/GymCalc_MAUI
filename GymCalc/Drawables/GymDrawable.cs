namespace GymCalc.Drawables;

public abstract class GymDrawable : IDrawable
{
    /// <summary>
    /// The width of the drawable in device-independent pixels.
    /// </summary>
    public double Width => GetWidth();

    /// <summary>
    /// The height of the drawable in device-independent pixels.
    /// </summary>
    public double Height => GetHeight();

    /// <summary>
    /// Calculate the width of the drawable in device-independent pixels.
    /// </summary>
    protected abstract double GetWidth();

    /// <summary>
    /// Calculate the height of the drawable in device-independent pixels.
    /// </summary>
    protected abstract double GetHeight();

    /// <inheritdoc />
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);
}
