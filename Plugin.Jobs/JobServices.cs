using System;
using Plugin.Jobs.Infrastructure;


namespace Plugin.Jobs
{
    public static class JobServices
    {
        public static IJobFactory Factory { get; set; } = new ReflectionJobFactory();
        public static IJobRepository Repository { get; set; } = new SqliteJobRepository();
    }
}
