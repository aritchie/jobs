using System;
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
        public App()
        {
            this.InitializeComponent();
            GlobalExceptionHandler.Register();

            var builder = new ContainerBuilder();
            builder.Register(_ => UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
            builder.RegisterJobManager();
            builder.RegisterJob<SampleJob>();
            var container = builder.Build();

            this.MainPage = new NavigationPage(new MainPage());
        }
    }
}
