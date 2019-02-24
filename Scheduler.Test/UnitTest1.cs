using System;
using System.Threading;
using Xunit;

using Scheduler;

namespace Scheduler.Test
{
    public class UnitTest1
    {
        static void Process( Object o )
        {
            Console.WriteLine("Process run.");
        }

        [Fact]
        public void Test1()
        {
            Scheduler.InitializeScheduler();

            Scheduler.AddJob( "GitHub-Updater", () => {
                Console.WriteLine("GitHub updated.");
                Thread.Sleep( 5000 );
            }, (Scheduler.Cron)10 );

            Scheduler.AddJob( "JobCount", () => {
                Console.WriteLine("Job count: " + Scheduler.GetJobCount());
            }, Cron.Secondly );

            new ManualResetEvent(false).WaitOne();
        }
    }
}
