using System;
using Windows.ApplicationModel.Background;


namespace Plugin.Jobs
{
    public static partial class CrossJobs
    {
        public static void Init()
        {
            Current = new JobManagerImpl();

            var builder = new BackgroundTaskBuilder
            {
                Name = "PluginTask",
                CancelOnConditionLoss = false,
                IsNetworkRequested = true,
                TaskEntryPoint = typeof(PluginBackgroundTask).FullName
            };
            //builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable));
            //builder.SetTrigger(new BluetoothLEAdvertisementWatcherTrigger());
            //builder.SetTrigger(new GattServiceProviderTrigger());
            //builder.SetTrigger(new GeovisitTrigger());
            //builder.SetTrigger(new ToastNotificationActionTrigger());
            builder.SetTrigger(new TimeTrigger(10, false));
            builder.Register();
        }
    }
}
