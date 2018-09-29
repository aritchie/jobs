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
            LastRunUtc = x.LastRunUtc,
            //RunPeriodic = x.RunPeriodic,
            //DeviceIdle = x.DeviceIdle,
            BatteryNotLow = x.BatteryNotLow,
            DeviceCharging = x.DeviceCharging,
            RequiredNetwork = (NetworkType)x.RequiredNetwork,
            Parameters = this.FromPayload(x.Payload)
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


        // TODO: yes this was lazy :)
        public JobInfo GetByName(string jobName) => this
            .GetJobs()
            .FirstOrDefault(x => x.Name.Equals(jobName, StringComparison.CurrentCultureIgnoreCase));


        // logs are left for now
        public void Cancel(string jobName) => this.conn.Delete<DbJobInfo>(jobName);
        public void CancelAll() => this.conn.DeleteAll<DbJobInfo>();


        public void Create(JobInfo jobInfo) => this.conn.Insert(new DbJobInfo
        {
            Name = jobInfo.Name,
            TypeName = jobInfo.Type.AssemblyQualifiedName,
            BatteryNotLow = jobInfo.BatteryNotLow,
            DeviceCharging = jobInfo.DeviceCharging,
            RequiredNetwork = (int)jobInfo.RequiredNetwork,
            Payload = this.ToPayload(jobInfo.Parameters)
        });


        public void Update(JobInfo jobInfo)
        {
            var dbJob = this.conn.Get<DbJobInfo>(jobInfo.Name);
            dbJob.TypeName = jobInfo.Type.AssemblyQualifiedName;
            dbJob.BatteryNotLow = jobInfo.BatteryNotLow;
            dbJob.DeviceCharging = jobInfo.DeviceCharging;
            dbJob.RequiredNetwork = (int)jobInfo.RequiredNetwork;
            dbJob.Payload = this.ToPayload(jobInfo.Parameters);
            dbJob.LastRunUtc = jobInfo.LastRunUtc;

            this.conn.Update(dbJob);
        }


        public void Log(JobLog log) => this.conn.Insert(new DbJobLog
        {
            Status = log.Status,
            CreatedOn = log.CreatedOn,
            JobName = log.JobName,
            Error = log.Error
        });


        protected virtual IJobParameters FromPayload(string payload)
        {
            var dict = new Dictionary<string, string>();
            if (!String.IsNullOrWhiteSpace(payload))
            {
                var pairs = payload.Split(';');
                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split(':');
                    if (keyValue.Length > 0)
                        dict.Add(keyValue[0], keyValue[1]);
                }
            }
            return new JobParameters(dict);
        }


        protected virtual string ToPayload(IJobParameters parameters)
        {
            var s = "";
            foreach (var key in parameters.Keys)
            {
                var value = parameters.Get(key, "");
                s += $"{key}:{value};";
            }
            s = s.TrimEnd(';');
            return s;
        }
    }
}
