using System;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static bool IsLoggingEnabled { get; set; } = true;


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
