using GymCalc.Models;

namespace GymCalc.Drawables;

public abstract class GymObjectDrawable : IDrawable
{
    internal GymObject GymObject { get; set; }

    internal double Height { get; set; }

    internal double Width { get; set; }

    internal double MinWidth { get; set; }

    internal double MaxWidth { get; set; }

    internal double MaxWeight { get; set; }

    /// <inheritdoc />
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    /// <summary>
    /// Create a GraphicsView that references the Drawable.
    /// The new GraphicsView can then be added to a Layout.
    /// </summary>
    internal GraphicsView CreateGraphicsView()
    {
        return new GraphicsView
        {
            Drawable = this,
            HeightRequest = Height,
            WidthRequest = Width,
        };
    }
}
