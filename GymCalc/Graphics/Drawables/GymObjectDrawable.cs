using GymCalc.Data.Models;

namespace GymCalc.Graphics.Drawables;

internal abstract class GymObjectDrawable : IDrawable
{
    internal GymObject GymObject { get; set; }

    internal double MaxWeight { get; set; }

    protected GymObjectDrawable()
    {
    }

    /// <inheritdoc />
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);

    internal abstract GraphicsView CreateGraphic();
}
