﻿using System;
using System.Threading;
using Android.App;
using Android.App.Job;


namespace Plugin.Jobs
{
    [Service(
        Name = "com.acrapps.JobsPluginService",
        Permission = "android.permission.BIND_JOB_SERVICE",
        Exported = true
    )]
    public class PluginJobService : JobService
    {
        CancellationTokenSource cancelSrc;


        public override bool OnStartJob(JobParameters @params)
        {
            this.cancelSrc = new CancellationTokenSource();
            CrossJobs
                .Current
                .RunAll(this.cancelSrc.Token)
                .ContinueWith(x => this.JobFinished(@params, false));
            return true;
        }


        public override bool OnStopJob(JobParameters @params)
        {
            this.cancelSrc?.Cancel();
            return true;
        }
    }
}