using GymCalc.Models;
using GymCalc.Utilities;
using CheckBox = InputKit.Shared.Controls.CheckBox;
using Galaxon.Core.Enums;
using GymCalc.ViewModels;

namespace GymCalc.Pages;

[QueryProperty(nameof(GymObjectTypeName), "type")]
[QueryProperty(nameof(EditMode), "editMode")]
public partial class ListPage : ContentPage
{
    private readonly ListViewModel _model;

    private string _gymObjectTypeName;

    public string GymObjectTypeName
    {
        get => _gymObjectTypeName;

        set
        {
            _model.GymObjectTypeName = value;
            _isGymObjectTypeNameSet = true;
            // _gymObjectTypeName = value;
            // OnPropertyChanged();
        }
    }

    private bool _isGymObjectTypeNameSet;

    private bool _editMode;

    public bool EditMode
    {
        get => _editMode;

        set
        {
            _model.EditMode = value;
            _isEditModeSet = true;
            // _editMode = value;
            // OnPropertyChanged();
        }
    }

    private bool _isEditModeSet;

    // public Label InstructionsLabel { get; set; }

    // private const int _IconButtonWidth = 32;
    //
    // private const int _IconButtonHeight = 32;
    //
    // private const int _IconButtonSpacing = 5;
    //
    // private const int _IconButtonsLayoutWidth = _IconButtonWidth * 2 + _IconButtonSpacing;

    // /// <summary>
    // /// Dictionary mapping checkboxes to gym objects.
    // /// </summary>
    // private readonly Dictionary<CheckBox, GymObject> _editCheckboxObjectMap = new ();
    //
    // /// <summary>
    // /// Dictionary mapping horizontal stacks (with icon buttons) to gym objects.
    // /// </summary>
    // private readonly Dictionary<HorizontalStackLayout, GymObject> _iconButtonsStackObjectMap =
    //     new ();

    public ListPage(ListViewModel listViewModel)
    {
        // Keep references to dependencies.
        _model = listViewModel;

        // Initialize.
        InitializeComponent();
        BindingContext = _model;

        // Event handlers.
        SizeChanged += OnSizeChanged;
    }

    private void EditModeChanged()
    {
        // Make sure both properties have been set.
        if (!_isGymObjectTypeNameSet || !_isEditModeSet)
        {
            return;
        }

        // Get the edit mode from the model.
        var editMode = _model.EditMode;

        // Update the instructions label.
        var text = editMode
            ? "Use the edit and delete icon buttons to make changes."
            : $"Use the checkboxes to select which {GymObjectTypeName.ToLower()} weights ({UnitsUtility.GetDefault().GetDescription()}) are available.";
        InstructionsLabel.Text = text
            + $" Use the Add button to add a new {GymObjectTypeName.ToLower()}, or the Reset button to reset to the defaults.";

        // Hide or show the Add and Reset buttons.
        // AddButton.IsVisible = editMode;
        // ResetButton.IsVisible = editMode;

        // // Hide or show the Edit and Delete buttons.
        // foreach (var (cb, gymObject) in _editCheckboxObjectMap)
        // {
        //     cb.IsVisible = true; //!editMode;
        // }
        // foreach (var (stack, gymObject) in _iconButtonsStackObjectMap)
        // {
        //     stack.IsVisible = true; //editMode;
        // }
    }

    #region Event handlers

    private async void OnSizeChanged(object sender, EventArgs e)
    {
        await _model.DisplayList();
    }

    // /// <inheritdoc />
    // protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    // {
    //     base.OnPropertyChanged(propertyName);
    //
    //     switch (propertyName)
    //     {
    //         case nameof(GymObjectTypeName):
    //             await _model.DisplayList();
    //             _isGymObjectTypeNameSet = true;
    //             break;
    //
    //         case nameof(EditMode):
    //             EditModeCheckBox.IsChecked = EditMode;
    //             EditModeChanged();
    //             _isEditModeSet = true;
    //             break;
    //     }
    // }

    /// <summary>
    /// When the checkbox next to a bar is changed, update the database.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    // private async void EnabledCheckBox_OnCheckChanged(object sender, EventArgs e)
    // {
    //     // Update the bar's Enabled state.
    //     var cb = (CheckBox)sender;
    //     var gymObject = _editCheckboxObjectMap[cb];
    //     gymObject.Enabled = cb.IsChecked;
    //     await _database.Connection.UpdateAsync(gymObject);
    // }

    // private async void EditIcon_OnClicked(object sender, EventArgs e)
    // {
    //     // Get the gym object id and go to the edit form.
    //     var editBtn = (Button)sender;
    //     var stack = (HorizontalStackLayout)editBtn.Parent;
    //     var gymObject = _iconButtonsStackObjectMap[stack];
    //     await Shell.Current.GoToAsync($"edit?type={GymObjectTypeName}&id={gymObject.Id}");
    // }
    //
    // private async void DeleteIcon_OnClicked(object sender, EventArgs e)
    // {
    //     // Get the gym object id and go to the delete confirmation page.
    //     var deleteBtn = (Button)sender;
    //     var stack = (HorizontalStackLayout)deleteBtn.Parent;
    //     var gymObject = _iconButtonsStackObjectMap[stack];
    //     await Shell.Current.GoToAsync($"delete?type={GymObjectTypeName}&id={gymObject.Id}");
    // }

    private void EditModeCheckBox_OnCheckChanged(object sender, EventArgs e)
    {
        EditModeChanged();
    }

    #endregion Event handlers
}
