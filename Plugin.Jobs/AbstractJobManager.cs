using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace Plugin.Jobs
{
    public abstract class AbstractJobManager : IJobManager
    {
        public virtual void RunTask(Func<Task> task) => this.RunTask(Guid.NewGuid().ToString(), task);

        public virtual async void RunTask(string taskName, Func<Task> task)
        {
            try
            {
                this.LogTask(JobState.Start, taskName);
                await task().ConfigureAwait(false);
                this.LogTask(JobState.Finish, taskName);
            }
            catch (Exception ex)
            {
                this.LogTask(JobState.Error, taskName, ex);
            }
        }


        public virtual IEnumerable<JobInfo> GetJobs() => CrossJobs.Repository.GetJobs();
        public virtual IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool errorsOnly = false)
            => CrossJobs.Repository.GetLogs(jobName, since, errorsOnly);

        public virtual void Cancel(string jobName) => CrossJobs.Repository.Cancel(jobName);
        public virtual void CancelAll() => CrossJobs.Repository.CancelAll();
        public bool IsRunning { get; protected set; }


        public virtual void Schedule(JobInfo jobInfo)
        {
            // TODO: validate stuff
            CrossJobs.Repository.Create(jobInfo);
        }


        public virtual Task<JobRunResults> Run(CancellationToken? cancelToken = null) => Task.Run(async () =>
        {
            if (this.IsRunning)
                throw new ArgumentException("");

            var ct = cancelToken ?? CancellationToken.None;
            this.IsRunning = false;
            // TODO: watch for backgrounding kill?
            // TODO: may want to allow concurrent jobs
            var cancelSrc = new CancellationTokenSource();
            var jobs = CrossJobs.Repository.GetJobs();

            var runId = Guid.NewGuid().ToString();
            var count = 0;
            var errors = 0;

            foreach (var job in jobs)
            {
                try
                {
                    if (this.CheckCriteria(job))
                    {
                        count++;
                        this.LogJob(JobState.Start, job, runId);
                        var service = CrossJobs.Factory.GetInstance(job);

                        await service
                            .Run(job, cancelSrc.Token)
                            .ConfigureAwait(false);

                        this.LogJob(JobState.Finish, job, runId);
                    }
                }
                catch (Exception ex)
                {
                    errors++;
                    this.LogJob(JobState.Error, job, runId);
                }
                finally
                {
                    job.LastRunUtc = DateTime.UtcNow;
                    // TODO: update job data
                }
            }

            this.IsRunning = false;
            return new JobRunResults(count, errors);
        });


        protected virtual bool CheckCriteria(JobInfo job)
        {
            var pluggedIn = Battery.State == BatteryState.Charging || Battery.State == BatteryState.Full;

            if (job.BatteryNotLow)
            {
                var lowBattery = Battery.ChargeLevel <= 0.2;
                if (!pluggedIn && lowBattery)
                    return false;
            }

            var inetAvail = Connectivity.NetworkAccess == NetworkAccess.Internet;
            var wifi = Connectivity.Profiles.Contains(ConnectionProfile.WiFi);
            if (job.RequiredNetwork == NetworkType.Any && !inetAvail)
                return false;

            if (job.RequiredNetwork == NetworkType.WiFi && !wifi)
                return false;

            return true;
        }


        protected virtual void LogJob(JobState state,
                                      JobInfo job,
                                      string runId,
                                      Exception exception = null)
            => CrossJobs.Repository.Log(new JobLog
            {
                JobName = job.Name,
                RunId = runId,
                CreatedOn = DateTime.UtcNow,
                Status = state,
                Error = exception?.ToString() ?? String.Empty
            });


        protected virtual void LogTask(JobState state, string taskName, Exception exception = null)
            => CrossJobs.Repository.Log(new JobLog
            {
                JobName = taskName,
                CreatedOn = DateTime.UtcNow,
                Status = state,
                Error = exception?.ToString() ?? String.Empty
            });
    }
}
