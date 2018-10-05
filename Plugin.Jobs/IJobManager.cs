using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Plugin.Jobs
{
    public interface IJobManager
    {
        /// <summary>
        /// Runs a one time, adhoc task - on iOS, it will initiate a background task
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="task"></param>
        void RunTask(string taskName, Func<Task> task);


        /// <summary>
        /// Flag to see if job manager is running registered tasks
        /// </summary>
        bool IsRunning { get; }


        /// <summary>
        /// Fires just as a job is about to start
        /// </summary>
        event EventHandler<JobInfo> JobStarted;


        /// <summary>
        /// Fires as each job finishes
        /// </summary>
        event EventHandler<JobRunResult> JobFinished;


        /// <summary>
        /// This force runs the manager and any registered jobs
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        Task<IEnumerable<JobRunResult>> RunAll(CancellationToken cancelToken = default(CancellationToken));


        /// <summary>
        /// Run a specific job adhoc
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        Task<JobRunResult> Run(string jobName, CancellationToken cancelToken = default(CancellationToken));


        /// <summary>
        /// Gets current registered jobs
        /// </summary>
        /// <returns></returns>
        IEnumerable<JobInfo> GetJobs();


        /// <summary>
        /// Get logs for jobs
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="since"></param>
        /// <param name="errorsOnly"></param>
        /// <returns></returns>
        IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool errorsOnly = false);


        /// <summary>
        /// Purge runtime logs
        /// </summary>
        /// <param name="jobName">Pass null to purge all logs</param>
        void PurgeLogs(string jobName = null);


        /// <summary>
        /// Create a new job
        /// </summary>
        /// <param name="jobInfo"></param>
        Task Schedule(JobInfo jobInfo);


        /// <summary>
        /// Cancel a job
        /// </summary>
        /// <param name="jobName"></param>
        void Cancel(string jobName);


        /// <summary>
        /// Cancel All Jobs
        /// </summary>
        void CancelAll();
    }
}
