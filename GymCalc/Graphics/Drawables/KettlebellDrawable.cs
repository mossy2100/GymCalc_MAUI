using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class KettlebellDrawable : IDrawable
{
    internal const int Height = 76;

    internal const int Width = 60;

    private readonly Kettlebell _kettlebell;

    public KettlebellDrawable(Kettlebell kettlebell)
    {
        _kettlebell = kettlebell;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Colors.
        var ballColor = Color.Parse(_kettlebell.BallColor);
        var bandColor = _kettlebell.HasBands ? Color.Parse(_kettlebell.BandColor) : ballColor;

        // Useful dimensions.
        const int y0 = 5;
        const int y1 = 15;
        const int y2 = 25;
        const int y3 = 35;

        // Handle top.
        canvas.StrokeColor = CustomColors.PaleGray;
        canvas.StrokeSize = 10;
        const int diam = 20;
        canvas.DrawArc(10, y0, diam, diam, 90, 180, false, false);
        canvas.DrawLine(20, y0, 40, y0);
        canvas.DrawArc(30, y0, diam, diam, 0, 90, false, false);

        // Bands.
        canvas.StrokeColor = bandColor;
        canvas.DrawLine(10, y1, 10, y2);
        canvas.DrawLine(50, y1, 50, y2);

        // Handle bottom.
        canvas.StrokeColor = ballColor;
        canvas.DrawLine(10, y2, 10, y3);
        canvas.DrawLine(50, y2, 50, y3);

        // Ball.
        canvas.FillColor = ballColor;
        canvas.FillArc(0, 20, Width, Width, 240, 300, true);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 20;
        canvas.FontColor = ballColor.GetTextColor();
        var weightString = _kettlebell.Weight.ToString(CultureInfo.InvariantCulture);
        canvas.DrawString(weightString, 10, 37, 40, 30, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    internal static GraphicsView CreateGraphic(Kettlebell kettlebell)
    {
        return new GraphicsView
        {
            Drawable = new KettlebellDrawable(kettlebell),
            HeightRequest = Height,
            WidthRequest = Width,
        };
    }
}
