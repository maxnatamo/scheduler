using System;
using System.Threading;
using Scheduler.Cron;

namespace Scheduler.Jobs
{
    public class Job
    {
        public string name;
        protected Action methodCall;
        protected CronExpression cron;
        protected DateTime nextRunTime;

        public Job( string name, Action methodCall, CronExpression cron )
        {
            this.name           = name;
            this.methodCall     = methodCall;
            this.cron           = cron;

            nextRunTime = cron.GetNextRunTime();
        }

        public void RunTask()
        {
            // Checks whether we've reached the nextRunTime.
            // We also check whether the current DateTime's Second variable
            // is zero, so the method call isn't executed every second.
            if( DateTime.Now > nextRunTime && DateTime.Now.Second == 0 ) {
                nextRunTime = cron.GetNextRunTime();
                methodCall();
            }
        }
    }
}