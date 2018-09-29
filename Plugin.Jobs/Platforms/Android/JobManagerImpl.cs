using System;
using System.Linq;
using System.Threading.Tasks;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl(IJobRepository repository = null, IJobFactory factory = null) : base(repository, factory) {}


        protected override bool CheckCriteria(JobInfo job) => job.IsEligibleToRun();


        public override async Task Schedule(JobInfo jobInfo)
        {
            await base.Schedule(jobInfo);
            CrossJobs.StartJobService();
        }


        public override void Cancel(string jobId)
        {
            base.Cancel(jobId);
            if (!this.Repository.GetJobs().Any())
                CrossJobs.StopJobService();
        }


        public override void CancelAll()
        {
            base.CancelAll();
            CrossJobs.StopJobService();
        }
    }
}
