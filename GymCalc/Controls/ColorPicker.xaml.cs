using Galaxon.Maui.Utilities;
using GymCalc.Graphics;

namespace GymCalc.Controls;

public partial class ColorPicker : ContentView
{
    public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
        nameof(Selected), typeof(string), typeof(ColorPicker), "", BindingMode.TwoWay, null,
        OnSelectedChanged);

    /// <summary>
    /// Dictionary mapping buttons to color names.
    /// </summary>
    private readonly Dictionary<Button, string> _buttonToColor = new ();

    /// <summary>
    /// Constructor.
    /// </summary>
    public ColorPicker()
    {
        InitializeComponent();
        ConstructGrid();
    }

    public string Selected
    {
        get => (string)GetValue(SelectedProperty);

        set => SetValue(SelectedProperty, value);
    }

    private void ConstructGrid()
    {
        // Lookup styles.
        Style? frameStyle = MauiUtility.LookupStyle("ColorPickerFrameStyle");
        Style? buttonStyle = MauiUtility.LookupStyle("ColorPickerButtonStyle");

        // Display the color buttons in the grid.
        const int N_ROWS = 4;
        const int N_COLS = 4;
        int r = 0;
        int c = 0;
        foreach ((string colorName, string colorHex) in Palette.Colors)
        {
            // Create a frame.
            var frame = new Frame
            {
                Style = frameStyle
            };

            // Create a button.
            var button = new Button
            {
                Style = buttonStyle,
                BackgroundColor = Palette.Get(colorName)
            };

            // Attach the event handler.
            button.Clicked += OnColorPickerButtonClicked;

            // Add it to the dictionary for easy lookup later.
            _buttonToColor[button] = colorName;

            // Put the button inside the frame.
            frame.Content = button;

            // Add the frame to the grid.
            ColorPickerGrid.Add(frame, c, r);

            // Go to next position.
            c++;
            if (c == N_COLS)
            {
                c = 0;
                r++;

                // Check if done.
                if (r == N_ROWS)
                {
                    break;
                }
            }
        }
    }

    private void OnColorPickerButtonClicked(object? sender, EventArgs e)
    {
        // Update the Selected property to the matching color name.
        if (sender is Button btn)
        {
            Selected = _buttonToColor[btn];
        }
    }

    private static void OnSelectedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sOldValue = (string)oldValue;
        var sNewValue = (string)newValue;
        var colorPicker = (ColorPicker)bindable;

        // Deselect the old selected button, if there is one.
        Button? oldButton = colorPicker._buttonToColor
            .FirstOrDefault(pair => pair.Value == sOldValue)
            .Key;
        if (oldButton != null)
        {
            VisualStateManager.GoToState((Frame)oldButton.Parent, "Normal");
        }

        // Select the button that was clicked.
        Button? newButton = colorPicker._buttonToColor
            .FirstOrDefault(pair => pair.Value == sNewValue)
            .Key;
        if (newButton != null)
        {
            VisualStateManager.GoToState((Frame)newButton.Parent, "Selected");
        }
    }
}
