using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Jobs;


namespace Sample.Jobs
{
    public class SampleJob : IJob
    {
        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            //var loops = (int)jobInfo.Parameters["LoopCount"];
            var loops = 25;

            for (var i = 0; i < loops; i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                await Task.Delay(1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
