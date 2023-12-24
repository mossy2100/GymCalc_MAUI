using GymCalc.Drawables;

namespace GymCalc.Models;

public class ListItem
{
    public GymObject GymObject { get; set; }

    public GymObjectDrawable Drawable { get; set; }

    public bool Enabled { get; set; }

    public static int IconButtonHeight { get; set; } = 32;

    public double Height => Math.Max(Drawable.Height, IconButtonHeight);

    internal ListItem(GymObject gymObject, GymObjectDrawable drawable, bool enabled)
    {
        GymObject = gymObject;
        Drawable = drawable;
        Enabled = enabled;
    }
}
