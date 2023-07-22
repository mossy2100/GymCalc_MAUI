using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics;

internal class KettlebellDrawable : IDrawable
{
    private readonly Kettlebell _kettlebell;

    public KettlebellDrawable(Kettlebell kettlebell)
    {
        _kettlebell = kettlebell;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Background.
        // var bg = new RectF(0, 0, dirtyRect.Width, dirtyRect.Height);
        // canvas.FillColor = Color.Parse("#eee");
        // canvas.FillRectangle(bg);

        // Colors.
        var black = Color.Parse("#222");
        var steel = Color.Parse("#ddd");
        var kettlebellColor = Color.Parse(_kettlebell.Color);

        // Handle top.
        canvas.StrokeColor = steel;
        canvas.StrokeSize = 10;
        canvas.DrawLine(10, 15, 10, 20);
        canvas.DrawArc(10, 5, 20, 20, 90, 180, false, false);
        canvas.DrawLine(20, 5, 40, 5);
        canvas.DrawArc(30, 5, 20, 20, 0, 90, false, false);
        canvas.DrawLine(50, 15, 50, 20);

        // Handle bottom.
        canvas.StrokeColor = kettlebellColor;
        canvas.DrawLine(10, 20, 10, 35);
        canvas.DrawLine(50, 20, 50, 35);

        // Black bands.
        if (_kettlebell.HasBlackBands)
        {
            canvas.StrokeColor = black;
            canvas.DrawLine(10, 16, 10, 24);
            canvas.DrawLine(50, 16, 50, 24);
        }

        // Ball.
        canvas.FillColor = kettlebellColor;
        canvas.FillArc(0, 20, 60, 60, 240, 300, true);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 20;
        canvas.FontColor = kettlebellColor.GetTextColor();
        var weightString = _kettlebell.Weight.ToString(CultureInfo.InvariantCulture);
        canvas.DrawString(weightString, 10, 37, 40, 30, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
