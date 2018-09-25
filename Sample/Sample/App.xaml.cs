using System;
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
            builder.RegisterJobManager();
            builder.RegisterJob<SampleJob>();
            var container = builder.Build();

            this.MainPage = new NavigationPage(new MainPage());
        }
    }
}
