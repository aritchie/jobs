using System;
using System.Timers;


namespace Plugin.Jobs
{
    public class JobManagerImpl : AbstractJobManager
    {
        readonly Timer timer;


        public JobManagerImpl(IJobRepository repository = null, IJobFactory factory = null) : base(repository, factory)
        {
            // TODO: need to remove Xamarin.Essentials from the equation for this to work
            this.timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            this.timer.Elapsed += async (sender, args) =>
            {
                this.timer.Stop();
                await this.RunAll().ConfigureAwait(false);
                this.timer.Start();
            };
            this.timer.Start();
        }
    }
}
