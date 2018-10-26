using System;
using System.Linq;
using DryIoc;


namespace Plugin.Jobs.DryIoc
{
    public class DryIocJobFactory : IJobFactory
    {
        readonly IContainer container;
        public DryIocJobFactory(IContainer container) => this.container = container;

        public IJob GetInstance(JobInfo jobInfo)
        {
            var job = this.container
                .ResolveMany<IJob>()
                .FirstOrDefault(x => x
                    .GetType()
                    .Equals(jobInfo.Type)
                );

            return job;
        }
    }
}
