using System.Globalization;
using GymCalc.Data.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

public class BarDrawable : GymObjectDrawable
{
    public BarDrawable()
    {
        Height = 20;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var bar = (Bar)GymObject;
        var width = dirtyRect.Width;

        // Bar background.
        canvas.FillColor = CustomColors.Get("PaleGray");
        var barBackground = new RectF(0, 0, width, (float)Height);
        canvas.FillRectangle(barBackground);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = bar.Weight.ToString(CultureInfo.InvariantCulture);
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 0, offset, width, (float)Height, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    // /// <inheritdoc />
    // internal override GraphicsView CreateGraphicsView()
    // {
    //     // Calculate the bar width.
    //     Width = MinWidth + GymObject.Weight / MaxWeightKg * (MaxWidth - MinWidth);
    //     return base.CreateGraphicsView();
    // }
}
