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

            // You can also use continuation calls. If set, the continuation method
            // will execute after the job has finished. Therefore, it is not available for
            // scheduled jobs, as they never finish. 
            Scheduler.ContinueWith( "One-time job", () => {
                Console.WriteLine( "The one-shot job has finished executing." );
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

            // Scheduler.HaltMainThread() is used to halt the main thread,
            // if there are no other halting statements.
            // This prevents the program from exiting, before all jobs are run.
            // The main thread will continue, when the job pool is empty.
            Scheduler.HaltMainThread();
        }
    }
}
