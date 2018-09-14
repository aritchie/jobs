using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace Plugin.Jobs
{
    public abstract class AbstractJobManager : IJobManager
    {
        public virtual async void RunTask(Func<Task> task)
        {
            try
            {
                this.Log(JobState.Start);
                await task().ConfigureAwait(false);
                this.Log(JobState.Finish);
            }
            catch (Exception ex)
            {
                this.Log(JobState.Error, exception: ex);
            }
        }


        public virtual IEnumerable<JobInfo> GetJobs() => CrossJobs.Repository.GetJobs();
        public virtual void Cancel(string jobName) => CrossJobs.Repository.Cancel(jobName);
        public virtual void CancelAll() => CrossJobs.Repository.CancelAll();
        public bool IsRunning { get; protected set; }


        public virtual void Schedule(JobInfo jobInfo)
        {
            // TODO: validate stuff
            CrossJobs.Repository.Create(jobInfo);
        }


        public virtual Task Run() => Task.Run(async () =>
        {
            if (this.IsRunning)
                return;

            this.IsRunning = false;
            // TODO: watch for backgrounding kill?
            // TODO: may want to allow concurrent jobs
            var cancelSrc = new CancellationTokenSource();
            var jobs = CrossJobs.Repository.GetJobs();

            foreach (var job in jobs)
            {
                try
                {
                    if (this.CheckCriteria(job))
                    {
                        this.Log(JobState.Start, job.Name);
                        var service = CrossJobs.Factory.GetInstance(job);

                        await service
                            .Run(job.Parameters, cancelSrc.Token)
                            .ConfigureAwait(false);

                        this.Log(JobState.Finish, job.Name);
                    }
                }
                catch (Exception ex)
                {
                    this.Log(JobState.Error, job.Name, ex);
                }
            }

            this.IsRunning = false;
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

            // TODO
            var wifi = false;
            var connected = false;

            return true;
        }

        protected virtual void Log(JobState state, string jobName = "BackgroundTask", Exception exception = null) => CrossJobs.Repository.Log(new JobLog
        {
            JobName = jobName,
            CreatedOn = DateTime.UtcNow,
            Status = state,
            Error = exception?.ToString() ?? String.Empty
        });
    }
}
