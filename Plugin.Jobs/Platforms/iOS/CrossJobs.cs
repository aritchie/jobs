using System;
using System.Threading;
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
            var result = UIBackgroundFetchResult.NoData;
            var app = UIApplication.SharedApplication;
            var taskId = 0;

            try
            {
                using (var cancelSrc = new CancellationTokenSource())
                {
                    taskId = (int)app.BeginBackgroundTask("RunAll", cancelSrc.Cancel);
                    await Current
                        .RunAll(cancelSrc.Token)
                        .ConfigureAwait(false);

                    result = UIBackgroundFetchResult.NewData;
                }
            }
            catch
            {
                result = UIBackgroundFetchResult.Failed;
            }
            finally
            {
                completionHandler(result);
                app.EndBackgroundTask(taskId);
            }
        }
    }
}
