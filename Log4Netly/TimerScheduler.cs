using System;
using System.Timers;

namespace Log4Netly
{
    public class TimerScheduler
    {
        private readonly int _intervalInMs;
        private Timer _timer;

        public TimerScheduler(int intervalInMs)
        {
            _intervalInMs = intervalInMs;
        }

        public void Execute(Action action)
        {
            _timer = new Timer(_intervalInMs);
            _timer.Elapsed += (sender, args) => action();
            _timer.AutoReset = true;
            _timer.Start();
        }
    }
}