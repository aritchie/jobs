using System;


namespace Plugin.Jobs.Infrastructure
{
    public class ReflectionJobFactory : IJobFactory
    {
        public IJob GetInstance(JobInfo jobInfo)
        {
            if (jobInfo.Type == null)
                throw new ArgumentException($"Job '{jobInfo.Name}' class type not found");

            var job = Activator.CreateInstance(jobInfo.Type) as IJob;
            if (job == null)
                throw new ArgumentException("Type is not IJob or was not found");

            return job;
        }
    }
}
