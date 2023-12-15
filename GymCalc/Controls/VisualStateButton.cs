namespace GymCalc.Controls;

public class VisualStateButton : Button
{
    public static readonly BindableProperty VisualStateProperty = BindableProperty.Create(
        nameof(VisualState), typeof(string), typeof(VisualStateButton),
        VisualStateManager.CommonStates.Normal, BindingMode.TwoWay, null, OnVisualStateChanged);

    public string VisualState
    {
        get => (string)GetValue(VisualStateProperty);

        set => SetValue(VisualStateProperty, value);
    }

    private static void OnVisualStateChanged(BindableObject bindable, object oldValue,
        object newValue)
    {
        VisualStateManager.GoToState((VisualStateButton)bindable, (string)newValue);
    }
}
