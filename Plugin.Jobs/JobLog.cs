using System;


namespace Plugin.Jobs
{
    // may want to log runtime batch # of some sort
    public class JobLog
    {
        public string RunId { get; set; }
        public string JobName { get; set; }
        public JobState Status { get; set; }
        public DateTime CreatedOn { get; set; }
        //public DateTime StartTimeUtc { get; set; }
        //public DateTime? EndTimeUtc { get; set; }
        public string Error { get; set; }
    }
}
