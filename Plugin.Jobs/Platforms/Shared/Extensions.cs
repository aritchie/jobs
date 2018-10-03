using System;
using System.Linq;
using Xamarin.Essentials;


namespace Plugin.Jobs
{
    public static partial class Extensions
    {
        public static bool IsEligibleToRun(this JobInfo job)
        {
            var pluggedIn = Battery.State == BatteryState.Charging || Battery.State == BatteryState.Full;

            if (job.BatteryNotLow)
            {
                var lowBattery = Battery.ChargeLevel <= 0.2;
                if (!pluggedIn && lowBattery)
                    return false;
            }

            var inetAvail = Connectivity.NetworkAccess == NetworkAccess.Internet;
            var wifi = Connectivity.Profiles.Contains(ConnectionProfile.WiFi);
            if (job.RequiredNetwork == NetworkType.Any && !inetAvail)
                return false;

            if (job.RequiredNetwork == NetworkType.WiFi && !wifi)
                return false;

            return true;
        }
    }
}
