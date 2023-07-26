using System.Globalization;
using GymCalc.Data.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class DumbbellDrawable : GymObjectDrawable
{
    internal const int Height = 50;

    private const int _Width = 100;

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var dumbbell = (Dumbbell)GymObject;
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Bar.
        const int barHeight = 20;
        var barTop = (height - barHeight) / 2;
        var bar = new RectF(0, barTop, width, barHeight);
        canvas.FillColor = CustomColors.Get("PaleGray");
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = height - 2 * smallPlateTop;
        canvas.FillColor = CustomColors.Get(dumbbell.Color);

        // Left small plate.
        var leftSmallPlate = new RectF(gapWidth, smallPlateTop, plateWidth, smallPlateHeight);
        canvas.FillRoundedRectangle(leftSmallPlate, cornerRadius);

        // Right small plate.
        var rightSmallPlate = new RectF(width - gapWidth - plateWidth, smallPlateTop, plateWidth,
            smallPlateHeight);
        canvas.FillRoundedRectangle(rightSmallPlate, cornerRadius);

        // Left large plate.
        var leftLargePlate = new RectF(gapWidth * 2 + plateWidth, 0, plateWidth, height);
        canvas.FillRoundedRectangle(leftLargePlate, cornerRadius);

        // Right large plate.
        var rightLargePlate = new RectF(width - 2 * (gapWidth + plateWidth), 0, plateWidth, height);
        canvas.FillRoundedRectangle(rightLargePlate, cornerRadius);

        // Weight label.
        canvas.Font = Font.DefaultBold;
        canvas.FontSize = 16;
        canvas.FontColor = Colors.Black;
        var weightString = dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        var p = (height - barHeight) / 2 + 2;
        canvas.DrawString(weightString, m, p, width - m * 2, barHeight, HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    internal override GraphicsView CreateGraphic()
    {
        return new GraphicsView
        {
            Drawable = this,
            HeightRequest = Height,
            WidthRequest = _Width,
        };
    }
}
