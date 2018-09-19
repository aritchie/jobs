using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Jobs;


namespace Sample.Jobs
{
    public class SampleJob : IJob
    {
        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
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
