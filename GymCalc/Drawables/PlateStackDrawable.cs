using GymCalc.Models;

namespace GymCalc.Drawables;

public class PlateStackDrawable : BaseDrawable
{
    /// <summary>
    /// The stack of plates.
    /// </summary>
    internal List<Plate>? Plates { get; init; }

    /// <inheritdoc/>
    protected override double GetWidth()
    {
        decimal maxPlateWeight =
            Plates == null || Plates.Count == 0 ? 0 : Plates.Max(p => p.Weight);
        return GymObjectDrawable.CalculateWidth(maxPlateWeight, MaxWeight);
    }

    /// <inheritdoc/>
    protected override double GetHeight()
    {
        return (Plates?.Count ?? 0) * PlateDrawable.HEIGHT;
    }

    /// <inheritdoc/>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float rectWidth = dirtyRect.Width;
        var i = 0;
        foreach (Plate plate in Plates!)
        {
            // Get the plate width.
            var w = (float)GymObjectDrawable.CalculateWidth(plate.Weight, MaxWeight);

            // Get the coordinates of the upper-left corner of the plate in the canvas.
            float x = (rectWidth - w) / 2f;
            float y = i * PlateDrawable.HEIGHT;

            // Draw the plate on the canvas.
            PlateDrawable.DrawPlate(canvas, plate, x, y, w, PlateDrawable.HEIGHT);

            // Next.
            i++;
        }
    }
}
