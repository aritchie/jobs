using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public override async Task<JobRunResult> Run(string jobName, CancellationToken? cancelToken = null)
        {
            var app = UIApplication.SharedApplication;
            var taskId = (int)app.BeginBackgroundTask(jobName, () =>
            {
                // TODO: cancelled log
            });
            var result = await base.Run(jobName, cancelToken);
            app.EndBackgroundTask(taskId);
            return result;
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
