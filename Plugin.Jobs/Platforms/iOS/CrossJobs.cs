using System;
using System.Linq;
using UIKit;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs() => Current = new JobManagerImpl();
        public static void Init() => UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);


        public async static void OnBackgroundFetch(Action<UIBackgroundFetchResult> completionHandler)
        {
            var results = await Current.RunAll().ConfigureAwait(false);
            if (results == null || !results.Any())
                completionHandler(UIBackgroundFetchResult.NoData);
            else if (results.Any(y => !y.Success))
                completionHandler(UIBackgroundFetchResult.Failed);
            else
                completionHandler(UIBackgroundFetchResult.NewData);
        }
    }
}
