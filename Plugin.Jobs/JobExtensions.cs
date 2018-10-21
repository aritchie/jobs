using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Plugin.Jobs
{
    public static class JobExtensions
    {
        public static T GetValue<T>(this JobInfo job, string key, T defaultValue = default(T))
        {
            if (!job.Parameters.ContainsKey(key))
                return defaultValue;

            var value = job.Parameters[key];
            if (value is string s && typeof(T) != typeof(string))
                return JsonConvert.DeserializeObject<T>(s);

            if (value.GetType() == typeof(T))
                return (T)value;

            if (value.GetType() == typeof(JObject))
                return ((JObject) value).ToObject<T>();

            if (value.GetType() == typeof(JArray))
                return ((JArray) value).ToObject<T>();
            
            return (T) Convert.ChangeType(value, typeof(T));
        }


        public static void SetValue(this JobInfo job, string key, object value)
        {
            job.Parameters[key] = value;
        }
    }
}
