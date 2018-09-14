using System;
using System.Collections.Generic;


namespace Plugin.Jobs
{
    public interface IJobRepository
    {
        IEnumerable<JobInfo> GetJobs();
        IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool failedOnly = false);

        DateTime? GetLastRuntime(string jobName);
        void Cancel(string jobName);
        void CancelAll();
        void Create(JobInfo jobInfo);
        void Log(JobLog log);
    }
}
