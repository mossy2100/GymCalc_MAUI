namespace GymCalc.Utilities;

public static class MauiUtilities
{
    internal static T LookupResource<T>(string resourceName, ContentPage page = null)
    {
        object resource;

        // 1. Look in the page.
        if (page != null && page.Resources.TryGetValue(resourceName, out resource))
        {
            return (T)resource;
        }

        // 2. Look in the app resources. This is where I usually put custom styles.
        var app = App.Current;
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
}
