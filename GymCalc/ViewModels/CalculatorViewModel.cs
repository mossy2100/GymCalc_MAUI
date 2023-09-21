using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Calculations;
using GymCalc.Constants;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;

namespace GymCalc.ViewModels;

public class CalculatorViewModel : BaseViewModel
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Bound properties.
    public string MaxWeightText { get; set; }

    public double BarWeight { get; set; }

    public string StartingWeightText { get; set; }

    public bool OneSideOnly { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Derived properties.
    public ExerciseType SelectedExerciseType { get; set; }

    public double? MaxWeight =>
        double.TryParse(MaxWeightText, out var maxWeight) ? maxWeight : null;

    public double? StartingWeight
    {
        get
        {
            // Treat blank as equal to 0.
            if (string.IsNullOrEmpty(StartingWeightText))
            {
                return 0;
            }
            return double.TryParse(StartingWeightText, out var startingWeight)
                ? startingWeight
                : null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Values extracted from user preferences.
    internal static string Units => GymCalc.Constants.Units.GetPreferred();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Commands.
    public ICommand CalculateCommand { get; private set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Lookup table for available bars.
    private Dictionary<double, Bar> _bars;

    internal Dictionary<double, Bar> Bars
    {
        get
        {
            if (_bars == null)
            {
                var task = Task.Run(() => BarRepository.GetInstance().GetAll(Units, true));
                task.Wait(1000);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var bars = task.Result;
                    _bars = bars.ToDictionary(p => p.Weight, p => p);
                }
                else
                {
                    ErrorMessage = "Could not load bars.";
                }
            }
            return _bars;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Lookup table for available plates.
    private List<Plate> _plates;

    internal List<Plate> Plates
    {
        get
        {
            if (_plates == null)
            {
                var task = Task.Run(() => PlateRepository.GetInstance().GetAll(Units, true));
                task.Wait(1000);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    _plates = task.Result;
                }
                else
                {
                    ErrorMessage = "Could not load plates.";
                }
            }
            return _plates;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Lookup table for available dumbbells.
    private Dictionary<double, Dumbbell> _dumbbells;

    internal Dictionary<double, Dumbbell> Dumbbells
    {
        get
        {
            if (_dumbbells == null)
            {
                var task = Task.Run(() => DumbbellRepository.GetInstance().GetAll(Units, true));
                task.Wait(1000);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var dumbbells = task.Result;
                    _dumbbells = dumbbells.ToDictionary(p => p.Weight, p => p);
                }
                else
                {
                    ErrorMessage = "Could not load dumbbells.";
                }
            }
            return _dumbbells;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Lookup table for available kettlebells.
    private Dictionary<double, Kettlebell> _kettlebells;

    internal Dictionary<double, Kettlebell> Kettlebells
    {
        get
        {
            if (_kettlebells == null)
            {
                var task = Task.Run(() => KettlebellRepository.GetInstance().GetAll(Units, true));
                task.Wait(1000);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var kettlebells = task.Result;
                    _kettlebells = kettlebells.ToDictionary(p => p.Weight, p => p);
                }
                else
                {
                    ErrorMessage = "Could not load kettlebells.";
                }
            }
            return _kettlebells;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Results.

    private List<PlatesResult> _platesResult;

    public List<PlatesResult> PlatesResults
    {
        get => _platesResult;

        set => SetProperty(ref _platesResult, value);
    }

    private bool _platesResultsVisible;

    public bool PlatesResultsVisible
    {
        get => _platesResultsVisible;

        set => SetProperty(ref _platesResultsVisible, value);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Error message.
    private string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;

        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CalculatorViewModel()
    {
        CalculateCommand = new AsyncCommand(async () => await Calculate());
    }

    #region Validation methods

    private bool ValidateMaxWeight()
    {
        // Check for number greater than 0.
        if (MaxWeight is null or <= 0)
        {
            ErrorMessage = "Please enter a maximum weight greater than 0.";
            return false;
        }

        ErrorMessage = "";
        return true;
    }

    private bool ValidateStartingWeight()
    {
        // Check for number greater than or equal to 0.
        if (StartingWeight is null or < 0)
        {
            ErrorMessage = "Please enter a starting weight greater than or equal to 0.";
            return false;
        }

        ErrorMessage = "";
        return true;
    }

    #endregion Validation methods

    #region Calculations

    private async Task Calculate()
    {
        switch (SelectedExerciseType)
        {
            case ExerciseType.Barbell:
                DoBarbellCalculations();
                break;

            case ExerciseType.Machine:
                DoMachineCalculations();
                break;

            case ExerciseType.Dumbbell:
                await DoDumbbellCalculations();
                break;

            case ExerciseType.Kettlebell:
                await DoKettlebellCalculations();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(SelectedExerciseType),
                    "Invalid exercise type.");
        }
    }

    private void DoBarbellCalculations()
    {
        PlatesResultsVisible = false;

        if (!ValidateMaxWeight())
        {
            return;
        }

        // Calculate and display the results.
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, BarWeight, true, Plates,
            "Plates each end");
        PlatesResultsVisible = true;
    }

    private void DoMachineCalculations()
    {
        if (!ValidateMaxWeight() || !ValidateStartingWeight())
        {
            return;
        }

        // Calculate and display the results.
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, StartingWeight!.Value,
            OneSideOnly, Plates, "Plates each side");
        PlatesResultsVisible = true;
    }

    private async Task DoDumbbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get the available dumbbell weights ordered from lightest to heaviest.
        // var availDumbbells = Dumbbells.Keys.ToList();

        // Calculate and display the results.
        // DumbbellResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, availDumbbells);
        // await DisplayDumbbellResults();
    }

    private async Task DoKettlebellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get the available kettlebell weights ordered from lightest to heaviest.
        // var availKettlebells = Kettlebells.Keys.ToList();

        // Calculate and display the results.
        // KettlebellResults = SingleWeightSolver.CalculateResults(MaxWeight!.Value, availKettlebells);
        // await DisplayKettlebellResults();
    }

    #endregion Calculations
}
