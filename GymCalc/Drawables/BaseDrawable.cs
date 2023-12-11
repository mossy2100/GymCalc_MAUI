namespace GymCalc.Drawables;

public abstract class BaseDrawable : IDrawable
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
    /// Maximum weight for a gym object of the relevant type. This value is used when calculating
    /// widths of variable-width gym objects like bars and plates.
    /// </summary>
    public decimal MaxWeight { get; set; }

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
