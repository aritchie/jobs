using System;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs()
        {
            Current = new JobManagerImpl();
        }
    }
}
