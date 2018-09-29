using System;
using System.Timers;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs()
        {
            Current = new JobManagerImpl();
        }


        static Timer timer;
        public static void Init(TimeSpan? periodicTime = null)
        {
            var ts = periodicTime ?? TimeSpan.FromMinutes(10);
            timer = new Timer(ts.TotalMilliseconds);
            timer.Elapsed += async (sender, args) =>
            {
                timer.Stop();
                try
                {
                    await Current.RunAll().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                timer.Start();
            };
            timer.Start();
        }
    }
}
