using System;
using System.IO;
using Acr.IO;
using SQLite;


namespace Plugin.Jobs.Infrastructure
{
    public class PluginSqliteConnection : SQLiteConnectionWithLock
    {
        public PluginSqliteConnection(string path = null) : base(
            new SQLiteConnectionString(
                path ?? Path.Combine(FileSystem.Current.AppData.FullName, "jobsplugin_v11.db"),
                true,
                null
            ),
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.FullMutex |
            SQLiteOpenFlags.PrivateCache
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
        public bool DeviceCharging { get; set; }
        public bool BatteryNotLow { get; set; }
        public int RequiredNetwork { get; set; }
        public bool Repeat { get; set; }
        public string Payload { get; set; }
    }


    public class DbJobLog : JobLog
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}
