using System;
using System.Linq;
using Xamarin.Essentials;


namespace Plugin.Jobs
{
    public static class Extensions
    {
        public static bool IsEligibleToRun(this JobInfo job)
        {
            var pluggedIn = Battery.State == BatteryState.Charging || Battery.State == BatteryState.Full;

            if (job.DeviceCharging && !pluggedIn)
                return false;

            if (job.BatteryNotLow && !pluggedIn && Battery.ChargeLevel <= 0.2)
                return false;

            var inetAvail = Connectivity.NetworkAccess == NetworkAccess.Internet;
            var wifi = Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);

            switch (job.RequiredNetwork)
            {
                case NetworkType.Any:
                    return inetAvail;

                case NetworkType.WiFi:
                    return inetAvail && wifi;

                default:
                    return true;
            }
        }
    }
}
