using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Jobs.Infrastructure;


namespace Plugin.Jobs
{
    public abstract class AbstractJobManager : IJobManager
    {
        protected AbstractJobManager(IJobRepository repository)
        {
            this.Repository = repository ?? new SqliteJobRepository();
        }


        protected IJobRepository Repository { get; }
        protected abstract bool CheckCriteria(JobInfo job);


        public virtual Task<bool> HasPermissions() => Task.FromResult(true);


        public virtual async void RunTask(string taskName, Func<CancellationToken, Task> task)
        {
            try
            {
                this.LogTask(JobState.Start, taskName);
                await task(CancellationToken.None).ConfigureAwait(false);
                this.LogTask(JobState.Finish, taskName);
            }
            catch (Exception ex)
            {
                this.LogTask(JobState.Error, taskName, ex);
            }
        }


        public virtual async Task<JobRunResult> Run(string jobName, CancellationToken cancelToken)
        {
            JobInfo job = null;
            try
            {
                job = this.Repository.GetByName(jobName);
                if (job == null)
                    throw new ArgumentException("No job found named " + jobName);

                var result = await this.RunJob(job, jobName, cancelToken).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                job = job ?? new JobInfo
                {
                    Name = jobName,
                    Type = typeof(object)
                };
                return new JobRunResult(job, ex);
            }
        }


        public virtual IEnumerable<JobInfo> GetJobs() => this.Repository.GetJobs();
        public virtual IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool errorsOnly = false)
            => this.Repository.GetLogs(jobName, since, errorsOnly);

        public virtual void PurgeLogs(string jobName = null, TimeSpan? maxAge = null)
            => this.Repository.PurgeLogs(jobName, maxAge);


        public virtual void Cancel(string jobName) => this.Repository.Cancel(jobName);
        public virtual void CancelAll() => this.Repository.CancelAll();
        public bool IsRunning { get; protected set; }
        public event EventHandler<JobInfo> JobStarted;
        public event EventHandler<JobRunResult> JobFinished;


        public virtual Task Schedule(JobInfo jobInfo)
        {
            if (String.IsNullOrWhiteSpace(jobInfo.Name))
                throw new ArgumentException("No job name defined");

            if (jobInfo.Type == null)
                throw new ArgumentException("Type not set");

            //if (!jobInfo.Type.GetTypeInfo().IsAssignableFrom(typeof(IJob)))
            //    throw new ArgumentException($"{jobInfo.Type.FullName} is not an implementation of {typeof(IJob).Name}");

            this.Repository.Persist(jobInfo, false);
            return Task.CompletedTask;
        }


        public virtual async Task<IEnumerable<JobRunResult>> RunAll(CancellationToken cancelToken)
        {
            var list = new List<JobRunResult>();
            var runId = Guid.NewGuid().ToString();

            if (!this.IsRunning)
            {
                try
                {
                    this.IsRunning = true;
                    var jobs = this.Repository.GetJobs();
                    var tasks = new List<Task<JobRunResult>>();

                    foreach (var job in jobs)
                    {
                        if (this.CheckCriteria(job))
                            tasks.Add(this.RunJob(job, runId, cancelToken));
                    }

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    list = tasks.Select(x => x.Result).ToList();
                }
                catch (Exception ex)
                {
                    var jobInfo = new JobInfo { Name = "NO_JOB" };
                    list.Add(new JobRunResult(jobInfo, ex));
                    this.LogJob(JobState.Error, jobInfo, runId, ex);
                }
                finally
                {
                    this.IsRunning = false;
                }
            }
            return list;
        }


        protected virtual async Task<JobRunResult> RunJob(JobInfo job, string batchName, CancellationToken cancelToken)
        {
            var result = default(JobRunResult);
            var cancel = false;

            try
            {
                this.JobStarted?.Invoke(this, job);
                this.LogJob(JobState.Start, job, "manual");
                var service = CrossJobs.ResolveJob(job);

                await service
                    .Run(job, cancelToken)
                    .ConfigureAwait(false);

                if (!job.Repeat)
                {
                    this.Cancel(job.Name);
                    cancel = true;
                }
                this.LogJob(JobState.Finish, job, "manual");
                result = new JobRunResult(job, null);
            }
            catch (Exception ex)
            {
                this.LogJob(JobState.Error, job, "manual", ex);
                result = new JobRunResult(job, ex);
            }
            finally
            {
                if (!cancel)
                {
                    job.LastRunUtc = DateTime.UtcNow;
                    this.Repository.Persist(job, true);
                }
            }
            this.JobFinished?.Invoke(this, result);
            return result;
        }


        protected virtual void LogJob(JobState state,
                                      JobInfo job,
                                      string runId,
                                      Exception exception = null)
        {
            if (!this.ShouldWriteLog(exception))
                return;

            this.Repository.Log(new JobLog
            {
                JobName = job.Name,
                RunId = runId,
                CreatedOn = DateTime.UtcNow,
                Status = state,
                Error = exception?.ToString() ?? String.Empty
            });
        }


        protected virtual void LogTask(JobState state, string taskName, Exception exception = null)
        {
            if (!this.ShouldWriteLog(exception))
                return;

            this.Repository.Log(new JobLog
            {
                JobName = taskName,
                CreatedOn = DateTime.UtcNow,
                Status = state,
                Error = exception?.ToString() ?? String.Empty
            });
        }


        protected virtual bool ShouldWriteLog(Exception exception)
        {
            switch (CrossJobs.LogLevel)
            {
                case JobLogLevel.None:
                    return false;

                case JobLogLevel.All:
                    return true;

                case JobLogLevel.ErrorsOnly:
                default:
                    return exception != null;
            }
        }
    }
}
