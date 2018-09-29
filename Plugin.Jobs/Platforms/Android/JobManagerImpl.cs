using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Java.Lang;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl(IJobRepository repository = null, IJobFactory factory = null) : base(repository, factory) {}


        protected override bool CheckCriteria(JobInfo job) => job.IsEligibleToRun();


        public override async Task Schedule(JobInfo jobInfo)
        {
            await base.Schedule(jobInfo);
            this.StartJobService();
        }


        public override void Cancel(string jobId)
        {
            base.Cancel(jobId);
            if (!this.Repository.GetJobs().Any())
                this.StopJobService();
        }


        public override void CancelAll()
        {
            base.CancelAll();
            this.Native().CancelAll();
        }


        JobScheduler Native() => (JobScheduler)Application.Context.GetSystemService(JobService.JobSchedulerService);

        void StartJobService()
        {
            var sch = this.Native();
            if (!sch.AllPendingJobs.Any(x => x.Id == CrossJobs.AndroidJobId))
            {
                var job = new Android.App.Job.JobInfo.Builder(
                        100,
                        new ComponentName(
                            Application.Context,
                            Class.FromType(typeof(PluginJobService))
                        )
                    )
                    .SetPeriodic(Convert.ToInt64(CrossJobs.PeriodicRunTime.TotalMilliseconds))
                    .SetPersisted(true)
                    .Build();

                sch.Schedule(job);
            }
        }


        void StopJobService() => this.Native().Cancel(CrossJobs.AndroidJobId);
    }
}
