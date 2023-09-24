namespace GymCalc.Controls;

public class VisualStateButton : Button
{
    private static readonly BindableProperty _VisualStateProperty = BindableProperty.Create(
        nameof(VisualState), typeof(string), typeof(VisualStateButton),
        VisualStateManager.CommonStates.Normal, propertyChanged: OnVisualStateChanged);

    public string VisualState
    {
        get => (string)GetValue(_VisualStateProperty);

        set => SetValue(_VisualStateProperty, value);
    }

    private static void OnVisualStateChanged(BindableObject bindable, object oldValue,
        object newValue)
    {
        VisualStateManager.GoToState((VisualStateButton)bindable, (string)newValue);
    }
}
