using System;
using System.Linq;
using UIKit;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs() => Current = new JobManagerImpl();
        public static void Init() => UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);


        public static void OnBackgroundFetch(Action<UIBackgroundFetchResult> completionHandler) => Current
            .RunAll()
            .ContinueWith(x =>
            {
                var results = x.Result;
                if (!results.Any())
                    completionHandler(UIBackgroundFetchResult.NoData);
                else if (results.Any(y => !y.Success))
                    completionHandler(UIBackgroundFetchResult.Failed);
                else
                    completionHandler(UIBackgroundFetchResult.NewData);
            });
    }
}
