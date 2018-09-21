# ACR Jobs Plugin Plugin for Xamarin & Windows

[![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)
[Change Log - Sept 14, 2018](changelog.md)


## PLATFORMS

Platform|Version
--------|-------
Android|5.0+
iOS|8+
Windows UWP|16299+

## FEATURES

* Run jobs in the background (mainly for use on iOS)


## SETUP
[![NuGet](https://img.shields.io/nuget/v/Plugin.BluetoothLE.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.BluetoothLE/)

**Android**

Add the following to your AndroidManifest.xml

```csharp
// IN your launch activity or application

Plugin.Jobs.CrossJobs.Init(activity, bundle);


[assembly: UsesPermission("android.permission.BIND_JOB_SERVICE")]
[assembly: UsesPermission("android.permission.RECEIVE_BOOT_COMPLETED")]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.BatteryStats)]
```
OR

```xml
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.BATTERY_STATS" />	
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.BIND_JOB_SERVICE" />
```

**iOS**

```csharp
// In AppDelegate.FinishedLaunching, add the following
Plugin.Jobs.CrossJobs.Init();

// Also, in AppDelegate, insert the following
public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
{
    Plugin.Jobs.CrossJobs.OnBackgroundFetch(completionHandler);
}
```

```xml
<!--Add the following to your info.plish-->
<key>UIBackgroundModes</key>
<array>
	<string>background-fetch</string>
</array>
```

## HOW TO USE

```csharp

// To issue an adhoc task that can continue to run in the background 
CrossJobs.Current.RunTask(async () => 
{
    // your code
});


// Scheduling a new job
public class YourJob : IJob
{
    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        var loops = jobInfo.Parameters.Get("LoopCount", 25);

        for (var i = 0; i < loops; i++)
        {
            if (cancelToken.IsCancellationRequested)
                break;

            await Task.Delay(1000, cancelToken).ConfigureAwait(false);
        }
    }
}
var job = new JobInfo
{
    Name = "YourJobName",
    Type = typeof(YourJob),

    // these are criteria that must be met in order for your job to run
    BatteryNotLow = this.BatteryNotLow,
    DeviceCharging = this.DeviceCharging
    NetworkType = NetworkType.Any
};

// you can pass variables to your job
job.Parameters.Set("LoopCount", 10);
CrossJobs.Current.Schedule(job);

// Cancelling A Job
CrossJobs.Current.Cancel("YourJobName");

// Cancelling All Jobs
CrossJobs.Current.CancelAll();

// Run All Jobs On-Demand
var results = await CrossJobs.Current.RunAll();

// Run A Specific Job On-Demand
var result = await CrossJobs.Current.Run("YourJobName");

// Listening for job(s) to Finish when not running on-demand
CrossJobs.Current.JobFinished += (sender, args) =>
{
    args.Job.Name // etc
    args.Success
    args.Exception
}

// Get Current Jobs (this does not include Tasks!)
var jobs = CrossJobs.Current.GetJobs();


// Get Job Logs - All Variables involved are optional filters
var logs = CrossJobs.Current.GetLogs(
    jobName,  // for a specific job
    DateTime.Yesterday, // all logs since this date/time (UTC based)
    errorsOnly // boolean to review logs that errored only
)

// Plugging in custom job factory
TODO
```

## FAQ

Q. How long does the background sync let me have on iOS

> 30 seconds and not a penny more

Q. How long does a task run on iOS

> 3 minutes on iOS 10+, 10 mins on iOS 8+

Q. How do I schedule periodic jobs?

> All jobs are considered periodic with or without criteria

Q. Why no job triggers? (ie. geofence, bluetooth, specific time)

> I am considering some triggers in the future. The current limitations on the time factored jobs is that iOS is in complete control of how/when things are run