using System;
using Android.App;
using Android.OS;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs() => Current = new JobManagerImpl();
        public static void Init(Activity activity, Bundle savedInstanceState)
            => Xamarin.Essentials.Platform.Init(activity, savedInstanceState);


        public static int AndroidJobId { get; set; } = 100;
        public static TimeSpan PeriodicRunTime { get; set; } = TimeSpan.FromMinutes(10);
    }
}
