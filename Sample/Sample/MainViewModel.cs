using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr;
using Acr.UserDialogs;
using Plugin.Jobs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Sample
{
    public class MainViewModel : ReactiveObject
    {
        readonly IJobManager jobManager;
        readonly IUserDialogs dialogs;


        public MainViewModel()
        {
            this.jobManager = CrossJobs.Current;
            this.dialogs = UserDialogs.Instance;

            var valObs = this.WhenAny(
                x => x.JobName,
                x => x.JobLoopCount,
                (name, loops) =>
                    !name.GetValue().IsEmpty() &&
                    loops.GetValue() >= 10
            );

            this.CreateJob = ReactiveCommand.Create(
                () =>
                {
                    CrossJobs.Current.Schedule(new JobInfo
                    {
                        Name = this.JobName.Trim(),
                        BatteryNotLow = this.BatteryNotLow,
                        DeviceCharging = this.DeviceCharging,
                        // NetworkType
                        Parameters = new Dictionary<string, object>
                        {
                            { "LoopCount", this.JobLoopCount }
                        }
                    });
                    this.LoadJobs.Execute(null);
                },
                valObs
            );

            this.RunAsTask = ReactiveCommand.Create(
                () =>
                {
                    // TODO: need to get a var that tells me this is running/finished
                    this.jobManager.RunTask(async () =>
                    {
                        for (var i = 0; i < this.JobLoopCount; i++)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);    
                        }
                    });
                },
                valObs
            );

            this.RunAllJobs = ReactiveCommand.Create(() =>
            {
                if (this.jobManager.IsRunning)
                    this.dialogs.Alert("Job Manager is already running");
                else
                    this.jobManager.Run();
            });

            this.CancelAllJobs = ReactiveCommand.Create(() =>
            {
                this.jobManager.CancelAll();
                this.LoadJobs.Execute(null);
            });

            this.LoadJobs = ReactiveCommand.Create(() =>
            {
                this.Jobs = this.jobManager
                    .GetJobs()
                    .Select(x => new CommandItem
                    {
                        Text = x.Name,
                        Detail = x.LastRunUtc?.ToString("R") ?? "Never Run",
                        PrimaryCommand = ReactiveCommand.CreateFromTask(async () =>
                        {
                            try
                            {
                                using (this.dialogs.Loading("Running Job " + x.Name))
                                await this.jobManager.Run(x.Name);
                            }
                            catch (Exception ex)
                            {
                                this.dialogs.Alert(ex.ToString());
                            }
                            this.LoadLogs.Execute(null);
                        }),
                        SecondaryCommand = ReactiveCommand.Create(() =>
                        {
                            this.jobManager.Cancel(x.Name);
                            this.LoadLogs.Execute(null);
                        })
                    })
                    .ToList();
            });

            this.LoadLogs = ReactiveCommand.Create(() =>
            {
                this.Logs = CrossJobs
                    .Current
                    .GetLogs()
                    .Select(x => new CommandItem
                    {
                        Text = x.JobName,
                        Detail = $"[{x.Status}] {x.CreatedOn:R}",
                        PrimaryCommand = ReactiveCommand.Create(() =>
                        {
                            if (x.Status == JobState.Error)
                                this.dialogs.Alert(x.Error, x.JobName);
                        })
                    })
                    .ToList();
            });
        }


        public void OnAppearing()
        {
            this.LoadJobs.Execute(null);
            this.LoadLogs.Execute(null);
        }


        public void OnDisappearing()
        {
        }


        public ICommand LoadJobs { get; }
        public ICommand LoadLogs { get; }
        public ICommand CancelAllJobs { get; }
        public ICommand CreateJob { get; }
        public ICommand RunAsTask { get; }
        public ICommand RunAllJobs { get; }

        public List<CommandItem> Jobs { get; private set; }
        public List<CommandItem> Logs { get; private set; }
        [Reactive] public string JobName { get; set; } = "TestJob";
        [Reactive] public int JobLoopCount { get; set; } = 100;
        [Reactive] public bool HasInternet { get; set; }
        [Reactive] public bool HasWiFi { get; set; }
        [Reactive] public bool BatteryNotLow { get; set; }
        [Reactive] public bool DeviceCharging { get; set; }
    }
}
