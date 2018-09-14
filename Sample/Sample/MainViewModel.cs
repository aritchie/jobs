using System;
using System.Collections.Generic;
using System.Windows.Input;
using Plugin.Jobs;
using ReactiveUI;


namespace Sample
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            this.Commands = new List<CommandItem>
            {
                new CommandItem
                {
                    Text = "",
                    Command = ReactiveCommand.Create(() =>
                    {

                    })
                }
            };
        }


        public List<CommandItem> Commands { get; }
        public List<JobInfo> Jobs { get; } = new List<JobInfo>();
        public List<JobLog> Logs { get; } = new List<JobLog>();


        void LoadJobs()
        {
            var jobs = CrossJobs.Current.GetJobs();
        }


        void LoadLogs()
        {
            var logs = CrossJobs.Current.GetLogs();
        }
    }
}
