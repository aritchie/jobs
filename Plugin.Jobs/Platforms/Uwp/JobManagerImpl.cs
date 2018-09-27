using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl(IJobRepository repository = null, IJobFactory factory = null) : base(repository, factory)
        {
        }


        protected override bool CheckCriteria(JobInfo job) => job.IsEligibleToRun();


        public override async Task Schedule(JobInfo jobInfo)
        {
            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();
            switch (requestStatus)
            {
                //case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                //case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                case BackgroundAccessStatus.AllowedSubjectToSystemPolicy:
                case BackgroundAccessStatus.AlwaysAllowed:
                    await base.Schedule(jobInfo);
                    this.TryRegUwpJob();
                    break;

                default:
                    throw new ArgumentException("Request declined - " + requestStatus);
            }
        }


        public override void Cancel(string jobName)
        {
            base.Cancel(jobName);
            if (!this.GetJobs().Any())
                GetPluginJob()?.Unregister(true);
        }


        public override void CancelAll()
        {
            base.CancelAll();
            GetPluginJob()?.Unregister(true);
        }


        void TryRegUwpJob()
        {
            // TODO: do I have to reg every app start?
            var job = GetPluginJob();
            if (job == null)
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = CrossJobs.BackgroundJobName,
                    //CancelOnConditionLoss = false,
                    //IsNetworkRequested = true,
                    TaskEntryPoint = typeof(PluginBackgroundTask).FullName
                };
                //builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable));
                //builder.SetTrigger(new BluetoothLEAdvertisementWatcherTrigger());
                //builder.SetTrigger(new GattServiceProviderTrigger());
                //builder.SetTrigger(new GeovisitTrigger());
                //builder.SetTrigger(new ToastNotificationActionTrigger());
                var runMins = Convert.ToUInt32(Math.Round(CrossJobs.PeriodicRunTime.TotalMinutes, 0));
                builder.SetTrigger(new TimeTrigger(runMins, false));
                builder.Register();
            }
        }


        static IBackgroundTaskRegistration GetPluginJob()
            => BackgroundTaskRegistration
                .AllTasks
                .Where(x => x.Value.Name.Equals(CrossJobs.BackgroundJobName))
                .Select(x => x.Value)
                .FirstOrDefault();
    }
}
