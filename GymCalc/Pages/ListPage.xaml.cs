using GymCalc.Models;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
public partial class ListPage : ContentPage
{
    #region Fields

    /// <summary>
    /// The gym object type name. This is passed as a parameter to the page and determines what
    /// objects to display in the list.
    /// As this page is a singleton, whenever the page is navigated to, the list is refreshed with
    /// gym objects of the specified type.
    /// </summary>
    private string? _gymObjectTypeName;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="listViewModel">Reference to the viewmodel.</param>
    public ListPage(ListViewModel listViewModel)
    {
        // Keep references to dependencies.
        Model = listViewModel;

        // Initialize.
        InitializeComponent();
        BindingContext = Model;

        // Event handlers.
        SizeChanged += OnSizeChanged;
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Reference to the viewmodel.
    /// </summary>
    public ListViewModel Model { get; }

    /// <summary>
    /// Page parameter specifying what type of gym object to list.
    /// </summary>
    public string? GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _gymObjectTypeName = value;

            // Copy the value to the model.
            Model.GymObjectTypeName = value;
        }
    }

    #endregion Properties

    #region Events

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Model.DisplayList();
    }

    /// <summary>
    /// Re-render the list if the page orientation changes.
    /// </summary>
    /// <param name="sender">The object sending the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void OnSizeChanged(object? sender, EventArgs e)
    {
        await Model.DisplayList();
    }

    /// <summary>
    /// Event handler for when a delete icon button is clicked.
    /// </summary>
    /// <param name="sender">The object sending the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void OnDeleteButtonClicked(object? sender, EventArgs e)
    {
        // Check we have the necessary information.
        if ((sender as Button)!.CommandParameter is not GymObject gymObject
            || GymObjectTypeName == null)
        {
            return;
        }

        // Show the confirmation dialog.
        var msg =
            $"Are you sure you want to delete the {gymObject.Weight} {gymObject.Units} {GymObjectTypeName.ToLower()}?";
        bool confirmed = await DisplayAlert("Please confirm", msg, "OK", "Cancel");

        // If confirmed, do the deletion.
        if (confirmed)
        {
            await Model.DeleteGymObject(gymObject);
        }
    }

    /// <summary>
    /// Event handler for when the Reset button is clicked.
    /// </summary>
    /// <param name="sender">The object sending the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void OnResetButtonClicked(object? sender, EventArgs e)
    {
        // Check we have the necessary information.
        if (GymObjectTypeName == null)
        {
            return;
        }

        // Show the confirmation dialog.
        var msg =
            $"This will remove all {GymObjectTypeName.ToLower()}s from the database and restore the defaults. Are you sure you want to do this?";
        bool confirmed = await DisplayAlert("Please confirm", msg, "OK", "Cancel");

        // If confirmed, do the reset.
        if (confirmed)
        {
            await Model.ResetGymObjects();
        }
    }

    #endregion Events
}
