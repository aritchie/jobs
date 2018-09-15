using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Jobs
{
    public interface IJobManager
    {
        // TODO: iOS doesn't have triggers like time like UWP & Android
        // TODO: iOS will have no concept of "criteria" like android - check UWP
        //event EventHandler<string, Exception> TaskFinished;
        //event EventHandler<JobRunResults> JobBatchFinished;

        /// <summary>
        /// Runs a one time task - on iOS, it will initiate a background task
        /// </summary>
        /// <param name="task"></param>
        void RunTask(Func<Task> task);


        /// <summary>
        /// Runs a one time task - on iOS, it will initiate a background task
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="task"></param>
        void RunTask(string taskName, Func<Task> task);


        /// <summary>
        /// Flag to see if job manager is running registered tasks
        /// </summary>
        bool IsRunning { get; }


        /// <summary>
        /// This force runs the manager and any registered jobs
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        Task<JobRunResults> Run(CancellationToken? cancelToken = null);


        /// <summary>
        /// Run a specific job adhoc
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        Task Run(string jobName, CancellationToken? cancelToken = null);


        /// <summary>
        /// Gets current registered jobs
        /// </summary>
        /// <returns></returns>
        IEnumerable<JobInfo> GetJobs();


        /// <summary>
        ///
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="since"></param>
        /// <param name="errorsOnly"></param>
        /// <returns></returns>
        IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool errorsOnly = false);


        /// <summary>
        /// Create a new job
        /// </summary>
        /// <param name="jobInfo"></param>
        void Schedule(JobInfo jobInfo);


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
