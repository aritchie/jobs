using System;
using Autofac;


namespace Plugin.Jobs
{
    public static class AutofacExtensions
    {
        public static void RegisterJob<T>(this ContainerBuilder builder) where T : IJob
            => builder
                .RegisterType<T>()
                .As<IJob>()
                .SingleInstance();



        public static void RegisterJobManager(this ContainerBuilder builder, bool includeAutofacJobFactory = true)
        {
            if (includeAutofacJobFactory)
            {
                builder
                    .RegisterType<AutofacJobFactory>()
                    .As<IJobFactory>()
                    .SingleInstance();
            }

            builder
                .RegisterType<JobManagerImpl>()
                .As<IJobManager>()
                .SingleInstance();

            builder.RegisterBuildCallback(c => CrossJobs.Current = c.Resolve<IJobManager>());
        }
    }
}
