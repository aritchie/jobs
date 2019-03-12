using System;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs()
        {
            Current = new JobManagerImpl();
        }


        public const string BackgroundJobName = nameof(PluginBackgroundTask);
        public static TimeSpan PeriodicRunTime { get; private set; }

        public static void Init(TimeSpan? periodTime = null)
            => PeriodicRunTime = periodTime ?? TimeSpan.FromMinutes(15);
    }
}
