using System;
using Foundation;
using Plugin.Jobs;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


namespace Sample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            app.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            CrossJobs.LogLevel = JobLogLevel.All;
            CrossJobs.Init(900);
            Forms.Init();
            this.LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }


        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
            => CrossJobs.OnBackgroundFetch(completionHandler);
    }
}
