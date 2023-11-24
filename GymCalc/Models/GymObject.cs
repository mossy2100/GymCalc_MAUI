using System.ComponentModel.DataAnnotations.Schema;
using Galaxon.Core.Types;
using GymCalc.Shared;
using SQLite;

namespace GymCalc.Models;

public class GymObject
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public decimal Weight { get; set; }

    public string? Units { get; set; }

    [NotMapped]
    public decimal WeightKg =>
        Units == Constants.Units.Kilograms.GetDescription()
            ? Weight
            : Weight * UnitsUtility.KG_PER_LB;

    public bool Enabled { get; set; }

    /// <summary>
    /// Clone the gym object.
    /// </summary>
    /// <typeparam name="T">The gym object derived type.</typeparam>
    /// <returns>A copy of the object.</returns>
    public T Clone<T>() where T : GymObject
    {
        return (T)MemberwiseClone();
    }
}
