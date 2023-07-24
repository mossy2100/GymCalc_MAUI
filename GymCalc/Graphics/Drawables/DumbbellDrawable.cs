using System.Globalization;
using GymCalc.Data.Models;
using Font = Microsoft.Maui.Graphics.Font;

namespace GymCalc.Graphics.Drawables;

internal class DumbbellDrawable : IDrawable
{
    internal const int Height = 50;

    internal const int Width = 100;

    private readonly Dumbbell _dumbbell;

    public DumbbellDrawable(Dumbbell dumbbell)
    {
        _dumbbell = dumbbell;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Bar.
        var barTop = (height - BarDrawable.Height) / 2;
        var bar = new RectF(0, barTop, width, BarDrawable.Height);
        canvas.FillColor = CustomColors.PaleGray;
        canvas.FillRectangle(bar);

        // Plates.
        const int gapWidth = 1;
        const int plateWidth = 12;
        const int cornerRadius = 2;
        var smallPlateTop = barTop / 3;
        var smallPlateHeight = height - 2 * smallPlateTop;
        canvas.FillColor = CustomColors.OffBlack;

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
        var weightString = _dumbbell.Weight.ToString(CultureInfo.InvariantCulture);
        const int m = (gapWidth + plateWidth) * 2;
        var p = (height - BarDrawable.Height) / 2 + 2;
        canvas.DrawString(weightString, m, p, width - m * 2, BarDrawable.Height,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    internal static GraphicsView CreateGraphic(Dumbbell dumbbell)
    {
        return new GraphicsView
        {
            Drawable = new DumbbellDrawable(dumbbell),
            HeightRequest = Height,
            WidthRequest = Width,
        };
    }
}
