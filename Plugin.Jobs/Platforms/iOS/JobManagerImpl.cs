using System;
using System.Threading.Tasks;
using UIKit;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        public override async void RunTask(Func<Task> task)
        {
            var app = UIApplication.SharedApplication;
            var taskId = 0;
            try
            {
                this.Log(JobState.Start, null);
                taskId = (int)app.BeginBackgroundTask("BackgroundTask", () =>
                {
                    // TODO: cancelled log
                });
                await task().ConfigureAwait(false);
                this.Log(JobState.Finish, null);
            }
            catch (Exception ex)
            {
                this.Log(JobState.Error, exception: ex);
            }
            finally
            {
                if (taskId > 0)
                    app.EndBackgroundTask(taskId);
            }
        }
    }
}
