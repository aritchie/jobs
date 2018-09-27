using System;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl(IJobRepository repository = null, IJobFactory factory = null) : base(repository, factory)
        {
        }


        protected override bool CheckCriteria(JobInfo job) => job.IsEligibleToRun();
    }
}
