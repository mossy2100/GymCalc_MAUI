using SQLite;

namespace GymCalc.Data.Models;

public class Plate
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public double Weight { get; set; }

    public string Unit { get; set; }

    public bool Enabled { get; set; }

    public string Color { get; set; }
}
