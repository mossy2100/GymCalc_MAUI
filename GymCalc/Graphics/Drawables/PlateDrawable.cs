using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class PlateDrawable : GymObjectDrawable
{
    internal const int CornerRadius = 4;

    public PlateDrawable()
    {
        Height = 30;
        MinWidth = 50;
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var plate = (Plate)GymObject;
        var width = dirtyRect.Width;

        // Get the color.
        var bgColor = CustomColors.Get(plate.Color);

        // Plate background.
        canvas.FillColor = bgColor.AddLuminosity(-0.1f);
        var plateBackground = new RectF(0, 0, width, Height);
        canvas.FillRoundedRectangle(plateBackground, CornerRadius);

        // Plate edge.
        canvas.FillColor = bgColor;
        var innerHeight = Height - 2 * CornerRadius;
        var plateEdge = new RectF(0, CornerRadius, width, innerHeight);
        canvas.FillRectangle(plateEdge);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = bgColor.GetTextColor();
        var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        var offset = DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 0;
        canvas.DrawString(weightString, 0, CornerRadius + offset, width, innerHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    internal override GraphicsView CreateGraphic()
    {
        // Calculate the plate width.
        var plateWidth = MinWidth + GymObject.Weight / MaxWeight * (MaxWidth - MinWidth);

        // Construct the graphic.
        return new GraphicsView
        {
            Drawable = this,
            HeightRequest = Height,
            WidthRequest = plateWidth,
        };
    }
}
