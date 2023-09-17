using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

public class PlateDrawable : GymObjectDrawable
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
        var plateBackground = new RectF(0, 0, width, (float)Height);
        canvas.FillRoundedRectangle(plateBackground, CornerRadius);

        // Plate edge.
        canvas.FillColor = bgColor;
        var innerHeight = (float)Height - 2 * CornerRadius;
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

    /// <inheritdoc />
    internal override GraphicsView CreateGraphicsView()
    {
        // Calculate the plate width.
        MaxWidth = 250;
        MaxWeight = 25;
        Width = MinWidth + GymObject.Weight / MaxWeight * (MaxWidth - MinWidth);
        return base.CreateGraphicsView();
    }
}
