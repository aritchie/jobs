## Setup - iOS

Install From [![NuGet](https://img.shields.io/nuget/v/Plugin.Jobs.svg?maxAge=2592000)](https://www.nuget.org/packages/Plugin.Jobs/)

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
