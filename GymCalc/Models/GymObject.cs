using SQLite;

namespace GymCalc.Models;

public class GymObject
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public double Weight { get; set; }

    public string Units { get; set; }

    public double WeightKg =>
        Units == Constants.Units.KILOGRAMS ? Weight : Weight * Constants.Units.KG_PER_LB;

    public bool Enabled { get; set; }
}
