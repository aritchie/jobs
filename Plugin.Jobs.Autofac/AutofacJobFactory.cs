using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;


namespace Plugin.Jobs
{
    public class AutofacJobFactory : IJobFactory
    {
        readonly ILifetimeScope scope;
        public AutofacJobFactory(ILifetimeScope scope) => this.scope = scope;


        public IJob GetInstance(JobInfo jobInfo)
        {
            if (this.scope.TryResolve(out IEnumerable<IJob> list))
            {
                var job = list.FirstOrDefault(x => x.GetType().Equals(jobInfo.Type));
                return job;
            }

            return null;
        }
    }
}
