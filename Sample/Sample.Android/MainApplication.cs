using System;
using Android.App;
using Android.Runtime;
using Plugin.Jobs;


namespace Sample.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }


        public override void OnCreate()
        {
            CrossJobs.ResolveJob = (jobInfo) => App.ResolveJob(jobInfo);
            CrossJobs.Init(this);
            base.OnCreate();
        }
    }
}