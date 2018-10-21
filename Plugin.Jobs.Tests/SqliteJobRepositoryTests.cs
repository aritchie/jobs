using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Plugin.Jobs.Infrastructure;
using Xunit;


namespace Plugin.Jobs.Tests
{

    
    public class SqliteJobRepositoryTests
    {
        [Fact]
        public void JobParametersSerialization()
        {
            var repo = this.GetFreshRepository();
            repo.Persist(new JobInfo
            {
                Name = nameof(JobParametersSerialization),
                Type = typeof(SqliteJobRepositoryTestJob),
                Parameters = new Dictionary<string, object>
                {
                    { "String", "TestString" },
                    { "Int", Int32.MaxValue },
                    { 
                        "Collection", 
                        new List<JobInfo>
                        {
                            new JobInfo { Name = "Hi" },
                            new JobInfo { Name = "Hi2" }
                        }
                    }
                }
            }, false);

            var job = repo.GetByName(nameof(JobParametersSerialization));
            job.Name.Should().Be(nameof(JobParametersSerialization));
            job.Type.Should().Be(typeof(SqliteJobRepositoryTestJob));
            job.Parameters.Count.Should().Be(3);
            job.Parameters["String"].Should().Be("TestString");
            job.Parameters["Int"].Should().Be(Int32.MaxValue);
            
            var jobs = job.GetValue<List<JobInfo>>("Collection");
            jobs.Should().NotBeNull();
            jobs.Count.Should().Be(2);
            jobs.First().Name.Should().Be("Hi");
        }



        SqliteJobRepository GetFreshRepository([CallerMemberName] string caller = null)
        {
            var dbPath = $"{caller}.db";
            if (File.Exists(dbPath))
                File.Delete(dbPath);
            
            var conn = new PluginSqliteConnection(dbPath);
            return new SqliteJobRepository(conn);
        }
        
        
        
    }
    
    public class SqliteJobRepositoryTestJob : IJob
    {
        public Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            return Task.CompletedTask;
        }
    }

}
