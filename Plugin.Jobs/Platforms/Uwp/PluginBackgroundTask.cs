using System;
using Windows.ApplicationModel.Background;


namespace Plugin.Jobs
{
    public class PluginBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            using (var cancelSrc = new CancellationTokenSource())
            {
                taskInstance.Canceled += (sender, reason) => cancelSrc.Cancel();
                await CrossJobs.Current.RunAll(cancelSrc.Token);
                taskInstance.GetDeferral().Complete();
            }
        }
    }
}
