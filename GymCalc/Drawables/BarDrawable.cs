using System.Globalization;
using GymCalc.Graphics;
using GymCalc.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Drawables;

public class BarDrawable : GymObjectDrawable
{
    public BarDrawable()
    {
        Height = 20;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var bar = (Bar)GymObject;
        var rectWidth = dirtyRect.Width;
        var barWidth = (float)(MinWidth + (bar.Weight / MaxWeight) * (rectWidth - MinWidth));

        // Bar background.
        canvas.FillColor = CustomColors.Get("PaleGray");
        var barBackground = new RectF((rectWidth - barWidth) / 2.0f, 0, barWidth, (float)Height);
        canvas.FillRectangle(barBackground);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 0, offset, rectWidth, (float)Height,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    // /// <inheritdoc />
    // internal override GraphicsView CreateGraphicsView()
    // {
    //     // Calculate the bar width.
    //     Width = MinWidth + GymObject.Weight / MaxWeightKg * (MaxWidth - MinWidth);
    //     return base.CreateGraphicsView();
    // }
}
