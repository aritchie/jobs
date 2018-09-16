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
using Sample.Jobs;


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
                        Type = typeof(SampleJob),
                        BatteryNotLow = this.BatteryNotLow,
                        DeviceCharging = this.DeviceCharging,
                        // TODO: NetworkType
                        Parameters = new Dictionary<string, object>
                        {
                            { "LoopCount", this.JobLoopCount }
                        }
                    });
                    this.LoadJobs.Execute(null);
                    this.dialogs.Toast("Job Created");
                },
                valObs
            );

            this.RunAsTask = ReactiveCommand.Create(
                () =>
                {
                    // TODO: need to get a var that tells me this is running/finished
                    this.jobManager.RunTask(async () =>
                    {
                        this.dialogs.Toast("Task Started");
                        for (var i = 0; i < this.JobLoopCount; i++)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);
                        }
                        this.dialogs.Toast("Task Finished");
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
                this.IsBusy = true;
                this.Jobs = this.jobManager
                    .GetJobs()
                    .Select(x => new CommandItem
                    {
                        Text = x.Name,
                        Detail = x.LastRunUtc?.ToLocalTime().ToString("R") ?? "Never Run",
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
                this.RaisePropertyChanged(nameof(this.Jobs));
                this.IsBusy = false;
            });

            this.LoadLogs = ReactiveCommand.Create(() =>
            {
                this.IsBusy = true;
                this.Logs = CrossJobs
                    .Current
                    .GetLogs()
                    .Select(x => new CommandItem
                    {
                        Text = x.JobName,
                        Detail = $"[{x.Status}] {x.CreatedOn.ToLocalTime():R}",
                        PrimaryCommand = ReactiveCommand.Create(() =>
                        {
                            if (x.Status == JobState.Error)
                                this.dialogs.Alert(x.Error, x.JobName);
                        })
                    })
                    .ToList();
                this.RaisePropertyChanged(nameof(this.Logs));
                this.IsBusy = false;
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
        [Reactive] public int JobLoopCount { get; set; } = 10;
        [Reactive] public bool HasInternet { get; set; }
        [Reactive] public bool HasWiFi { get; set; }
        [Reactive] public bool BatteryNotLow { get; set; }
        [Reactive] public bool DeviceCharging { get; set; }
        [Reactive] public bool IsBusy { get; set; }
    }
}
