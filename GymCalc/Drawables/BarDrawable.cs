using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class BarDrawable : GymObjectDrawable
{
    public const int HEIGHT = 20;

    /// <inheritdoc/>
    public override double GetWidth()
    {
        return CalculateWidth();
    }

    /// <inheritdoc/>
    public override double GetHeight()
    {
        return HEIGHT;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var bar = (Bar)GymObject!;
        var height = (float)Height;
        var width = (float)Width;

        // Bar background.
        canvas.FillColor = CustomColors.Get("PaleGray");
        var barBackground = new RectF(0, 0, width, height);
        canvas.FillRectangle(barBackground);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
        int offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 0, offset, width, height, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
