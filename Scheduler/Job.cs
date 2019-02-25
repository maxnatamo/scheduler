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


    /// <summary>
    /// The main job class.
    /// This class is a main building block for the scheduler,
    /// as it holds all the required methods and values.
    /// </summary>
    public class Job
    {
        public string name;
        protected Action methodCall;
        protected Action continuationCall;
        protected CronExpression cron;
        protected DateTime nextRunTime;
        protected JobType jobType;

        public Job( string name, Action methodCall )
        {
            this.name           = name;
            this.methodCall     = methodCall;
            this.jobType        = JobType.OneShot;
        }

        public Job( string name, Action methodCall, CronExpression cron )
        {
            this.name           = name;
            this.methodCall     = methodCall;
            this.cron           = cron;
            this.nextRunTime    = cron.GetNextRunTime();
            this.jobType        = JobType.Scheduled;
        }

        public Job( string name, Action methodCall, DateTime delay )
        {
            this.name           = name;
            this.methodCall     = methodCall;
            this.nextRunTime    = delay;
            this.jobType        = JobType.Delayed;
        }

        public void RunTask()
        {
            if( this.jobType == JobType.Scheduled )
            {
                // Checks whether we've reached the nextRunTime.
                // We also check whether the current DateTime's Second variable
                // is zero, so the method call isn't executed every second.
                if( DateTime.Now > this.nextRunTime && DateTime.Now.Second == 0 )
                {
                    methodCall();
                    this.nextRunTime = cron.GetNextRunTime();
                    return;
                }
            } else {
                if( this.jobType == JobType.OneShot ) {
                    // If the job is a oneshot, it will be removed off the job pool,
                    // whenever it's done executing.
                    this.methodCall();
                    Scheduler.DeleteJob( this.name );
                }
                else if( this.jobType == JobType.Delayed )
                {
                    if( DateTime.Now > this.nextRunTime )
                    {
                        this.methodCall();
                        Scheduler.DeleteJob( this.name );
                    }
                }

                // If the job is not a scheduled job, run the continuation method.
                if( this.continuationCall != null ) {
                    this.continuationCall();
                }
            }
        }

        /// <summary>
        /// Adds a continuation method to the current job.
        /// A continuation method is executed, when the parent job
        /// is finished executing. This method is not available for scheduled jobs.
        /// <param name="continuationCall">The method to call, when the job has finished executing.</param>
        /// <returns>False if the job is scheduled, true if the method was added successfully.</returns>
        /// </summary>
        public bool AddContinuationCall( Action continuationCall )
        {
            // Continuation calls are not available for scheduled jobs,
            // as they never finish executing.
            if( this.jobType == JobType.Scheduled ) {
                return false;
            }

            this.continuationCall = continuationCall;
            return true;
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