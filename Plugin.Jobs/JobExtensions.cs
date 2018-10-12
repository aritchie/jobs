using System;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Plugin.Jobs
{
    public static class JobExtensions
    {
        public static Task ScheduleLogTrimmingJob(this IJobManager jobManager) => jobManager.ScheduleLogTrimmingJob(TimeSpan.FromSeconds(30));
        public static Task ScheduleLogTrimmingJob(this IJobManager jobManager, TimeSpan maxAge) => LogTrimmingJob.Schedule(jobManager, maxAge);


        public static T GetValue<T>(this JobInfo job, string key, T defaultValue = default(T))
        {
            if (!job.Parameters.ContainsKey(key))
                return defaultValue;

            var value = job.Parameters[key];
            if (value is string s && typeof(T) != typeof(string))
                return JsonConvert.DeserializeObject<T>(s);

            if (value.GetType() == typeof(T))
                return (T)value;

            return (T) Convert.ChangeType(value, typeof(T));
        }


        public static void SetValue(this JobInfo job, string key, object value)
        {
            job.Parameters[key] = value;
        }
    }
}
