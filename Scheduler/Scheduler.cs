using System;
using System.Threading;
using System.Collections.Generic;

using Scheduler.Jobs;
using Scheduler.Cron;

namespace Scheduler
{
    public static class Scheduler
    {
        static List<Job> jobPool = new List<Job>();
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        public static void InitializeScheduler()
        {
            ThreadPool.QueueUserWorkItem( new WaitCallback( ( Object o ) => {
                for(;;) {
                    foreach(var job in jobPool.ToArray()) {
                        new Thread(() => job.RunTask() ).Start();
                    }
                    Thread.Sleep(1000);
                }
            } ) );
        }

        /// <summary>
        /// Add a job to the job pool.
        /// <param name="name">The identifier of the job.</param>
        /// <param name="methodCall">The action to call.</param>
        /// </summary>
        public static void AddJob( string name, Action methodCall )
        {
            if( JobPoolHasJob( name ) ) {
                throw new Exception("Job identifier has already been added to the job pool.");
            }

            jobPool.Add( new Job( name, methodCall ) );
            Console.WriteLine( "[LOG] Job added: '" + name + "'." );
        }

        /// <summary>
        /// Add a job to the job pool, with repeating execution.
        /// <param name="name">The identifier of the job.</param>
        /// <param name="methodCall">The action to call.</param>
        /// <param name="cron">The Cron expression. This is the 'time-table' of the job and determines when the job will be run.</param>
        /// </summary>
        public static void AddJob( string name, Action methodCall, CronExpression cron )
        {
            if( JobPoolHasJob( name ) ) {
                throw new Exception("Job identifier has already been added to the job pool.");
            }

            jobPool.Add( new Job( name, methodCall, cron ) );
            Console.WriteLine( "[LOG] Job added: '" + name + "'." );
        }

        /// <summary>
        /// Add a job to the job pool, with delayed exection.
        /// <param name="name">The identifier of the job.</param>
        /// <param name="methodCall">The action to call.</param>
        /// <param name="jobDelay">The delay of the job.</param>
        /// </summary>
        public static void AddJob( string name, Action methodCall, DateTime jobDelay )
        {
            if( JobPoolHasJob( name ) ) {
                throw new Exception("Job identifier has already been added to the job pool.");
            }

            jobPool.Add( new Job( name, methodCall, jobDelay ) );
            Console.WriteLine( "[LOG] Job added: '" + name + "' has been added to the job pool." );
        }

        /// <summary>
        /// Removes a job from the job pool, using the name identifier.
        /// <param name="name">The name of the job, to remove.</param>
        /// <returns>True if job is found and removed, false otherwise.</returns>
        /// </summary>
        public static bool DeleteJob( string name )
        {
            for( int i = 0; i < jobPool.Count; i++ ) {
                if (jobPool[i].name == name) {
                    Console.WriteLine("[LOG] Job '" + name + "' has been deleted from the job.");
                    jobPool.RemoveAt( i );
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether the specified job is in the job pool.
        /// <param name="jobName">The job name to check for.</param>
        /// </summary>
        public static bool JobPoolHasJob ( string jobName )
        {
            foreach( var job in jobPool ) {
                if( job.name == jobName ) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clear the job pool and shutdown the scheduler.
        /// </summary>
        public static void Shutdown( )
        {
            jobPool.Clear();
            resetEvent.Set();
        }

        /// <summary>
        /// The the amount of jobs in the current job pool.
        /// </summary>
        public static int GetJobCount()
        {
            return jobPool.Count;
        }

        /// <summary>
        /// Halts the main thread, if nothing is halting it from exiting.
        /// The main thread will continue, once the jobpool is cleared or empty.
        /// </summary>
        public static void HaltMainThread()
        {
            resetEvent.WaitOne();
        }
    } 
}