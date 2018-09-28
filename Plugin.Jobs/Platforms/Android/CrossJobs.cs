using System;
using System.Linq;
using Android.App;
using Android.App.Job;
using Android.Content;
using Java.Lang;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static void Init(Application application)
        {
            Xamarin.Essentials.Platform.Init(application);
            Current = new JobManagerImpl();
            //    .SetRequiresCharging(true)
            //    .SetRequiresBatteryNotLow(true)
            //    .SetRequiredNetworkType(NetworkType.Any)
            //    .SetRequiredNetworkType(NetworkType.Unmetered)
            //    .SetExtras(params)
            var jobScheduler = (JobScheduler) Application.Context.GetSystemService(Context.JobSchedulerService);
            if (jobScheduler.AllPendingJobs.Any(x => x.Id == 100))
                return;

            var job = new Android.App.Job.JobInfo.Builder(
                    100,
                    new ComponentName(
                        context,
                        Class.FromType(typeof(PluginJobService))
                    )
                )
                .SetPeriodic(Convert.ToInt64(TimeSpan.FromMinutes(10).TotalMilliseconds))
                .SetPersisted(true)
                .Build();

            jobScheduler.Schedule(job);
        }
    }
}
