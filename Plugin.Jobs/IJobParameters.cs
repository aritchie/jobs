using System;
using System.Collections.Generic;


namespace Plugin.Jobs
{
    public interface IJobParameters
    {
        ICollection<string> Keys { get; }
        bool ContainsKey(string key);
        void Set<T>(string key, T value);
        T Get<T>(string key, T defaultValue = default(T));
    }
}
