using System;
using System.Threading;
using System.Threading.Tasks;


namespace Plugin.Jobs
{
    public interface IJob
    {
        /// <summary>
        /// Runs your code
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <param name="cancelToken"></param>
        /// <returns>Return true to run next wave, return false to cancel job</returns>
        Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken);
    }
}
