using System;
using UIKit;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static void Init()
        {
            Current = new JobManagerImpl();
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(900); // TODO
        }


        public static async void OnBackgroundFetch(Action<UIBackgroundFetchResult> completionHandler)
        {
            await Current.Run().ConfigureAwait(false);
            completionHandler(UIBackgroundFetchResult.NewData);
        }
    }
}
