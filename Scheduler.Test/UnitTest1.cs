using System;
using System.Threading;
using Xunit;

using Scheduler;
using Scheduler.Cron;

namespace Scheduler.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Scheduler.InitializeScheduler();

            Scheduler.AddJob( "GitHub-Updater", () => {
                Console.WriteLine( "[" + DateTime.Now.ToString("F") + "] GitHub updated.");
            }, new CronExpression("* * * *") );

            Scheduler.HaltMainThread();
        }
    }
}
