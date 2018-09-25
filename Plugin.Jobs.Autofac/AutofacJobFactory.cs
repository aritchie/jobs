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

        public IJob GetInstance(JobInfo jobInfo) => this.scope
                                                        .Resolve<IEnumerable<IJob>>()
                                                        .FirstOrDefault(x => x
                                                            .GetType()
                                                            .FullName
                                                            .Equals(jobInfo.Type.FullName)
                                                        ) ?? throw new ArgumentException("");
    }
}
