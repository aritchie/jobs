using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;


namespace Plugin.Jobs
{
    public static class AutofacExtensions
    {
        //public static void RegisterJob<T>(this ContainerBuilder builder, JobInfo jobInfo) where T : IJob
        //{
        //    builder.RegisterBuildCallback(ctx => ctx.Resolve<IJobManager>().Schedule(jobInfo));
        //    //builder
        //    .RegisterType<T>()
        //    .Named<IJob>(jobName)
        //    .As<IJob>()
        //    .SingleInstance();
        //}

        // TODO: this is currently haphazard since scheduling and DI registration are separate functions
        // also, a job type may be reused by named differently due to different parameters

        public class TestJob : IJob
        {
            public Task Run(JobInfo jobInfo, CancellationToken cancelToken)
            {
                throw new NotImplementedException();
            }
        }

        // I don't like this
        public static void Test()
        {
            var cb = new ContainerBuilder();
            cb.RegisterJobManager(); // user should never register a custom IJobManager unless at a platform level
            cb.RegisterJob<IJob>("test");
            var container = cb.Build();

            // TODO: what if job is already registered - need method to check
            container.Resolve<IJobManager>().Schedule(new JobInfo
            {
                Name = "test",
                Type = typeof(TestJob)
            });
        }

        public static void RegisterJob<T>(this ContainerBuilder builder, string jobName) where T : IJob
            => builder
                .RegisterType<T>()
                .Named<IJob>(jobName)
                .As<IJob>()
                .SingleInstance();

        public static void RegisterJobManager(this ContainerBuilder builder)
            => builder
                .Register(ctx =>
                {
                    if (ctx.IsRegistered<IJobRepository>())
                        JobServices.Repository = ctx.Resolve<IJobRepository>();

                    if (ctx.IsRegistered<IJobFactory>())
                        JobServices.Factory = ctx.Resolve<IJobFactory>();

                    return CrossJobs.Current;
                })
                .As<IJobManager>()
                .SingleInstance();
    }
}
