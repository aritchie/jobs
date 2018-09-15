using System;
using Windows.ApplicationModel.Background;


namespace Plugin.Jobs
{
    public class PluginBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            await CrossJobs.Current.Run();
            taskInstance.GetDeferral().Complete();
        }
    }
}
