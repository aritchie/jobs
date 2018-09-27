# ACR Jobs Plugin Plugin for Xamarin & Windows

[![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)
[Change Log - Sept 27, 2018](changelog.md)


## PLATFORMS

Platform|Version
--------|-------
Android|5.0+
iOS|8+
Windows UWP|16299+
Any Other Platform|Must Support .NET Standard 2.0

## FEATURES
* Cross Platform Job Framework
* Run adhoc jobs in the background (mainly for use on iOS)
* Define jobs with runtime parameters to run at regular intervals
* Place criteria as to when jobs can run responsibly
    * battery charging or not low
    * network connectivity via WiFi or Mobile

## Docs
* Setup
    * [Android](platform_android.md)
    * [iOS](platform_ios.md)
    * [UWP](platform_uwp.md)
* Running Adhoc One-Time Tasks
* Scheduling Defined Jobs
* [Querying Jobs, Run Logs, & Events](other.md)
* Dependency Injection
    * [Autofac](autofac.md)
    * [DryIoc](dryioc.md)
    
## SETUP

Install From [![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)

Follow the Setup Guids
* [Android](platform_android.md)
* [iOS](platform_ios.md)
* [UWP](platform_uwp.md)

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

```