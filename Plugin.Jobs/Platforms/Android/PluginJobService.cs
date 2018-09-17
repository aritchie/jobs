using System;
using Android.App;
using Android.App.Job;


namespace Plugin.Jobs
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE")]
    public class PluginJobService : JobService
    {
        public override bool OnStartJob(Android.App.Job.JobParameters @params)
        {
            CrossJobs.Current.Run().ContinueWith(x => this.JobFinished(@params, true));
            return true;
        }


        public override bool OnStopJob(Android.App.Job.JobParameters @params) => true; // reschedule?
    }
}