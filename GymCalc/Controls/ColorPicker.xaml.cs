using GymCalc.Graphics;
using GymCalc.Utilities;

namespace GymCalc.Controls;

public partial class ColorPicker : ContentView
{
    public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
        nameof(Selected), typeof(string), typeof(ColorPicker), "", BindingMode.TwoWay, null,
        OnSelectedChanged);

    public string Selected
    {
        get => (string)GetValue(SelectedProperty);

        set => SetValue(SelectedProperty, value);
    }

    /// <summary>
    /// Dictionary mapping buttons to color names.
    /// </summary>
    private readonly Dictionary<Button, string> _buttonToColor = new ();

    internal ColorPicker()
    {
        InitializeComponent();
        ConstructGrid();
    }

    private void ConstructGrid()
    {
        // Lookup styles.
        var frameStyle = MauiUtilities.LookupStyle("ColorPickerFrameStyle");
        var buttonStyle = MauiUtilities.LookupStyle("ColorPickerButtonStyle");

        // Display the color buttons in the grid.
        const int N_ROWS = 4;
        const int N_COLS = 4;
        var r = 0;
        var c = 0;
        foreach (var (colorName, colorHex) in CustomColors.Palette)
        {
            // Create a frame.
            var frame = new Frame
            {
                Style = frameStyle,
            };

            // Create a button.
            var button = new Button
            {
                Style = buttonStyle,
                BackgroundColor = CustomColors.Get(colorName),
            };

            // Attach the event handler.
            button.Clicked += ColorPickerButton_OnClicked;

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

    private void ColorPickerButton_OnClicked(object sender, EventArgs e)
    {
        // Update the Selected property to the matching color name.
        Selected = _buttonToColor[(Button)sender];
    }

    private static void OnSelectedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sOldValue = (string)oldValue;
        var sNewValue = (string)newValue;
        var colorPicker = (ColorPicker)bindable;

        // Deselect the old selected button, if there is one.
        var oldButton = colorPicker._buttonToColor
            .FirstOrDefault(pair => pair.Value == sOldValue)
            .Key;
        if (oldButton != null)
        {
            ((Frame)oldButton.Parent).BackgroundColor = Colors.Transparent;
        }

        // Select the button that was clicked.
        var newButton = colorPicker._buttonToColor
            .FirstOrDefault(pair => pair.Value == sNewValue)
            .Key;
        if (newButton != null)
        {
            ((Frame)newButton.Parent).BackgroundColor = MauiUtilities.LookupColor("Primary");
        }
    }
}
