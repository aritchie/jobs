using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Jobs;


namespace Sample.Jobs
{
    public class SampleJob : IJob
    {
        public Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            return Task.FromResult(false);
        }
    }
}
