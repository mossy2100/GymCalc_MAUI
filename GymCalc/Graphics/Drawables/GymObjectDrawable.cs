using GymCalc.Data.Models;

namespace GymCalc.Graphics.Drawables;

internal abstract class GymObjectDrawable : IDrawable
{
    internal GymObject GymObject { get; set; }

    internal int Height { get; set; }

    internal int Width { get; set; }

    internal int MinWidth { get; set; }

    internal int MaxWidth { get; set; }

    internal double MaxWeight { get; set; }

    /// <inheritdoc />
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    internal abstract GraphicsView CreateGraphic();
}
