using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics;

internal class KettlebellGraphic : IDrawable
{
    private Kettlebell _kettlebell;

    public KettlebellGraphic(Kettlebell kettlebell)
    {
        _kettlebell = kettlebell;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        const float w = 60f;
        const float r = 30f;

        // Background.
        // var bg = new RectF(0, 0, w, h);
        // canvas.FillColor = Color.Parse("#eee");
        // canvas.FillRectangle(bg);

        // Colors.
        var black = Color.Parse("#222");
        // var chrome = Color.Parse("#ddd");
        var kettlebellColor = Color.Parse(_kettlebell.Color);

        // Handle.
        canvas.StrokeColor = kettlebellColor;
        canvas.StrokeSize = 10;
        canvas.DrawLine(6.34f, 20f, 16.34f, 37.32f);
        canvas.DrawArc(5f, 5f, 20f, 20f, 90, 210, false, false);
        canvas.DrawLine(15f, 5f, 45f, 5f);
        canvas.DrawArc(35f, 5f, 20f, 20f, 330, 90, false, false);
        canvas.DrawLine(53.66f, 20f, 43.66f, 37.32f);

        // Black bands.
        if (_kettlebell.HasBlackBands)
        {
            canvas.StrokeColor = black;
            canvas.DrawLine(11.34f, 28.66f, 16.34f, 37.32f);
            canvas.DrawLine(48.66f, 28.66f, 43.66f, 37.32f);
        }

        // Ball.
        canvas.FillColor = kettlebellColor;
        canvas.FillArc(0, r, w, w, 240, 300, true);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 20;
        canvas.FontColor = kettlebellColor.GetTextColor();
        var weightString = _kettlebell.Weight.ToString(CultureInfo.InvariantCulture);
        var maxStringWidth = 100;
        canvas.DrawString(weightString, (w - maxStringWidth) / 2, 45, maxStringWidth, 30,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
