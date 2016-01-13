using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatrixIO.IO
{
    public class TimeAverage : IDisposable
    {
        private readonly object _syncObject = new object();
        private readonly Timer _timer;
        private readonly List<long> _previousCounts = new List<long>(301);
        private long _currentCount = 0;
        private bool _running = false;

        public TimeAverage()
        {

            _timer = new Timer(new TimerCallback(TimeElapsed), this, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            lock (_syncObject)
            {
                _timer.Change(1000, 1000);
                _running = true;
            }
        }

        public void Stop()
        {
            lock(_syncObject)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _running = false;
            }
        }

        public void TimeElapsed(object state)
        {
            lock(_syncObject)
            {
                if (!_running) return;

                var lastBytes = _currentCount;
                _currentCount = 0;

                _previousCounts.Add(lastBytes);
                if(_previousCounts.Count > 300)
                    _previousCounts.RemoveRange(0, _previousCounts.Count - 300);
            }
        }

        public void Add(long number)
        {
            if (!_running) return;
            lock(_syncObject) _currentCount += number;
        }

        public double GetAverage(int seconds)
        {
            if (seconds < 1 || seconds > 300) throw new ArgumentException();
            lock(_syncObject)
            {
                return _previousCounts.Count > 0
                           ? _previousCounts.Skip(Math.Max(0, _previousCounts.Count() - seconds)).Take(seconds).Average()
                           : 0.0;
            }
        }

        public void Dispose()
        {
            if (_running) Stop();
            _timer.Dispose();
        }
    }
}
