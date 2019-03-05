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
        readonly ReflectionJobFactory fallbackFactory;


        protected AbstractJobManager(IJobRepository repository, IJobFactory factory)
        {
            this.fallbackFactory = new ReflectionJobFactory();
            this.Repository = repository ?? new SqliteJobRepository();
            this.Factory = factory ?? this.fallbackFactory;
        }


        protected IJobRepository Repository { get; }
        protected IJobFactory Factory { get; }
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
            var job = this.Repository.GetByName(jobName);
            if (job == null)
                throw new ArgumentException("No job found named " + jobName);

            var result = await this.RunJob(job, "manual", cancelToken).ConfigureAwait(false);
            return result;
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


        public virtual Task<IEnumerable<JobRunResult>> RunAll(CancellationToken cancelToken) => Task.Run(async () =>
        {
            if (this.IsRunning)
                throw new ArgumentException("Job manager is already running");

            this.IsRunning = true;
            var jobs = this.Repository.GetJobs();
            var runId = Guid.NewGuid().ToString();
            var tasks = new List<Task<JobRunResult>>();

            foreach (var job in jobs)
            {
                if (this.CheckCriteria(job))
                    tasks.Add(this.RunJob(job, runId, cancelToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            var results = tasks.Select(x => x.Result).AsEnumerable();
            this.IsRunning = false;
            return results;
        });


        protected virtual async Task<JobRunResult> RunJob(JobInfo job, string batchName, CancellationToken cancelToken)
        {
            this.JobStarted?.Invoke(this, job);
            var result = default(JobRunResult);
            var cancel = false;

            try
            {
                this.LogJob(JobState.Start, job, "manual");
                var service = this.Factory.GetInstance(job) ?? this.fallbackFactory.GetInstance(job);

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
            if (!CrossJobs.IsLoggingEnabled)
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
            if (!CrossJobs.IsLoggingEnabled)
                return; 

            this.Repository.Log(new JobLog
            {
                JobName = taskName,
                CreatedOn = DateTime.UtcNow,
                Status = state,
                Error = exception?.ToString() ?? String.Empty
            });
        }
    }
}
