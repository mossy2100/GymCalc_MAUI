#if ANDROID
using Android.Content;
using Android.Runtime;
using Android.Views;
#elif IOS
using UIKit;
#endif

using GymCalc.Constants;

namespace GymCalc.Services;

public class DeviceOrientationService
{
    public DeviceOrientation GetOrientation()
    {
#if ANDROID
        IWindowManager? windowManager = Android.App.Application.Context
            .GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
        SurfaceOrientation? orientation = windowManager?.DefaultDisplay?.Rotation;
        if (orientation == null)
        {
            return DeviceOrientation.Undefined;
        }
        bool isLandscape =
            orientation is SurfaceOrientation.Rotation90 or SurfaceOrientation.Rotation270;
        return isLandscape ? DeviceOrientation.Landscape : DeviceOrientation.Portrait;
#elif IOS
        UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
        bool isPortrait = orientation is UIInterfaceOrientation.Portrait
            or UIInterfaceOrientation.PortraitUpsideDown;
        return isPortrait ? DeviceOrientation.Portrait : DeviceOrientation.Landscape;
#endif
    }
}
