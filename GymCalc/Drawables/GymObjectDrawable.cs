using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : IDrawable
{
    public GymObject GymObject { get; set; }

    public double Width => GetWidth();

    public double Height => GetHeight();

    public const double MIN_WIDTH = 50;

    /// <summary>
    /// TODO - Update to use real available width.
    /// </summary>
    public const double MAX_WIDTH = 200;

    internal double MaxWeight { get; set; }

    public abstract double GetWidth();

    public abstract double GetHeight();

    /// <inheritdoc />
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    /// <summary>
    /// Calculate variable (weight-dependent) width for bars and plates.
    /// </summary>
    public double CalculateWidth()
    {
        return MIN_WIDTH + (GymObject.Weight / MaxWeight) * (MAX_WIDTH - MIN_WIDTH);
    }

    // /// <summary>
    // /// Create a GraphicsView that references the Drawable.
    // /// The new GraphicsView can then be added to a Layout.
    // /// </summary>
    // internal GraphicsView CreateGraphicsView()
    // {
    //     return new GraphicsView
    //     {
    //         Drawable = this,
    //         HeightRequest = Height,
    //         WidthRequest = Width,
    //     };
    // }
}
