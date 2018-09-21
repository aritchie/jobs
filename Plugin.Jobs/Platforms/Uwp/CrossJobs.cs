using System;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public const string BackgroundJobName = nameof(PluginBackgroundTask);


        public static void Init()
        {
            Current = new JobManagerImpl();
        }
    }
}
