using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.Jobs;


namespace Sample.Jobs
{
    public class SampleJob : IJob
    {
        readonly IUserDialogs dialogs;
        public SampleJob(IUserDialogs dialogs) => this.dialogs = dialogs;


        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            // you really shouldn't interact with the UI from a job
            this.dialogs.Alert("HELLO FROM JOB");
            var loops = jobInfo.Parameters.Get("LoopCount", 25);

            for (var i = 0; i < loops; i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                await Task.Delay(1000, cancelToken).ConfigureAwait(false);
            }
        }
    }
}
