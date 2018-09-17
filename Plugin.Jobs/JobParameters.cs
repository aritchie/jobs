using System;
using System.Collections.Generic;


namespace Plugin.Jobs
{
    public class JobParameters : IJobParameters
    {
        readonly IDictionary<string, string> dict;
        public JobParameters(IDictionary<string, string> dict = null)
            => this.dict = dict ?? new Dictionary<string, string>();


        public ICollection<string> Keys => this.dict.Keys;
        public bool ContainsKey(string key) => this.dict.ContainsKey(key);
        public void Set<T>(string key, T value) => this.dict.Add(key, value.ToString());


        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (!this.ContainsKey(key))
                return defaultValue;

            //if (typeof(T) == typeof(string))
            //    return (T)this.dict[key];
            var value = this.dict[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
