using SQLite;

namespace GymCalc.Models;

public class GymObject
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public decimal Weight { get; set; }

    public string? Units { get; set; }

    public bool Enabled { get; set; }

    public string? Color { get; set; }
}
