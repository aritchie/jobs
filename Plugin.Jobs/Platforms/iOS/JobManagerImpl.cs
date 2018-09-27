using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UIKit;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public JobManagerImpl(IJobRepository repository = null, IJobFactory factory = null) : base(repository, factory)
        {
        }


        protected override bool CheckCriteria(JobInfo job) => job.IsEligibleToRun();


        public override async Task<JobRunResult> Run(string jobName, CancellationToken? cancelToken = null)
        {
            using (var cancelSrc = new CancellationTokenSource())
            {
                var ct = cancelToken ?? CancellationToken.None;
                using (ct.Register(() => cancelSrc.Cancel()))
                {
                    var app = UIApplication.SharedApplication;
                    var taskId = (int) app.BeginBackgroundTask(jobName, cancelSrc.Cancel);
                    var result = await base.Run(jobName, cancelSrc.Token);
                    app.EndBackgroundTask(taskId);
                    return result;
                }
            }
        }


        public override async Task<IEnumerable<JobRunResult>> RunAll(CancellationToken? cancelToken = null)
        {
            using (var cancelSrc = new CancellationTokenSource())
            {
                var ct = cancelToken ?? CancellationToken.None;
                using (ct.Register(() => cancelSrc.Cancel()))
                {
                    var app = UIApplication.SharedApplication;
                    var taskId = (int) app.BeginBackgroundTask("RunAll", cancelSrc.Cancel);
                    var result = await base.RunAll(cancelSrc.Token);
                    app.EndBackgroundTask(taskId);
                    return result;
                }
            }
        }


        public override async void RunTask(string taskName, Func<Task> task)
        {
            var app = UIApplication.SharedApplication;
            var taskId = 0;
            try
            {
                this.LogTask(JobState.Start, taskName);
                taskId = (int)app.BeginBackgroundTask(taskName, () =>
                {
                    // TODO: cancelled log
                });
                await task().ConfigureAwait(false);
                this.LogTask(JobState.Finish, taskName);
            }
            catch (Exception ex)
            {
                this.LogTask(JobState.Error, taskName, ex);
            }
            finally
            {
                if (taskId > 0)
                    app.EndBackgroundTask(taskId);
            }
        }
    }
}
