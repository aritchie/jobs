using System;
using DryIoc;

namespace Plugin.Jobs.DryIoc
{
    public static class DryIocExtensions
    {
        /// <summary>
        /// WARNING: Make sure all of your dependencies for any custom job manager are registered before calling this!
        /// </summary>
        /// <param name="container"></param>
        /// <param name="includeDryIocJobFactory"></param>
        public static void RegisterJobManager(this Container container, bool includeDryIocJobFactory = true)
        {
            if (includeDryIocJobFactory)
            {
                container
                    .Register<DryIocJobFactory>()
                    .As<IJobFactory>()
                    .SingleInstance();
            }

            container.Register<IJobManager, JobManagerImpl>(Reuse.Singleton);
            CrossJobs.Current = container.Resolve<IJobManager>();
        }


        public static void RegisterJob<TJob>(this Container container) where TJob : IJob =>
            container.Register<TJob>(Reuse.Singleton);


        public static void Schedule<TJob>(this Container container, JobInfo jobInfo) where TJob : IJob
        {
            jobInfo.Type = typeof(TJob);
            container.RegisterJob<TJob>();
            CrossJobs.Current.Schedule(jobInfo);
        }
    }
}
