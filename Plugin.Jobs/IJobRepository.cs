using System;
using System.Collections.Generic;


namespace Plugin.Jobs
{
    public interface IJobRepository
    {
        IEnumerable<JobInfo> GetJobs();
        IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool failedOnly = false);
        void PurgeLogs(string jobName = null, TimeSpan? maxAge = null);

        JobInfo GetByName(string jobName);
        void Cancel(string jobName);
        void CancelAll();
        void Persist(JobInfo jobInfo, bool updateDate);
        void Log(JobLog log);
    }
}
