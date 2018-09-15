using System;
using System.Collections.Generic;


namespace Plugin.Jobs
{
    public interface IJobRepository
    {
        IEnumerable<JobInfo> GetJobs();
        IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool failedOnly = false);

        JobInfo GetByName(string jobName);
        void Cancel(string jobName);
        void CancelAll();
        void Create(JobInfo jobInfo);
        void Update(JobInfo jobInfo);
        void Log(JobLog log);
    }
}
