using System;
using Android.App;
using Android.App.Job;
using Android.Content;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl()
        {
            // TODO: register job to run periodically for all jobs?  what about network checks
            var jobScheduler = (JobScheduler) Application.Context.GetSystemService(Context.JobSchedulerService);
            //var job = new JobInfo.Builder(0, ComponentName.CreateRelative(""))
            //    .SetPeriodic(0, 0)
            //    .SetRequiresCharging(true)
            //    .SetRequiresBatteryNotLow(true)
            //    .SetRequiredNetworkType(NetworkType.Any)
            //    .SetRequiredNetworkType(NetworkType.Unmetered)
            //    .SetPersisted(true)
            //    //.SetExtras(params)
        }
    }
}
