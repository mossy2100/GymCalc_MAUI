using System.Globalization;
using Galaxon.Maui.Utilities;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class KettlebellDrawable : GymObjectDrawable
{
    /// <summary>
    /// Constant width for kettlebell graphics.
    /// </summary>
    private const int _WIDTH = 60;

    /// <summary>
    /// Constant height for kettlebell graphics.
    /// </summary>
    private const int _HEIGHT = 76;

    /// <inheritdoc/>
    protected override double GetWidth()
    {
        return _WIDTH;
    }

    /// <inheritdoc/>
    protected override double GetHeight()
    {
        return _HEIGHT;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var kettlebell = (Kettlebell)GymObject!;
        var width = (float)Width;

        // Colors.
        Color? ballColor = CustomColors.Get(kettlebell.BallColor) ?? CustomColors.Get("Black");
        Color? bandColor =
            (kettlebell.HasBands == true ? CustomColors.Get(kettlebell.BandColor) : null)
            ?? ballColor;

        // Useful dimensions.
        const int y0 = 5;
        const int y1 = 15;
        const int y2 = 25;
        const int y3 = 35;

        // Handle top.
        canvas.StrokeColor = CustomColors.Get("Silver");
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
        canvas.FillArc(0, 20, width, width, 240, 300, true);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 20;
        canvas.FontColor = ballColor!.GetTextColor();
        var weightString = kettlebell.Weight.ToString(CultureInfo.InvariantCulture);
        int offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 10, 35 + offset, 40, 30, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
