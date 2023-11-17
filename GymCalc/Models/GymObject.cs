using Galaxon.Core.Enums;
using GymCalc.Shared;
using SQLite;

namespace GymCalc.Models;

public class GymObject
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public decimal Weight { get; set; }

    public string Units { get; set; }

    public decimal WeightKg =>
        Units == Constants.Units.Kilograms.GetDescription()
            ? Weight
            : Weight * UnitsUtility.KG_PER_LB;

    public bool Enabled { get; set; }
}
