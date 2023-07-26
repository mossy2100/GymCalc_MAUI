using System.Globalization;
using GymCalc.Data.Models;
using GymCalc.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class PlateDrawable : GymObjectDrawable
{
    internal const int MinWidth = 50;

    internal const int CornerRadius = 4;

    internal const int Height = 30;

    internal static int InnerHeight => Height - 2 * CornerRadius;

    public PlateDrawable()
    {
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
        var plateEdge = new RectF(0, CornerRadius, width, InnerHeight);
        canvas.FillRectangle(plateEdge);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = bgColor.GetTextColor();
        var weightString = plate.Weight.ToString(CultureInfo.InvariantCulture);
        canvas.DrawString(weightString, 0, CornerRadius + 2, width, InnerHeight,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    internal override GraphicsView CreateGraphic()
    {
        // Calculate the plate width.
        var maxPlateWidth = MauiUtilities.GetDeviceWidth() / PageLayout.GetNumColumns() * 0.7;
        var plateWidth = MinWidth + GymObject.Weight / MaxWeight * (maxPlateWidth - MinWidth);

        // Construct the graphic.
        return new GraphicsView
        {
            Drawable = this,
            HeightRequest = Height,
            WidthRequest = plateWidth,
        };
    }
}
