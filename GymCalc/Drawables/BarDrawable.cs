using Galaxon.Maui.Utilities;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class BarDrawable : GymObjectDrawable
{
    /// <summary>
    /// Constant height for bar graphics.
    /// </summary>
    private const int _HEIGHT = 20;

    /// <inheritdoc/>
    protected override double GetWidth()
    {
        return CalculateWidth();
    }

    /// <inheritdoc/>
    protected override double GetHeight()
    {
        return _HEIGHT;
    }

    /// <inheritdoc/>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var bar = (Bar)GymObject!;
        var height = (float)Height;
        var width = (float)Width;

        // Bar background.
        Color color = Palette.GetColor(bar.Color) ?? Colors.Silver;
        canvas.FillColor = color;
        var barBackground = new RectF(0, 0, width, height);
        canvas.FillRectangle(barBackground);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = color.GetTextColor();
        var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
        int offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 0, offset, width, height, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}
