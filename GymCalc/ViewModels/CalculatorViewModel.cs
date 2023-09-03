using GymCalc.Constants;

namespace GymCalc.ViewModels;

public class CalculatorViewModel
{
    public ExerciseType SelectedExerciseType { get; set; }

    public double MaxWeight { get; set; }

    public double BarWeight { get; set; }

    public double StartingWeight { get; set; }

    public bool OneSideOnly { get; set; }
}
