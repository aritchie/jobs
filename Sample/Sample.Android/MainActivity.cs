using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Jobs;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


namespace Sample.Droid
{
    [Activity
        (Label = "Jobs Sample",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            UserDialogs.Init(() => (Activity)Forms.Context);
            Forms.Init(this, savedInstanceState);
            this.LoadApplication(new App());

            CrossJobs.Init(this, savedInstanceState);
        }
    }
}