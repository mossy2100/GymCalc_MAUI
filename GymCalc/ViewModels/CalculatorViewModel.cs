using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using GymCalc.Calculations;
using GymCalc.Constants;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;

namespace GymCalc.ViewModels;

public class CalculatorViewModel : INotifyPropertyChanged
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

    public double? StartingWeight =>
        double.TryParse(StartingWeightText, out var startingWeight) ? startingWeight : null;

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
    private Dictionary<double, Plate> _plates;

    internal Dictionary<double, Plate> Plates
    {
        get
        {
            if (_plates == null)
            {
                var task = Task.Run(() => PlateRepository.GetInstance().GetAll(Units, true));
                task.Wait(1000);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var plates = task.Result;
                    _plates = plates.ToDictionary(p => p.Weight, p => p);
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
    public Dictionary<int, PlatesResult> PlatesResults { get; private set; }

    // internal List<PlatesResult> BarbellResults { get; private set; }
    //
    // internal List<PlatesResult> DumbbellResults { get; private set; }
    //
    // internal List<PlatesResult> MachineResults { get; private set; }
    //
    // internal List<PlatesResult> KettlebellResults { get; private set; }

    private PlatesResult _platesResult;

    public PlatesResult CurrentPlatesResult
    {
        get => _platesResult;

        set
        {
            if (_platesResult != value)
            {
                _platesResult = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _showPlatesResults;

    public bool ShowPlatesResults
    {
        get => _showPlatesResults;

        set
        {
            if (_showPlatesResults != value)
            {
                _showPlatesResults = value;
                OnPropertyChanged();

                if (value)
                {
                    ShowSingleWeightResults = false;
                }
            }
        }
    }

    private bool _showSingleWeightResults;

    public bool ShowSingleWeightResults
    {
        get => _showSingleWeightResults;

        set
        {
            if (_showSingleWeightResults != value)
            {
                _showSingleWeightResults = value;
                OnPropertyChanged();

                if (value)
                {
                    ShowPlatesResults = false;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Error message.
    private string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;

        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
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
        // Accept blank as equal to 0.
        if (MaxWeightText == "")
        {
            MaxWeightText = "0";
        }

        // Check for number greater than 0.
        if (MaxWeight is null or <= 0)
        {
            ErrorMessage = "Please enter maximum weight as a number greater than 0.";
            return false;
        }

        ErrorMessage = "";
        return true;
    }

    private bool ValidateStartingWeight()
    {
        // Accept blank as equal to 0.
        if (StartingWeightText == "")
        {
            StartingWeightText = "0";
        }

        // Check for number greater than or equal to 0.
        if (StartingWeight is null or < 0)
        {
            ErrorMessage = "Please enter starting weight as a number greater than or equal to 0.";
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
                await DoBarbellCalculations();
                break;

            case ExerciseType.Dumbbell:
                await DoDumbbellCalculations();
                break;

            case ExerciseType.Machine:
                await DoMachineCalculations();
                break;

            case ExerciseType.Kettlebell:
                await DoKettlebellCalculations();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(SelectedExerciseType),
                    "Invalid exercise type.");
        }
    }

    private async Task DoBarbellCalculations()
    {
        if (!ValidateMaxWeight())
        {
            return;
        }

        // Get the available plate weights ordered from lightest to heaviest.
        var availPlates = Plates.Values.ToList();

        // Calculate and display the results.
        PlatesResults = PlateSolver.CalculateResults(MaxWeight!.Value, BarWeight, true, availPlates,
            "Plates each end");
        CurrentPlatesResult = PlatesResults[100];
        // VisualStateManager.GoToState(PercentButton100, "Selected");
        ShowPlatesResults = true;
        // await DisplayBarbellResults();
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

    private async Task DoMachineCalculations()
    {
        if (!ValidateMaxWeight() || !ValidateStartingWeight())
        {
            return;
        }

        // Check if they want one side only.
        // bool oneSideOnly = OneSideOnly.IsChecked;

        // Get the available plate weights ordered from lightest to heaviest.
        // var availPlateWeights = Plates.Keys.ToList();

        // Calculate and display the results.
        // PlateResults = PlateSolver.CalculateResults(MaxWeight!.Value, StartingWeight!.Value,
        //     OneSideOnly, availPlateWeights);
        // ShowPlateResults = true;
        // await DisplayMachineResults();
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

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion INotifyPropertyChanged
}
