using System;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Java.Lang;
using Plugin.Jobs.Platforms.Android;

[assembly: UsesPermission("android.permission.BIND_JOB_SERVICE")]
[assembly: UsesPermission("android.permission.RECEIVE_BOOT_COMPLETED")]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.BatteryStats)]

//https://developer.android.com/guide/background/

namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static void Init(Activity activity, Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(activity, bundle);
            Init();
        }


        public static void Init(Application application)
        {
            Xamarin.Essentials.Platform.Init(application);
            Init();
        }


        static void Init()
        {
            Current = new JobManagerImpl();
            //    .SetRequiresCharging(true)
            //    .SetRequiresBatteryNotLow(true)
            //    .SetRequiredNetworkType(NetworkType.Any)
            //    .SetRequiredNetworkType(NetworkType.Unmetered)
            //    .SetExtras(params)
            var jobScheduler = (JobScheduler) Application.Context.GetSystemService(Context.JobSchedulerService);
            var job = new Android.App.Job.JobInfo.Builder(
                    0,
                    new ComponentName(
                        Application.Context,
                        Class.FromType(typeof(PluginJobService))
                    )
                )
                .SetPeriodic(0, 0)
                //.SetPersisted(true)
                .Build();

            jobScheduler.Schedule(job);
        }
    }
}
