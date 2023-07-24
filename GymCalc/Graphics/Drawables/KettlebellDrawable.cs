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

        // Handle top.
        canvas.StrokeColor = CustomColors.PaleGray;
        canvas.StrokeSize = 10;
        canvas.DrawArc(10, 5, 20, 20, 90, 180, false, false);
        canvas.DrawLine(20, 5, 40, 5);
        canvas.DrawArc(30, 5, 20, 20, 0, 90, false, false);

        // Bands.
        var y1 = 15;
        var y2 = 25;
        var y3 = 35;
        canvas.StrokeColor = bandColor;
        canvas.DrawLine(10, y1, 10, y2);
        canvas.DrawLine(50, y1, 50, y2);

        // Handle bottom.
        canvas.StrokeColor = ballColor;
        canvas.DrawLine(10, y2, 10, y3);
        canvas.DrawLine(50, y2, 50, y3);

        // Ball.
        canvas.FillColor = ballColor;
        canvas.FillArc(0, 20, 60, 60, 240, 300, true);

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
