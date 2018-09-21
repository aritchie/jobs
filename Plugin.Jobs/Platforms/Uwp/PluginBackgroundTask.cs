using System;
using Windows.ApplicationModel.Background;


namespace Plugin.Jobs
{
    public class PluginBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //taskInstance.Canceled += (sender, reason) => { };
            await CrossJobs.Current.RunAll();
            taskInstance.GetDeferral().Complete();
        }
    }
}
