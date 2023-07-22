using GymCalc.Data.Models;

namespace GymCalc.Graphics;

internal static class GraphicsFactory
{
    internal static GraphicsView CreateDumbbellGraphic(Dumbbell dumbbell)
    {
        return new GraphicsView
        {
            Drawable = new DumbbellDrawable(dumbbell),
            HeightRequest = Dumbbell.Height,
            WidthRequest = Dumbbell.Width,
        };
    }

    internal static GraphicsView CreateKettlebellGraphic(Kettlebell kettlebell)
    {
        return new GraphicsView
        {
            Drawable = new KettlebellDrawable(kettlebell),
            HeightRequest = Kettlebell.Height,
            WidthRequest = Kettlebell.Width,
        };
    }
}
