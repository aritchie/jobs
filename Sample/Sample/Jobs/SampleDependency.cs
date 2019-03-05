using System;


namespace Sample.Jobs
{
    public interface ISampleDependency
    {
        DateTime Timestamp { get; }
    }


    public class SampleDependency : ISampleDependency
    {
        public DateTime Timestamp => DateTime.Now;
    }
}
