using System;
using Autofac;


namespace Plugin.Jobs
{
    public static class AutofacExtensions
    {
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

            builder.RegisterBuildCallback(c =>
            {
                var mgr = c.Resolve<IJobManager>();
                CrossJobs.Current = mgr;
            });
        }


        public static void RegisterJob<TJob>(this ContainerBuilder builder) where TJob : IJob =>
            builder.RegisterType<TJob>().As<IJob>().SingleInstance();


        public static void Schedule<TJob>(this ContainerBuilder builder, JobInfo jobInfo) where TJob : IJob
        {
            jobInfo.Type = typeof(TJob);
            builder.RegisterJob<TJob>();
            builder.RegisterBuildCallback(_ => CrossJobs.Current.Schedule(jobInfo));
        }
    }
}
