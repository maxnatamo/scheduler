using System;
using System.Threading;
using Xunit;

using Scheduler;
using Scheduler.Cron;
using Scheduler.Jobs;

namespace Scheduler.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Scheduler.InitializeScheduler();

            // This job is a one-shot job. This means that the job will execute once,
            // then get removed from the job pool.
            Scheduler.AddJob( "One-time job", () => {
                Console.WriteLine( "This is a one-shot job.");
            } );

            // This job is a repeating job, meaning it
            // runs every cycle of the supplied Cron expression.
            Scheduler.AddJob( "GitHub-Updater", () => {
                Console.WriteLine( "[" + DateTime.Now.ToString("F") + "] GitHub updated.");
            }, new CronExpression("* * * *") );

            // This job is a delayed job. Like the one-shot job, it only runs once,
            // but with a delay.
            Scheduler.AddJob( "Schedule logger", () => {
                Console.WriteLine( "Current amount of jobs in job pool: " + Scheduler.GetJobCount() + ".");
            }, JobDelay.FromMinutes( 1 ) );

            Scheduler.HaltMainThread();
        }
    }
}
