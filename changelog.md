# CHANGE LOG

## 2.0 [patch]
* [fix][uwp] Default period timespan needs to be a minimum of 15 mins
* [fix][uwp] Deal with background cancellations
* [fix][all] Safety job loading in Run & RunAll
* [fix][ios] don't check preconditions - OS will already have figured this out
* [breaking] CrossJobs.LogLevel allows None|ErrorsOnly|All replacing IsLoggingEnabled flag

## 2.0.x
* [BREAKING] Job Factory has been removed in place of CrossJobs.ResolveJob(Func<JobInfo, IJob>)
* [BREAKING] DI libraries have been removed - Android complicated this for most people 
* [improvement] Android job service is managed better via Init
* [improvement] You can enable/disable logging globally using CrossJob.IsLoggingEnabled = bool;
* Xamarin Essentials, JSON.NET nuget version bumps

## 1.4.1
* [feature] add IJobManager.HasPermissions check

## 1.4.0
* [update] Update to Xamarin.Essentials 1.0.0

## 1.3.0
* [feature] If container based job factory fails to find an instance, the default job factory is used
* [feature] In your job, you can now set the jobInfo.Repeats to false to stop it from running again in the future

## 1.2.1
* [fix] Support complex objects & lists in job parameters

## 1.2.0
* [feature] Built-in log trimming job - Use JobManager.ScheduleLogTrimmingJob(TimeSpan MaxAge)

## 1.1.1
* [ios][fix] RunAll is running inside RunTask producing 2 background tasks /w is also producing bad reporting
* [ios][fix] RunTask now supports cancellation (only iOS has this functionality)

## 1.1.0
* Better internal parameter serializer
* JobParameters was an unnecessary interface and has been replaced by plain-jane IDictionary /w some beautiful extension methods

## 1.0.0
* Initial Release