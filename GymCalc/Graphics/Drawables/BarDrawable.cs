using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class BarDrawable : IDrawable
{
    internal const int MinWidth = 50;

    internal const int Height = 20;

    private readonly Bar _bar;

    public BarDrawable(Bar bar)
    {
        _bar = bar;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;

        // Bar background.
        canvas.FillColor = CustomColors.PaleGray;
        var barBackground = new RectF(0, 0, width, Height);
        canvas.FillRectangle(barBackground);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = _bar.Weight.ToString(CultureInfo.InvariantCulture);
        canvas.DrawString(weightString, 0, 2, width, Height, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    internal static GraphicsView CreateGraphic(Bar bar, double maxBarWeight)
    {
        // Calculate the bar width.
        var maxBarWidth = MauiUtilities.GetDeviceWidth() / PageLayout.GetNumColumns() * 0.7;
        var barWidth = MinWidth + bar.Weight / maxBarWeight * (maxBarWidth - MinWidth);

        // Construct the graphic.
        return new GraphicsView
        {
            Drawable = new BarDrawable(bar),
            HeightRequest = Height,
            WidthRequest = barWidth,
        };
    }
}
