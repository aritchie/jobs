using System;


namespace Plugin.Jobs
{
    public struct JobRunResults
    {
        public JobRunResults(int tasks, int errors)
        {
            this.Tasks = tasks;
            this.Errors = errors;
        }


        public int Tasks { get; }
        public int Errors { get; }
    }
}
