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

            this.CreateJob = ReactiveCommand.CreateFromTask(
                async _ =>
                {
                    var job = new JobInfo
                    {
                        Name = this.JobName.Trim(),
                        Type = typeof(SampleJob),
                        Repeat = this.Repeat,
                        BatteryNotLow = this.BatteryNotLow,
                        DeviceCharging = this.DeviceCharging,
                        RequiredNetwork = (NetworkType)Enum.Parse(typeof(NetworkType), this.NetworkType)
                    };
                    job.SetValue("LoopCount", this.JobLoopCount);
                    await this.jobManager.Schedule(job);

                    this.LoadJobs.Execute(null);
                    this.dialogs.Toast("Job Created Successfully");
                },
                valObs
            );

            this.RunAsTask = ReactiveCommand.Create(
                () => this.jobManager.RunTask(this.JobName + "Task", async _ =>
                {
                    this.dialogs.Toast("Task Started");
                    for (var i = 0; i < this.JobLoopCount; i++)
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                    this.dialogs.Toast("Task Finished");
                }),
                valObs
            );

            this.ChangeNetworkType = ReactiveCommand.Create(() =>
            {
                var cfg = new ActionSheetConfig()
                    .Add(
                        Plugin.Jobs.NetworkType.None.ToString(),
                        () => this.NetworkType = Plugin.Jobs.NetworkType.None.ToString()
                    )
                    .Add(
                        Plugin.Jobs.NetworkType.Any.ToString(),
                        () => this.NetworkType = Plugin.Jobs.NetworkType.Any.ToString()
                    )
                    .Add(
                        Plugin.Jobs.NetworkType.WiFi.ToString(),
                        () => this.NetworkType = Plugin.Jobs.NetworkType.WiFi.ToString()
                    )
                    .SetCancel();
                this.dialogs.ActionSheet(cfg);
            });

            this.RunAllJobs = ReactiveCommand.Create(() =>
            {
                if (!this.AssertJobs())
                    return;

                if (this.jobManager.IsRunning)
                    this.dialogs.Alert("Job Manager is already running");
                else
                {
                    this.dialogs.Toast("Job Batch Started");
                    this.jobManager.RunAll();
                }
            });

            this.PurgeOldLogs = ReactiveCommand.CreateFromTask(() => this.PurgeLogs(false));

            this.PurgeAllLogs = ReactiveCommand.CreateFromTask(() => this.PurgeLogs(true));

            this.CancelAllJobs = ReactiveCommand.CreateFromTask(async _ =>
            {
                if (!this.AssertJobs())
                    return;

                var confirm = await this.dialogs.ConfirmAsync("Are you sure you wish to cancel all jobs?");
                if (confirm)
                {
                    this.jobManager.CancelAll();
                    this.LoadJobs.Execute(null);
                }
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
                this.Logs = this.jobManager
                    .GetLogs()
                    .OrderByDescending(x => x.CreatedOn)
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


        public async void OnAppearing()
        {
            this.LoadJobs.Execute(null);
            this.LoadLogs.Execute(null);

            this.jobManager.JobStarted += this.OnJobStarted;
            this.jobManager.JobFinished += this.OnJobFinished;
            this.HasPermissions = await this.jobManager.HasPermissions();
        }


        public void OnDisappearing()
        {
            this.jobManager.JobStarted -= this.OnJobStarted;
            this.jobManager.JobFinished -= this.OnJobFinished;
        }


        public ICommand LoadJobs { get; }
        public ICommand LoadLogs { get; }
        public ICommand CancelAllJobs { get; }
        public ICommand CreateJob { get; }
        public ICommand RunAsTask { get; }
        public ICommand RunAllJobs { get; }
        public ICommand ChangeNetworkType { get; }
        public ICommand PurgeOldLogs { get; }
        public ICommand PurgeAllLogs { get; }

        public List<CommandItem> Jobs { get; private set; }
        public List<CommandItem> Logs { get; private set; }
        [Reactive] public bool HasPermissions { get; private set; }
        [Reactive] public string JobName { get; set; } = "TestJob";
        [Reactive] public int JobLoopCount { get; set; } = 10;
        [Reactive] public string NetworkType { get; set; } = Plugin.Jobs.NetworkType.None.ToString();
        [Reactive] public bool BatteryNotLow { get; set; }
        [Reactive] public bool DeviceCharging { get; set; }
        [Reactive] public bool Repeat { get; set; } = true;
        [Reactive] public bool IsBusy { get; set; }


        void OnJobStarted(object sender, JobInfo job)
            => this.dialogs.Toast($"Job {job.Name} Started Running");


        void OnJobFinished(object sender, JobRunResult args)
        {
            if (args.Success)
                this.dialogs.Toast($"Job {args.Job.Name} Finished");
            else
                this.dialogs.Alert(args.Exception.ToString(), $"Job {args.Job.Name} Failed");

            this.LoadLogs.Execute(null);
            this.LoadJobs.Execute(null);
        }


        bool AssertJobs()
        {
            if (!this.jobManager.GetJobs().Any())
            {
                this.dialogs.Alert("There are no jobs");
                return false;
            }

            return true;
        }


        async Task PurgeLogs(bool all)
        {
            TimeSpan? age = null;
            if (!all)
                age = TimeSpan.FromHours(8);

            var msg = all ? "Are you sure you wish to purge all logs?" : "Do you want to purge logs older than 8 hours";
            var confirm = await this.dialogs.ConfirmAsync(msg);
            if (confirm)
            {
                this.jobManager.PurgeLogs(null, age);
                this.LoadLogs.Execute(null);
            }
        }
    }
}
