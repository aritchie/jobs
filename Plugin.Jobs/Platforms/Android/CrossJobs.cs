using System;
using System.Linq;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Java.Lang;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static int AndroidJobId { get; set; } = 100;
        public static TimeSpan PeriodicRunTime { get; set; } = TimeSpan.FromMinutes(10);

        static CrossJobs() => Current = new JobManagerImpl();


        public static void Init(Activity activity, Bundle savedInstanceState)
        {
            Xamarin.Essentials.Platform.Init(activity, savedInstanceState);
            StartJobService();
        }


        public static JobScheduler NativeScheduler() => (JobScheduler)Application.Context.GetSystemService(JobService.JobSchedulerService);
        public static Android.App.Job.JobInfo NativeJob => NativeScheduler()
            .AllPendingJobs?
            .FirstOrDefault(x => x.Id == CrossJobs.AndroidJobId);


        public static void StartJobService()
        {
            var sch = NativeScheduler();
            if (!sch.AllPendingJobs.Any(x => x.Id == CrossJobs.AndroidJobId))
            {
                var job = new Android.App.Job.JobInfo.Builder(
                        CrossJobs.AndroidJobId,
                        new ComponentName(
                            Application.Context,
                            Class.FromType(typeof(PluginJobService))
                        )
                    )
                    .SetPeriodic(Convert.ToInt64(CrossJobs.PeriodicRunTime.TotalMilliseconds))
                    .SetPersisted(true)
                    .Build();

                sch.Schedule(job);
            }
        }


        public static void StopJobService() => NativeScheduler().Cancel(CrossJobs.AndroidJobId);
    }
}
