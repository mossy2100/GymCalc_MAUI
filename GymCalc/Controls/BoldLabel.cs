namespace GymCalc.Controls;

/// <summary>
/// This custom control is a solution to the problem of Labels on iOS ignoring the FontAttributes
/// property. If Microsoft fix this bug, we won't need this control anymore. We'll be able to just
/// use a regular Label with FontAttributes=Bold.
/// TODO Add a XML file with the markup for this control instead of creating everything dynamically.
/// </summary>
public class BoldLabel : Label
{
    public new static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(BoldLabel), propertyChanged: OnTextChanged);

    public new string Text
    {
        get => (string)GetValue(TextProperty);

        set => SetValue(TextProperty, value);
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not BoldLabel bl)
        {
            return;
        }

        // Get the old and new values as strings.
        string oldText = oldValue as string ?? "";
        string newText = newValue as string ?? "";

        // Check for change.
        if (oldText == newText)
        {
            return;
        }

        // If no spans have been added yet, add the single bold span.
        if (bl.FormattedText == null)
        {
            bl.FormattedText = new FormattedString();
        }
        if (bl.FormattedText.Spans.Count == 0)
        {
            bl.FormattedText.Spans.Add(new Span { FontAttributes = FontAttributes.Bold });
        }

        // Update the span text.
        bl.FormattedText.Spans[0].Text = newText;
    }
}
