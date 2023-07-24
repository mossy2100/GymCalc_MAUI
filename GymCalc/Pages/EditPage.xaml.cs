using System.Globalization;
using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;
using SQLite;

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

    public void SetFormFields(HeavyThing ht)
    {
        if (ht == null)
        {
            // Set form fields to defaults.
            WeightEntry.Text = "";
            UnitsRadio.SelectedItem = Units.GetPreferred();
            EnabledCheckBox.IsChecked = true;
        }
        else
        {
            // Set form fields from object.
            WeightEntry.Text = ht.Weight.ToString(CultureInfo.InvariantCulture);
            UnitsRadio.SelectedItem = ht.Units;
            EnabledCheckBox.IsChecked = ht.Enabled;
        }
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
    /// Initialize the form fields.
    /// </summary>
    private async Task InitializeForm()
    {
        switch (ObjectType)
        {
            case "bar":
                var bar = await BarRepository.Get(ObjectId);
                SetFormFields(bar);
                ColorGrid.IsVisible = false;
                HasBandsGrid.IsVisible = false;
                BandColorGrid.IsVisible = false;
                break;

            case "plate":
                var plate = await PlateRepository.Get(ObjectId);
                SetFormFields(plate);
                ColorGrid.IsVisible = true;
                HasBandsGrid.IsVisible = false;
                BandColorGrid.IsVisible = false;
                break;

            case "dumbbell":
                var dumbbell = await DumbbellRepository.Get(ObjectId);
                SetFormFields(dumbbell);
                ColorGrid.IsVisible = false;
                HasBandsGrid.IsVisible = false;
                BandColorGrid.IsVisible = false;
                break;

            case "kettlebell":
                var kettlebell = await KettlebellRepository.Get(ObjectId);
                SetFormFields(kettlebell);
                ColorGrid.IsVisible = true;
                HasBandsGrid.IsVisible = true;
                BandColorGrid.IsVisible = kettlebell is { HasBands: true };
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
            // Color =
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
            // BallColor =
            HasBands = HasBandsCheckBox.IsChecked,
            // BandColor =
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
}
