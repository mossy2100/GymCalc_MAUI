using GymCalc.Data;

namespace GymCalc;

public partial class App : Application
{
    /// <summary>
    /// The currently selected exercise type on the Calculator page.
    /// </summary>
    internal static ExerciseType SelectedExerciseType = ExerciseType.Barbell;

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
