using GymCalc.Drawables;

namespace GymCalc.Models;

public class ListItem
{
    public GymObject? GymObject { get; set; }

    public GymObjectDrawable? Drawable { get; set; }

    public bool Enabled { get; set; }
}
