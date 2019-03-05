using System;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl(IJobRepository repository = null) : base(repository)
        {
        }


        protected override bool CheckCriteria(JobInfo job) => true;
    }
}
