using System.Globalization;
using System.Runtime.CompilerServices;
using Galaxon.Core.Enums;
using GymCalc.Data;
using GymCalc.Models;
using GymCalc.Constants;
using GymCalc.Utilities;
using GymCalc.ViewModels;
using SQLite;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

[QueryProperty(nameof(Operation), "op")]
[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class EditPage : ContentPage
{
    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Reference to the view model.
    /// </summary>
    private EditViewModel _model;

    // ---------------------------------------------------------------------------------------------
    // Page parameters.

    private string _operation;

    public string Operation
    {
        get => _operation;

        set
        {
            _operation = value;
            OnPropertyChanged();
        }
    }

    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _gymObjectTypeName = value;
            OnPropertyChanged();
        }
    }

    private int _gymObjectId;

    public int GymObjectId
    {
        get => _gymObjectId;

        set
        {
            _gymObjectId = value;
            OnPropertyChanged();
        }
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="model"></param>
    public EditPage(EditViewModel model)
    {
        InitializeComponent();
        _model = model;
        BindingContext = _model;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    // ---------------------------------------------------------------------------------------------
    // Event handlers.

    /// <inheritdoc />
    protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Operation):
            case nameof(GymObjectTypeName):
            case nameof(GymObjectId):
                // SetTitle();
                await _model.Initialize(Operation, GymObjectTypeName, GymObjectId);
                break;
                //
                // // SetTitle();
                // await _model.Initialize(Operation, GymObjectTypeName, GymObjectId);
                // break;
                //
                // // SetTitle();
                // await _model.Initialize(Operation, GymObjectTypeName, GymObjectId);
                // // await PopulateForm();
                // break;
        }
    }

    // ---------------------------------------------------------------------------------------------

    // private void SetTitle()
    // {
    //     var verb = GymObjectId == 0 ? "Add" : "Edit";
    //     Title = $"{verb} {GymObjectTypeName}";
    // }

    // /// <summary>
    // /// Clear the form.
    // /// </summary>
    // private void ClearForm()
    // {
    //     WeightEntry.Text = "";
    //     UnitsRadio.SelectedItem = UnitsUtility.GetDefault().GetDescription();
    //     EnabledCheckBox.IsChecked = true;
    //     MainColor.Selected = "OffBlack";
    //     HasBandsCheckBox.IsChecked = false;
    //     BandColor.Selected = "OffBlack";
    // }

    // /// <summary>
    // /// Set the fields common to all gym objects.
    // /// </summary>
    // /// <param name="ht"></param>
    // private void SetCommonFields(GymObject ht)
    // {
    //     WeightEntry.Text = ht.Weight.ToString(CultureInfo.InvariantCulture);
    //     UnitsRadio.SelectedItem = ht.Units;
    //     EnabledCheckBox.IsChecked = ht.Enabled;
    // }

    // /// <summary>
    // /// Copy the object values into the form fields.
    // /// </summary>
    // private async Task PopulateForm()
    // {
    //     // If we have an id, set the values.
    //     switch (GymObjectTypeName)
    //     {
    //         case GymObjectType.Bar:
    //             var bar = await _barRepo.Get(GymObjectId);
    //             if (bar != null)
    //             {
    //                 SetCommonFields(bar);
    //             }
    //             break;
    //
    //         case GymObjectType.Plate:
    //             var plate = await _plateRepo.Get(GymObjectId);
    //             if (plate != null)
    //             {
    //                 SetCommonFields(plate);
    //                 MainColor.Selected = plate.Color;
    //             }
    //             break;
    //
    //         case "Dumbbell":
    //             var dumbbell = await _dumbbellRepo.Get(GymObjectId);
    //             if (dumbbell != null)
    //             {
    //                 SetCommonFields(dumbbell);
    //                 MainColor.Selected = dumbbell.Color;
    //             }
    //             break;
    //
    //         case GymObjectType.Kettlebell:
    //             var kettlebell = await _kettlebellRepo.Get(GymObjectId);
    //             if (kettlebell != null)
    //             {
    //                 SetCommonFields(kettlebell);
    //                 MainColor.Selected = kettlebell.BallColor;
    //                 HasBandsCheckBox.IsChecked = kettlebell.HasBands;
    //                 if (kettlebell.HasBands)
    //                 {
    //                     BandColor.Selected = kettlebell.BandColor;
    //                     BandColorGrid.IsVisible = true;
    //                 }
    //             }
    //             break;
    //     }
    // }

    // private async void CancelButton_OnClicked(object sender, EventArgs e)
    // {
    //     await AppShell.GoToList(GymObjectTypeName);
    // }

    // private void ClearErrorMessage()
    // {
    //     ErrorMessage.Text = "";
    //     ErrorMessage.IsVisible = false;
    // }
    //
    // private void SetErrorMessage(string errorMessage)
    // {
    //     ErrorMessage.Text = errorMessage;
    //     ErrorMessage.IsVisible = true;
    // }

    // private void HasBandsCheckBox_OnCheckChanged(object sender, EventArgs e)
    // {
    //     BandColorGrid.IsVisible = ((CheckBox)sender).IsChecked;
    // }
}
