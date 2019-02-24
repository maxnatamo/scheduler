using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.CronExpression
{
    public enum Cron {
        Secondly    = 1,
        Minutely    = 60,
        Hourly      = 3600,
        Daily       = 84600
    };

    public static class CronExpression
    { }
}