using System;
using System.Linq;
using UIKit;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        static CrossJobs() => Current = new JobManagerImpl();


        /// <summary>
        /// You must call this on iOS to set background fetch interval
        /// </summary>
        /// <param name="mininmumTime">If null is passed, BackgroundFetchIntervalMinimum is used - you should not change this unless you truly want to take measures into your own hands</param>
        public static void Init(double? minimumTime = null)
        {
            if (minimumTime == null)
                UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            else
                UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(minimumTime.Value);
        }


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
