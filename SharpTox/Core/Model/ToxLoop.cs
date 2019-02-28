using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using SharpTox.Core.Interfaces;

namespace SharpTox.Core
{
    public static class ToxLoop
    {
        public static IDisposable Start([NotNull] IToxIterate toxIterate, IScheduler scheduler = null)
        {
            SerialDisposable serial = new SerialDisposable();
            scheduler = scheduler ?? Scheduler.Default;
            serial.Disposable = scheduler.Schedule(Iterate);
            return serial;

            void Iterate()
            {
                var time = toxIterate.Iterate();
                serial.Disposable = scheduler.Schedule(time, Iterate);
            }
        }
    }
}
