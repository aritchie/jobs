using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


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


        public void PurgeLogs(string jobName = null)
        {
            if (String.IsNullOrWhiteSpace(jobName))
                this.conn.DeleteAll<DbJobLog>();
            else
                this.conn.Logs.Delete(x => x.JobName == jobName);
        }


        public void TrimLogs(TimeSpan maxAge)
        {
            var date = DateTime.UtcNow.Subtract(maxAge);
            this.conn.Logs.Delete(x => x.CreatedOn < date);
        }


        // TODO: yes this was lazy :)
        public JobInfo GetByName(string jobName) => this
            .GetJobs()
            .FirstOrDefault(x => x.Name.Equals(jobName, StringComparison.CurrentCultureIgnoreCase));


        // logs are left for now
        public void Cancel(string jobName) => this.conn.Delete<DbJobInfo>(jobName);
        public void CancelAll() => this.conn.DeleteAll<DbJobInfo>();


        public void Persist(JobInfo jobInfo, bool updateDate)
        {
            var job = this.GetDbJob(jobInfo.Name);
            if (job == null)
            {
                this.conn.Insert(new DbJobInfo
                {
                    Name = jobInfo.Name,
                    TypeName = jobInfo.Type.AssemblyQualifiedName,
                    BatteryNotLow = jobInfo.BatteryNotLow,
                    DeviceCharging = jobInfo.DeviceCharging,
                    RequiredNetwork = (int) jobInfo.RequiredNetwork,
                    Payload = this.ToPayload(jobInfo.Parameters)
                });
            }
            else
            {
                job.TypeName = jobInfo.Type.AssemblyQualifiedName;
                job.BatteryNotLow = jobInfo.BatteryNotLow;
                job.DeviceCharging = jobInfo.DeviceCharging;
                job.RequiredNetwork = (int)jobInfo.RequiredNetwork;
                job.Payload = this.ToPayload(jobInfo.Parameters);
                if (updateDate)
                    job.LastRunUtc = jobInfo.LastRunUtc;

                this.conn.Update(job);
            }
        }


        public void Log(JobLog log) => this.conn.Insert(new DbJobLog
        {
            Status = log.Status,
            CreatedOn = log.CreatedOn,
            JobName = log.JobName,
            Error = log.Error
        });


        protected DbJobInfo GetDbJob(string name) => this.conn.Find<DbJobInfo>(name);

        protected virtual IDictionary<string, object> FromPayload(string payload)
        {
            if (String.IsNullOrWhiteSpace(payload))
                return new Dictionary<string, object>();

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
        }


        protected virtual string ToPayload(IDictionary<string, object> parameters)
            => JsonConvert.SerializeObject(parameters);
    }
}
