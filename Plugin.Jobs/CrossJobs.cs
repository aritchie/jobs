using System;
using Plugin.Jobs.Infrastructure;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
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


        public static IJobRepository Repository { get; set; } = new SqliteJobRepository();
        public static IJobFactory Factory { get; set; } = new ReflectionJobFactory();
    }
}
