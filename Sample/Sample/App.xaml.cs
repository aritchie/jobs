using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Autofac;
using Plugin.Jobs;
using Sample.Jobs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]


namespace Sample
{
    public partial class App : Application
    {
        static IContainer jobContainer;
        static IContainer mainContainer;


        public App()
        {
            this.InitializeComponent();

            InitJobContainer();

            // this will register all the dependencies you need for your views (foreground portion of application)
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AutofacRegistrationSource(jobContainer));

            builder.Register(_ => UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
            builder.RegisterType<GlobalExceptionHandler>().AsImplementedInterfaces().AutoActivate().SingleInstance();
            builder.Register(_ => CrossJobs.Current).As<IJobManager>().SingleInstance();
            mainContainer = builder.Build();

            // This backfills for iOS and UWP, Android will have set this from MainApplication
            CrossJobs.ResolveJob = (jobInfo) => ResolveJob(jobInfo);  
            this.MainPage = new NavigationPage(new MainPage());
        }


        static void InitJobContainer()
        {
            if (jobContainer != null)
                return;

            // register the jobs here as well as any dependencies they have
            var builder = new ContainerBuilder();
            builder.RegisterType<SampleDependency>().As<ISampleDependency>().SingleInstance();
            builder.RegisterType<SampleJob>().As<IJob>().SingleInstance();
            jobContainer = builder.Build();
        }


        public static IJob ResolveJob(JobInfo jobInfo)
        {
            InitJobContainer();
            if (jobContainer.TryResolve(out IEnumerable<IJob> list))
            {
                var job = list.FirstOrDefault(x => x.GetType().Equals(jobInfo.Type));
                return job;
            }
            return null;
        }
    }
}
