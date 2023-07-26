using SQLite;

namespace GymCalc.Data.Models;

public class GymObject
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public double Weight { get; set; }

    public string Units { get; set; }

    public bool Enabled { get; set; }
}
