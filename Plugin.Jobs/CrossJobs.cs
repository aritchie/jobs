using System;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs()
        { 
            ResolveJob = (jobInfo) => 
            {
                if (jobInfo.Type == null)
                    throw new ArgumentException($"Job '{jobInfo.Name}' class type not found");

                var job = Activator.CreateInstance(jobInfo.Type) as IJob;
                if (job == null)
                    throw new ArgumentException("Type is not IJob or was not found");

                return job;
            };
        }


        public static bool IsLoggingEnabled { get; set; } = true;
        public static Func<JobInfo, IJob> ResolveJob { get; set; }

        static IJobManager current;
        public static IJobManager Current
        {
            get
            {
                if (current == null)
                    throw new ArgumentException("[Plugin.Jobs] No platform plugin found.  Did you install the nuget package in your app project as well?");

                return current;
            }
            set => current = value;
        }
    }
}
