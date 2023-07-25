using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using SQLite;
using CheckBox = InputKit.Shared.Controls.CheckBox;

namespace GymCalc.Pages;

[QueryProperty(nameof(ObjectType), "type")]
[QueryProperty(nameof(ObjectId), "id")]
public partial class EditPage : ContentPage
{
    private string _type;

    public string ObjectType
    {
        get => _type;

        set
        {
            _type = value;
            OnPropertyChanged();
        }
    }

    private int _id;

    public int ObjectId
    {
        get => _id;

        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public EditPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        // The first time the page appears, ObjectType won't be set, so this won't do anything.
        // But also, the first time the page appears, the Loaded event will fire on the EditForm
        // (and other views).
        await InitializeForm();
    }

    private async void EditForm_OnLoaded(object sender, EventArgs e)
    {
        // This event only fires the first time the page appears, so it handles the fact that the
        // first time the page appears, ObjectType isn't known at the time of the OnAppearing()
        // event.
        await InitializeForm();
    }

    /// <summary>
    /// Clear the form.
    /// </summary>
    public void ClearForm()
    {
        WeightEntry.Text = "";
        UnitsRadio.SelectedItem = Units.GetPreferred();
        EnabledCheckBox.IsChecked = true;
        MainColor.Selected = "OffBlack";
        HasBandsCheckBox.IsChecked = false;
        BandColor.Selected = "OffBlack";
    }

    /// <summary>
    /// Set the fields common to all heavy things.
    /// </summary>
    /// <param name="ht"></param>
    public void SetCommonFields(HeavyThing ht)
    {
        WeightEntry.Text = ht.Weight.ToString(CultureInfo.InvariantCulture);
        UnitsRadio.SelectedItem = ht.Units;
        EnabledCheckBox.IsChecked = ht.Enabled;
    }

    /// <summary>
    /// Initialize the form fields.
    /// </summary>
    private async Task InitializeForm()
    {
        // Clear the form.
        ClearForm();

        // If we have an object type, modify the form accordingly. If we have an id, set the values.
        switch (ObjectType)
        {
            case "bar":
                MainColorGrid.IsVisible = false;
                HasBandsGrid.IsVisible = false;
                BandColorGrid.IsVisible = false;

                var bar = await BarRepository.Get(ObjectId);
                if (bar != null)
                {
                    SetCommonFields(bar);
                }
                break;

            case "plate":
                MainColorGrid.IsVisible = true;
                HasBandsGrid.IsVisible = false;
                BandColorGrid.IsVisible = false;

                var plate = await PlateRepository.Get(ObjectId);
                if (plate != null)
                {
                    SetCommonFields(plate);
                    MainColor.Selected = plate.Color;
                }
                break;

            case "dumbbell":
                MainColorGrid.IsVisible = false;
                HasBandsGrid.IsVisible = false;
                BandColorGrid.IsVisible = false;

                var dumbbell = await DumbbellRepository.Get(ObjectId);
                if (dumbbell != null)
                {
                    SetCommonFields(dumbbell);
                }
                break;

            case "kettlebell":
                MainColorGrid.IsVisible = true;
                HasBandsGrid.IsVisible = true;
                BandColorGrid.IsVisible = true;

                var kettlebell = await KettlebellRepository.Get(ObjectId);
                if (kettlebell != null)
                {
                    SetCommonFields(kettlebell);
                    MainColor.Selected = kettlebell.BallColor;
                    HasBandsCheckBox.IsChecked = kettlebell.HasBands;
                    if (kettlebell.HasBands)
                    {
                        BandColor.Selected = kettlebell.BandColor;
                    }
                    else
                    {
                        BandColorGrid.IsVisible = false;
                    }
                }
                else
                {
                    BandColorGrid.IsVisible = false;
                }
                break;
        }
    }

    private async void CancelButton_OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{ObjectType}s");
    }

    private async void SaveButton_OnClicked(object sender, EventArgs e)
    {
        // Validate the form.
        var weightOk = double.TryParse(WeightEntry.Text, out var weight) && weight > 0;
        if (!weightOk)
        {
            ErrorMessage.Text = "Please ensure the weight is a number greater than 0.";
            return;
        }

        var db = Database.GetConnection();

        switch (ObjectType)
        {
            case "bar":
                await SaveBar(weight, db);
                break;

            case "plate":
                await SavePlate(weight, db);
                break;

            case "dumbbell":
                await SaveDumbbell(weight, db);
                break;

            case "kettlebell":
                await SaveKettlebell(weight, db);
                break;
        }
    }

    private async Task SaveBar(double weight, SQLiteAsyncConnection db)
    {
        var bar = new Bar
        {
            Weight = weight,
            Units = (string)UnitsRadio.SelectedItem,
            Enabled = EnabledCheckBox.IsChecked,
        };

        if (ObjectId == 0)
        {
            await db.InsertAsync(bar);
        }
        else
        {
            bar.Id = ObjectId;
            await db.UpdateAsync(bar);
        }

        await Shell.Current.GoToAsync("//bars");
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

        if (ObjectId == 0)
        {
            await db.InsertAsync(plate);
        }
        else
        {
            plate.Id = ObjectId;
            await db.UpdateAsync(plate);
        }

        await Shell.Current.GoToAsync("//plates");
    }

    private async Task SaveDumbbell(double weight, SQLiteAsyncConnection db)
    {
        var dumbbell = new Dumbbell
        {
            Weight = weight,
            Units = (string)UnitsRadio.SelectedItem,
            Enabled = EnabledCheckBox.IsChecked,
        };

        if (ObjectId == 0)
        {
            await db.InsertAsync(dumbbell);
        }
        else
        {
            dumbbell.Id = ObjectId;
            await db.UpdateAsync(dumbbell);
        }

        await Shell.Current.GoToAsync("//dumbbells");
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

        if (ObjectId == 0)
        {
            await db.InsertAsync(kettlebell);
        }
        else
        {
            kettlebell.Id = ObjectId;
            await db.UpdateAsync(kettlebell);
        }

        await Shell.Current.GoToAsync("//kettlebells");
    }

    private void HasBandsCheckBox_OnCheckChanged(object sender, EventArgs e)
    {
        BandColorGrid.IsVisible = ((CheckBox)sender).IsChecked;
    }
}
