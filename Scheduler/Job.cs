using System;
using System.Threading;

namespace Scheduler.Jobs
{
    public class Job
    {
        public string name;
        protected Action methodCall;
        protected Cron cron;

        private DateTime nextRun;

        public Job( string _name, Action _methodCall, Cron _cron )
        {
            name = _name;
            methodCall = _methodCall;
            cron = _cron;
        }

        public void RunTask()
        {
            if((DateTime.Now - nextRun).TotalSeconds >= 0) {
                nextRun = DateTime.Now + new TimeSpan( (int)cron * TimeSpan.TicksPerSecond );
                methodCall();
            }
        }
    }
}