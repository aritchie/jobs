using System;
using Autofac;


namespace Plugin.Jobs
{
    public class AutofacJobFactory : IJobFactory
    {
        readonly ILifetimeScope scope;
        public AutofacJobFactory(ILifetimeScope scope) => this.scope = scope;
        public IJob GetInstance(JobInfo jobInfo) => this.scope.ResolveNamed<IJob>(jobInfo.Name);
    }
}
