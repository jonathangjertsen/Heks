using System.Collections.Generic;

namespace FrameTimer
{
    public enum TimerMode
    {
        Oneshot,
        Repeat,
    }

    public class Timer
    {
        private readonly int top;
        private readonly Timeout onTimeout;

        private bool running;
        private int timer;
        private readonly TimerMode mode;

        public delegate void Timeout();

        public Timer(int top, Timeout onTimeout, TimerMode mode = TimerMode.Oneshot)
        {
            this.top = top;
            this.onTimeout = onTimeout;
            this.mode = mode;

            timer = top;
            running = false;
        }

        public void Tick()
        {
            if (!running)
            {
                return;
            }

            timer -= 1;
            if (timer <= 0)
            {
                onTimeout();

                if (mode == TimerMode.Oneshot)
                {
                    Stop();
                }
                else if (mode == TimerMode.Repeat)
                {
                    Start();
                }
            }
        }

        public void Start()
        {
            running = true;
            timer = top;
        }

        public void Stop()
        {
            running = false;
            timer = top;
        }

        public void Pause()
        {
            running = false;
        }
    }

    public class TimerCollection
    {
        private readonly List<Timer> timers;

        public TimerCollection(List<Timer> timers)
        {
            this.timers = timers;
        }

        public void TickAll()
        {
            foreach(Timer timer in timers)
            {
                timer.Tick();
            }
        }

        public void StopAll()
        {
            foreach(Timer timer in timers)
            {
                timer.Stop();
            }
        }
    }
}
