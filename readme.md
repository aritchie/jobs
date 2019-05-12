# UPDATE - This library has is moving to the Shiny Framework at https://github.com/shinyorg/shiny but will continue to offer support for now

# ACR Background Jobs Plugin for Xamarin & Windows

[Change Log - March 5, 2019](changelog.md)
### [SUPPORT THIS PROJECT](https://allancritchie.net/projects)

Plugin.Jobs|[![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)

### BUILDS
Branch|Status
------|------
Master|[![Build status](https://dev.azure.com/allanritchie/Plugins/_apis/build/status/Jobs?branchName=master)](https://dev.azure.com/allanritchie/Plugins/_build/latest?definitionId=0)
Dev|[![Build status](https://dev.azure.com/allanritchie/Plugins/_apis/build/status/Jobs?branchName=dev)](https://dev.azure.com/allanritchie/Plugins/_build/latest?definitionId=0)



## PLATFORMS

Platform|Version
--------|-------
Android|5.0+
iOS|8+
Windows UWP|16299+
Any Other Platform|Must Support .NET Standard 2.0

iOS, Android, & UWP implementations use [Xamarin Essentials](https://github.com/xamarin/essentials)

## FEATURES
* Cross Platform Background Jobs Framework
* Run adhoc jobs in the background (mainly for use on iOS)
* Define jobs with runtime parameters to run at regular intervals
* Internal logging to let you know how often jobs are running, if they complete successfully or error
* Place criteria as to when jobs can run responsibly
    * device is charging 
    * battery is not low
    * Internet connectivity via Mobile
    * Internet connectivity via WiFi

## DOCS
* Setup
    * [Android](platform_android.md)
    * [iOS](platform_ios.md)
    * [UWP](platform_uwp.md)
* [Running Adhoc One-Time Tasks](#adhoc)
* [Scheduling Background Jobs](#schedule)
* [Cancelling Jobs](#cancel)
* [Running Jobs On-Demand](#ondemand)
* [Querying Jobs, Run Logs, & Events](other.md)
* [Dependency Injection](di.md)
* [FAQ - Frequently Asked Questions](faq.md)
    
## SETUP

Install From [![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)

Follow the Setup Guids
* [Android](platform_android.md)
* [iOS](platform_ios.md)
* [UWP](platform_uwp.md)

## HOW TO USE


#### <a name="adhoc"></a>Creating a One-Time Adhoc Job
```csharp

// To issue an adhoc task that can continue to run in the background 
CrossJobs.Current.RunTask(async () => 
{
    // your code
});
```

#### <a name="schedule"></a>Scheduling a background job
```csharp
// first define your job
public class YourJob : IJob
{
    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        var loops = jobInfo.GetValue("LoopCount", 25);

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
    NetworkType = NetworkType.Any,
    Repeat = true; //defaults to true, set to false to run once OR set it inside a job to cancel further execution
};

// you can pass variables to your job
job.SetValue("LoopCount", 10);


// lastly, schedule it to go - don't worry about scheduling something more than once, we just update if your job name matches an existing one
CrossJobs.Current.Schedule(job);
```

#### <a name="cancel"></a>Cancelling Jobs
```csharp
// Cancelling A Job
CrossJobs.Current.Cancel("YourJobName");

// Cancelling All Jobs
CrossJobs.Current.CancelAll();
```

#### <a name="ondemand"></a>Running Jobs On-Demand
```csharp
// Run All Jobs On-Demand
var results = await CrossJobs.Current.RunAll();

// Run A Specific Job On-Demand
var result = await CrossJobs.Current.Run("YourJobName");

```
