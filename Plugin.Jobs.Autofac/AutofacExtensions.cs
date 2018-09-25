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


        public static void RegisterJobManager(this ContainerBuilder builder, bool includeAutofacJobFactory = true, Action<IJobManager> onReady = null)
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
                onReady?.Invoke(mgr);
            });
        }
    }
}
