using Galaxon.Core.Enums;
using GymCalc.Utilities;
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
        Units == Constants.Units.Kilograms.GetDescription()
            ? Weight
            : Weight * UnitsUtility.KG_PER_LB;

    public bool Enabled { get; set; }
}
