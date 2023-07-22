using GymCalc.Data.Models;
using GymCalc.Graphics.Drawables;
using GymCalc.Graphics.Objects;

namespace GymCalc.Graphics;

internal static class GraphicsFactory
{
    internal static GraphicsView CreateDumbbellGraphic(Dumbbell dumbbell)
    {
        return new GraphicsView
        {
            Drawable = new DumbbellDrawable(dumbbell),
            HeightRequest = DumbbellGraphic.Height,
            WidthRequest = DumbbellGraphic.Width,
        };
    }

    internal static GraphicsView CreateKettlebellGraphic(Kettlebell kettlebell)
    {
        return new GraphicsView
        {
            Drawable = new KettlebellDrawable(kettlebell),
            HeightRequest = KettlebellGraphic.Height,
            WidthRequest = KettlebellGraphic.Width,
        };
    }
}
