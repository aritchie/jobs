using System;
using System.IO;
using Acr.IO;
using SQLite;


namespace Plugin.Jobs.Infrastructure
{
    public class PluginSqliteConnection : SQLiteConnectionWithLock
    {
        public PluginSqliteConnection() : base(
            new SQLiteConnectionString(
                Path.Combine(FileSystem.Current.AppData.FullName, "jobsplugin.db"),
                true,
                null
            ),
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create
        )
        {
            this.CreateTable<DbJobInfo>();
            this.CreateTable<DbJobLog>();
        }


        public TableQuery<DbJobInfo> Jobs => this.Table<DbJobInfo>();
        public TableQuery<DbJobLog> Logs => this.Table<DbJobLog>();

    }


    public class DbJobInfo
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string TypeName { get; set; }

        public DateTime? LastRunUtc { get; set; }
        //public bool RunPeriodic { get; set; }
        //public bool DeviceIdle { get; set; } // this will only work on droid
        public bool DeviceCharging { get; set; }
        public bool BatteryNotLow { get; set; }
        public int RequiredNetwork { get; set; }
        public string Payload { get; set; }
        //public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }


    public class DbJobLog : JobLog
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}
