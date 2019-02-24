using System;
using System.Threading;
using Scheduler.Cron;

namespace Scheduler.Jobs
{
    public enum JobType {
        OneShot,
        Scheduled,
        Delayed
    };

    public class Job
    {
        public string name;
        protected Action methodCall;
        protected CronExpression cron;
        protected DateTime nextRunTime;
        protected JobType jobType;

        public Job( string name, Action methodCall )
        {
            this.name           = name;
            this.methodCall     = methodCall;

            jobType = JobType.OneShot;
        }

        public Job( string name, Action methodCall, CronExpression cron )
        {
            this.name           = name;
            this.methodCall     = methodCall;
            this.cron           = cron;

            nextRunTime = cron.GetNextRunTime();
            jobType = JobType.Scheduled;
        }

        public Job( string name, Action methodCall, DateTime delay )
        {
            this.name           = name;
            this.methodCall     = methodCall;

            nextRunTime = delay;
            jobType = JobType.Delayed;
        }

        public void RunTask()
        {
            if( jobType == JobType.OneShot )
            {
                // If the job is a oneshot, it will be removed off the job pool,
                // whenever it's done executing.
                methodCall();
                Scheduler.DeleteJob( name );
                return;
            }
            else if( jobType == JobType.Delayed )
            {
                if( DateTime.Now > nextRunTime )
                {
                    methodCall();
                    Scheduler.DeleteJob( name );
                }
                return;
            }
            else if( jobType == JobType.Scheduled )
            {
                // Checks whether we've reached the nextRunTime.
                // We also check whether the current DateTime's Second variable
                // is zero, so the method call isn't executed every second.
                if( DateTime.Now > nextRunTime && DateTime.Now.Second == 0 )
                {
                    methodCall();
                    nextRunTime = cron.GetNextRunTime();
                    return;
                }
            }
        }
    }

    public static class JobDelay
    {
        public static DateTime FromMinutes( int minutes )
        {
            return DateTime.Now.AddMinutes( minutes );
        }

        public static DateTime FromHours( int hours )
        {
            return DateTime.Now.AddHours( hours );
        }

        public static DateTime FromDays( int days )
        {
            return DateTime.Now.AddDays( days );
        }

        public static DateTime FromMonths( int months )
        {
            return DateTime.Now.AddMonths( months );
        }
    }
}