using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainViewModel()
        {
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
                this.WhenAny(
                    x => x.JobName,
                    x => x.JobLoopCount,
                    (name, loops) =>
                        !name.GetValue().IsEmpty() &&
                        loops.GetValue() >= 10
                )
            );

            this.CancelAllJobs = ReactiveCommand.Create(() =>
            {
                CrossJobs.Current.CancelAll();
                this.LoadJobs.Execute(null);
            });

            this.LoadJobs = ReactiveCommand.Create(() =>
            {
                this.Jobs = CrossJobs.Current.GetJobs()
                    .Select(x => new CommandItem
                    {
                        Text = x.Name,
                        Detail = x.LastRunUtc?.ToString("R") ?? "Never Run",
                        Command = ReactiveCommand.Create(() =>
                        {
                            UserDialogs.Instance.ActionSheet(new ActionSheetConfig()
                                .SetTitle("Job: " + x.Name)
                                .Add("Run", async () =>
                                {
                                    try
                                    {
                                        using (UserDialogs.Instance.Loading("Running Job " + x.Name))
                                            await CrossJobs.Current.Run(x.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        UserDialogs.Instance.Alert(ex.ToString());
                                    }
                                    this.LoadLogs.Execute(null);
                                })
                                .Add("Cancel", () =>
                                {
                                    CrossJobs.Current.Cancel(x.Name);
                                    this.LoadJobs.Execute(null);
                                })
                                .SetCancel()
                            );
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
                        Command = ReactiveCommand.Create(() =>
                        {
                            if (x.Status == JobState.Error)
                                UserDialogs.Instance.Alert(x.Error, x.JobName);
                        })
                    })
                    .ToList();
            });

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


        public void OnAppearing()
        {
            this.LoadJobs.Execute(null);
            this.LoadLogs.Execute(null);
        }


        public void OnDisappearing()
        {
        }

        public List<CommandItem> Commands { get; }
        public List<CommandItem> Jobs { get; private set; }
        public List<CommandItem> Logs { get; private set; }

        public ICommand LoadJobs { get; }
        public ICommand LoadLogs { get; }
        public ICommand CancelAllJobs { get; }
        public ICommand CreateJob { get; }
        [Reactive] public string JobName { get; set; }
        [Reactive] public int JobLoopCount { get; set; }
        [Reactive] public bool HasInternet { get; set; }
        [Reactive] public bool HasWiFi { get; set; }
        [Reactive] public bool BatteryNotLow { get; set; }
        [Reactive] public bool DeviceCharging { get; set; }
    }
}
