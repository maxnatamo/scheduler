using System;
using System.Threading;
using System.Collections.Generic;

using Scheduler.Jobs;
using Scheduler.CronExpression;

namespace Scheduler
{
    public static class Scheduler
    {
        static List<Job> jobPool = new List<Job>();

        public static void InitializeScheduler()
        {
            ThreadPool.QueueUserWorkItem( new WaitCallback( ( Object o ) => {
                for(;;)
                {
                    foreach(var job in jobPool)
                    {
                        new Thread(() => job.RunTask() ).Start();
                    }
                    Thread.Sleep(1000);
                }
            } ) );
        }

        public static void AddJob( string name, Action methodCall, Cron cron )
        {
            jobPool.Add( new Job( name, methodCall, cron ) );
            Console.WriteLine( "[LOG] Job added: '" + name + "'." );
        }

        public static bool DeleteJob( string name )
        {
            for( int i = 0; i < jobPool.Count; i++ ) {
                if (jobPool[i].name == name) {
                    jobPool.RemoveAt( i );
                    return true;
                }
            }
            return false;
        }

        public static int GetJobCount()
        {
            return jobPool.Count;
        }
    }
}