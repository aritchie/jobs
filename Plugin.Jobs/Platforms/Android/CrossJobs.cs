using System;
using Android.App;
using Android.OS;

[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.BatteryStats)]

//https://developer.android.com/guide/background/

namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static void Init(Activity activity, Bundle bundle)
        {
            Current = new JobManagerImpl();
            Xamarin.Essentials.Platform.Init(activity, bundle);
        }


        public static void Init(Application application)
        {
            Current = new JobManagerImpl();
            Xamarin.Essentials.Platform.Init(application);
        }
    }
}
