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
```

```xml
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.BATTERY_STATS" />	

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

// TODO
```

## FAQ

Q. How long does the background sync let me have on iOS

> 30 seconds and not a penny more

Q. How long does a task run on iOS

> 3 minutes on iOS 10+, 10 mins on iOS 8+