using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Plugin.Jobs
{
    public interface IJob
    {
        /// <summary>
        /// Runs your code
        /// </summary>
        /// <param name="lastRunUtc"></param>
        /// <param name="parameters"></param>
        /// <param name="cancelToken"></param>
        /// <returns>Return true to run next wave, return false to cancel job</returns>
        Task<bool> Run(DateTime? lastRunUtc, IDictionary<string, object> parameters, CancellationToken cancelToken);
    }
}
