using GymCalc.Models;

namespace GymCalc.Drawables;

public class PlatesDrawable : IDrawable
{
    public List<Plate> Plates { get; set; }

    public double Width
    {
        get
        {
            var maxPlateWeight = Plates.Count == 0 ? 0 : Plates.Max(p => p.Weight);
            return GymObjectDrawable.CalculateWidth(maxPlateWeight, MaxWeight);
        }
    }

    public double Height => Plates.Count * PlateDrawable.HEIGHT;

    internal decimal MaxWeight { get; set; }

    /// <inheritdoc/>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var rectWidth = dirtyRect.Width;
        var i = 0;
        foreach (var plate in Plates)
        {
            // Get the dimensions.
            var w = (float)GymObjectDrawable.CalculateWidth(plate.Weight, MaxWeight);
            var h = (float)PlateDrawable.HEIGHT;

            // Get the coordinates of the upper-left corner of the plate in the canvas.
            var x = (rectWidth - w) / 2f;
            var y = i * h;

            // Draw the plate on the canvas.
            PlateDrawable.DrawPlate(canvas, plate, x, y, w, h);

            // Next.
            i++;
        }
    }
}
