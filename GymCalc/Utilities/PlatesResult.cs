using GymCalc.Data.Models;

namespace GymCalc.Utilities;

public class PlatesResult
{
    public double IdealWeight;

    public double ClosestWeight;

    public List<Plate> Plates = new ();
}
