## Setup - Android

1.Install From [![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)

2. In your application.OnCreate - add the following (if you don't have one, please take a look at the [sample](https://github.com/aritchie/jobs/blob/master/Sample/Sample.Android/MainActivity.cs)).  This is required in order for background reboots to take place.
```csharp
Plugin.Jobs.CrossJobs.Init(activity, bundle); // activity
```


3.Add the following to your AndroidManifest.xml

```xml
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.BATTERY_STATS" />	
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
```