using System;


namespace Plugin.Jobs
{
    public interface IJobFactory
    {
        IJob GetInstance(JobInfo jobInfo);
    }
}
