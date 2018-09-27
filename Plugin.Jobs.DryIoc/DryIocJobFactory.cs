using System;
using System.Linq;
using DryIoc;


namespace Plugin.Jobs.DryIoc
{
    public class DryIocJobFactory : IJobFactory
    {
        readonly IContainer container;
        public DryIocJobFactory(IContainer container) => this.container = container;

        public IJob GetInstance(JobInfo jobInfo) =>
            this.container
                .ResolveMany<IJob>()
                .FirstOrDefault(x => x
                    .GetType()
                    .FullName
                    .Equals(jobInfo.Type.FullName)
                ) ?? throw new ArgumentException($"No implementation for job '{jobInfo.Type.FullName}' found");
    }
}
