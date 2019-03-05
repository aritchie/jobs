using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Jobs;


namespace Sample.Jobs
{
    public class SampleJob : IJob
    {
        readonly ISampleDependency dependency;
        public SampleJob(ISampleDependency dependency)
        {
            this.dependency = dependency;
        }

        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            // you really shouldn't interact with the UI from a job
            //this.dialogs.Alert("HELLO FROM JOB");
            var loops = jobInfo.GetValue("LoopCount", 25);
            
            for (var i = 0; i < loops; i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                await Task.Delay(1000, cancelToken).ConfigureAwait(false);
            }
        }
    }
}
