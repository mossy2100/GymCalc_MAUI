using SQLite;

namespace GymCalc.Data.Models;

internal class HeavyThing
{
    [PrimaryKey, AutoIncrement]
    internal int Id { get; set; }

    internal double Weight { get; set; }

    internal string Unit { get; set; }

    internal bool Enabled { get; set; }
}
