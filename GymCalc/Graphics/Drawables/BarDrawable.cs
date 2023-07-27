using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class BarDrawable : GymObjectDrawable
{
    internal const int Height = 20;

    private const int _MinWidth = 50;

    private const int _MaxWidth = 250;

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var bar = (Bar)GymObject;
        var width = dirtyRect.Width;

        // Bar background.
        canvas.FillColor = CustomColors.Get("PaleGray");
        var barBackground = new RectF(0, 0, width, Height);
        canvas.FillRectangle(barBackground);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
        canvas.DrawString(weightString, 0, 2, width, Height, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    internal override GraphicsView CreateGraphic()
    {
        // Calculate the bar width.
        var barWidth = _MinWidth + GymObject.Weight / MaxWeight * (_MaxWidth - _MinWidth);

        // Construct the graphic.
        return new GraphicsView
        {
            Drawable = this,
            HeightRequest = Height,
            WidthRequest = barWidth,
        };
    }
}
