using System;
using System.Threading;
using Android.App;
using Android.App.Job;


namespace Plugin.Jobs
{
    [Service]
    public class PluginJobService : JobService
    {
        readonly CancellationTokenSource cancelSrc = new CancellationTokenSource();


        public override bool OnStartJob(Android.App.Job.JobParameters @params)
        {
            CrossJobs.Current.RunAll(this.cancelSrc.Token).ContinueWith(x => this.JobFinished(@params, true));
            return true;
        }


        public override bool OnStopJob(Android.App.Job.JobParameters @params)
        {
            this.cancelSrc.Cancel();
            return true;
        }
    }
}