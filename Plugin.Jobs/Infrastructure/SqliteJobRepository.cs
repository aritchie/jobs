using System;
using System.Collections.Generic;
using System.Linq;


namespace Plugin.Jobs.Infrastructure
{
    public class SqliteJobRepository : IJobRepository
    {
        readonly PluginSqliteConnection conn = new PluginSqliteConnection();


        public IEnumerable<JobInfo> GetJobs() => this.conn.Jobs.ToList().Select(x => new JobInfo
        {
            Name = x.Name,
            Type = Type.GetType(x.TypeName),
            RunPeriodic = x.RunPeriodic,
            BatteryNotLow = x.BatteryNotLow,
            DeviceCharging = x.DeviceCharging,
            DeviceIdle = x.DeviceIdle,
            RequiredNetwork = (NetworkType)x.RequiredNetwork
            //Parameters = null
            //Payload = null // TODO: serialize and deserialize, I don't want a key/value table
        });


        public IEnumerable<JobLog> GetLogs(string jobName = null, DateTime? since = null, bool failedOnly = false)
        {
            var query = this.conn.Logs.AsQueryable();
            if (!String.IsNullOrWhiteSpace(jobName))
                query = query.Where(x => x.JobName == jobName);

            if (since != null)
                query = query.Where(x => x.CreatedOn >= since.Value);

            if (failedOnly)
                query = query.Where(x => x.Error != "");

            return query.ToList();
        }


        public DateTime? GetLastRuntime(string jobName)
        {
            var dt = this.conn
                .Logs
                .Where(x => x.JobName == jobName)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => x.CreatedOn)
                .FirstOrDefault();

            if (dt == DateTime.MinValue)
                return null;

            return dt;
        }


        // logs are left for now
        public void Cancel(string jobName) => this.conn.Delete<DbJobInfo>(jobName);
        public void CancelAll() => this.conn.DeleteAll<DbJobInfo>();


        public void Create(JobInfo jobInfo) => this.conn.Insert(new DbJobInfo
        {
            Name = jobInfo.Name,
            TypeName = jobInfo.Type.FullName,
            RunPeriodic = jobInfo.RunPeriodic,
            BatteryNotLow = jobInfo.BatteryNotLow,
            DeviceCharging = jobInfo.DeviceCharging,
            DeviceIdle = jobInfo.DeviceIdle,
            RequiredNetwork = (int)jobInfo.RequiredNetwork,
            Payload = null // TODO: serialize and deserialize, I don't want a key/value table
        });


        public void Log(JobLog log) => this.conn.Insert(new DbJobLog
        {
            Status = log.Status,
            CreatedOn = log.CreatedOn,
            JobName = log.JobName,
            Error = log.Error
        });
    }
}
