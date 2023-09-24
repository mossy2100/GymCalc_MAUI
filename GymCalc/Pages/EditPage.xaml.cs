using System.Globalization;
using System.Runtime.CompilerServices;
using Galaxon.Core.Enums;
using GymCalc.Data;
using GymCalc.Models;
using GymCalc.Constants;
using GymCalc.Utilities;
using SQLite;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(GymObjectId), "id")]
public partial class EditPage : ContentPage
{
    private readonly Database _database;

    private readonly BarRepository _barRepo;

    private readonly PlateRepository _plateRepo;

    private readonly DumbbellRepository _dbRepo;

    private readonly KettlebellRepository _kbRepo;

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

    public EditPage(Database database, BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        _database = database;
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;

        InitializeComponent();
        BindingContext = this;

        // Workaround for issue with Back button label.
        // <see href="https://github.com/dotnet/maui/issues/8335" />
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    /// <inheritdoc />
    protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(GymObjectTypeName):
                SetTitle();
                InitializeForm();
                break;

            case nameof(GymObjectId):
                SetTitle();
                await PopulateForm();
                break;
        }
    }

    private void SetTitle()
    {
        var verb = GymObjectId == 0 ? "Add" : "Edit";
        Title = $"{verb} {GymObjectTypeName}";
    }

    /// <summary>
    /// Clear the form.
    /// </summary>
    private void ClearForm()
    {
        WeightEntry.Text = "";
        UnitsRadio.SelectedItem = UnitsUtility.GetDefault().GetDescription();
        EnabledCheckBox.IsChecked = true;
        MainColor.Selected = "OffBlack";
        HasBandsCheckBox.IsChecked = false;
        BandColor.Selected = "OffBlack";
    }

    /// <summary>
    /// Set the fields common to all gym objects.
    /// </summary>
    /// <param name="ht"></param>
    private void SetCommonFields(GymObject ht)
    {
        WeightEntry.Text = ht.Weight.ToString(CultureInfo.InvariantCulture);
        UnitsRadio.SelectedItem = ht.Units;
        EnabledCheckBox.IsChecked = ht.Enabled;
    }

    /// <summary>
    /// Hide and show the form fields appropriate to this object type.
    /// </summary>
    private void InitializeForm()
    {
        // Clear the form.
        ClearForm();

        // Hide extended fields, as they mostly aren't needed.
        MainColorGrid.IsVisible = false;
        HasBandsGrid.IsVisible = false;
        BandColorGrid.IsVisible = false;

        // If we have an object type, modify the form accordingly.
        // If we don't, that's an error.
        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                break;

            case GymObjectType.Plate:
                MainColorGrid.IsVisible = true;
                break;

            case GymObjectType.Dumbbell:
                MainColorGrid.IsVisible = true;
                break;

            case GymObjectType.Kettlebell:
                MainColorGrid.IsVisible = true;
                HasBandsGrid.IsVisible = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(GymObjectTypeName), "Invalid object type.");
        }
    }

    /// <summary>
    /// Copy the object values into the form fields.
    /// </summary>
    private async Task PopulateForm()
    {
        // If we have an id, set the values.
        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                var bar = await _barRepo.Get(GymObjectId);
                if (bar != null)
                {
                    SetCommonFields(bar);
                }
                break;

            case GymObjectType.Plate:
                var plate = await _plateRepo.Get(GymObjectId);
                if (plate != null)
                {
                    SetCommonFields(plate);
                    MainColor.Selected = plate.Color;
                }
                break;

            case "Dumbbell":
                var dumbbell = await _dbRepo.Get(GymObjectId);
                if (dumbbell != null)
                {
                    SetCommonFields(dumbbell);
                    MainColor.Selected = dumbbell.Color;
                }
                break;

            case GymObjectType.Kettlebell:
                var kettlebell = await _kbRepo.Get(GymObjectId);
                if (kettlebell != null)
                {
                    SetCommonFields(kettlebell);
                    MainColor.Selected = kettlebell.BallColor;
                    HasBandsCheckBox.IsChecked = kettlebell.HasBands;
                    if (kettlebell.HasBands)
                    {
                        BandColor.Selected = kettlebell.BandColor;
                        BandColorGrid.IsVisible = true;
                    }
                }
                break;
        }
    }

    private async void CancelButton_OnClicked(object sender, EventArgs e)
    {
        await AppShell.GoToList(GymObjectTypeName, true);
    }

    private void ClearErrorMessage()
    {
        ErrorMessage.Text = "";
        ErrorMessage.IsVisible = false;
    }

    private void SetErrorMessage(string errorMessage)
    {
        ErrorMessage.Text = errorMessage;
        ErrorMessage.IsVisible = true;
    }

    private async void SaveButton_OnClicked(object sender, EventArgs e)
    {
        // Validate the form.
        var weightOk = double.TryParse(WeightEntry.Text, out var weight) && weight > 0;
        if (!weightOk)
        {
            SetErrorMessage("Please ensure the weight is a number greater than 0.");
            return;
        }
        ClearErrorMessage();

        var conn = _database.Connection;

        switch (GymObjectTypeName)
        {
            case GymObjectType.Bar:
                await SaveBar(weight, conn);
                break;

            case GymObjectType.Plate:
                await SavePlate(weight, conn);
                break;

            case GymObjectType.Dumbbell:
                await SaveDumbbell(weight, conn);
                break;

            case GymObjectType.Kettlebell:
                await SaveKettlebell(weight, conn);
                break;
        }

        await AppShell.GoToList(GymObjectTypeName, true);
    }

    private async Task SaveBar(double weight, SQLiteAsyncConnection db)
    {
        var bar = new Bar
        {
            Weight = weight,
            Units = (string)UnitsRadio.SelectedItem,
            Enabled = EnabledCheckBox.IsChecked,
        };

        if (GymObjectId == 0)
        {
            await db.InsertAsync(bar);
        }
        else
        {
            bar.Id = GymObjectId;
            await db.UpdateAsync(bar);
        }
    }

    private async Task SavePlate(double weight, SQLiteAsyncConnection db)
    {
        var plate = new Plate
        {
            Weight = weight,
            Units = (string)UnitsRadio.SelectedItem,
            Enabled = EnabledCheckBox.IsChecked,
            Color = MainColor.Selected,
        };

        if (GymObjectId == 0)
        {
            await db.InsertAsync(plate);
        }
        else
        {
            plate.Id = GymObjectId;
            await db.UpdateAsync(plate);
        }
    }

    private async Task SaveDumbbell(double weight, SQLiteAsyncConnection db)
    {
        var dumbbell = new Dumbbell
        {
            Weight = weight,
            Units = (string)UnitsRadio.SelectedItem,
            Enabled = EnabledCheckBox.IsChecked,
            Color = MainColor.Selected,
        };

        if (GymObjectId == 0)
        {
            await db.InsertAsync(dumbbell);
        }
        else
        {
            dumbbell.Id = GymObjectId;
            await db.UpdateAsync(dumbbell);
        }
    }

    private async Task SaveKettlebell(double weight, SQLiteAsyncConnection db)
    {
        var kettlebell = new Kettlebell
        {
            Weight = weight,
            Units = (string)UnitsRadio.SelectedItem,
            Enabled = EnabledCheckBox.IsChecked,
            BallColor = MainColor.Selected,
            HasBands = HasBandsCheckBox.IsChecked,
            BandColor = BandColor.Selected,
        };

        if (GymObjectId == 0)
        {
            await db.InsertAsync(kettlebell);
        }
        else
        {
            kettlebell.Id = GymObjectId;
            await db.UpdateAsync(kettlebell);
        }
    }

    private void HasBandsCheckBox_OnCheckChanged(object sender, EventArgs e)
    {
        BandColorGrid.IsVisible = ((CheckBox)sender).IsChecked;
    }
}
