namespace GymCalc.Utilities;

public static class MauiUtilities
{
    public static T LookupResource<T>(string resourceName, ContentPage page = null)
    {
        // 1. Look in the page.
        if (page != null && page.Resources.TryGetValue(resourceName, out var resource))
        {
            return (T)resource;
        }

        // 2. Look in the app resources. This is where I usually put custom styles.
        var app = Application.Current;
        if (app == null)
        {
            return default(T);
        }
        var appResources = app.Resources;
        if (appResources.TryGetValue(resourceName, out resource))
        {
            return (T)resource;
        }

        // 3. Look in the merged dictionaries (Styles.xml and Colors.xml).
        var resourceDicts = appResources.MergedDictionaries;
        if (resourceDicts is { Count: > 0 })
        {
            foreach (var resourceDict in resourceDicts)
            {
                if (resourceDict.TryGetValue(resourceName, out resource))
                {
                    return (T)resource;
                }
            }
        }

        return default(T);
    }

    public static Color LookupColor(string resourceName, ContentPage page = null)
    {
        return LookupResource<Color>(resourceName, page);
    }

    public static Style LookupStyle(string resourceName, ContentPage page = null)
    {
        return LookupResource<Style>(resourceName, page);
    }

    /// <summary>
    /// Remove all the children from a StackLayout, VerticalStackLayout, or HorizontalStackLayout.
    /// </summary>
    public static void ClearLayout(Layout stack)
    {
        // Remove children.
        while (stack.Children.Count > 0)
        {
            stack.Children.RemoveAt(stack.Children.Count - 1);
        }
    }

    /// <summary>
    /// Remove all the children from a Grid.
    /// If removeCols is true, remove all the column definitions as well.
    /// If removeRows is true, remove all the row definitions as well.
    /// </summary>
    public static void ClearGrid(Grid grid, bool removeCols = false, bool removeRows = false)
    {
        // Remove children.
        ClearLayout(grid);

        // Remove column definitions.
        if (removeCols)
        {
            // Remove column definitions.
            while (grid.ColumnDefinitions.Count > 0)
            {
                grid.ColumnDefinitions.RemoveAt(grid.ColumnDefinitions.Count - 1);
            }
        }

        // Remove row definitions.
        if (removeRows)
        {
            // Remove row definitions.
            while (grid.RowDefinitions.Count > 0)
            {
                grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
            }
        }
    }

    /// <summary>
    /// Get the device width in device-independent units.
    /// </summary>
    /// <returns></returns>
    public static double GetDeviceWidth()
    {
        return DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
    }

    /// <summary>
    /// Get the device height in device-independent units.
    /// </summary>
    /// <returns></returns>
    public static double GetDeviceHeight()
    {
        return DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    }

    /// <summary>
    /// Get the device orientation.
    /// </summary>
    /// <returns></returns>
    public static DisplayOrientation GetOrientation()
    {
        return DeviceDisplay.Current.MainDisplayInfo.Orientation;
    }

    /// <summary>
    /// Get the device platform (e.g. iOS, Android).
    /// </summary>
    /// <returns></returns>
    public static DevicePlatform GetPlatform()
    {
        return DeviceInfo.Current.Platform;
    }
}
