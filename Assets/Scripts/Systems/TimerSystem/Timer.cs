
using System;

public class Timer
{
    private int top;
    private Callback.Void onTimeout;
    private readonly Callback.Void onTick;
    private readonly TimerMode mode;
    public bool logCallbacks = false;
    public string name = "unnamed";

    public int Value { get; private set; }
    public bool Running { get; private set; }

    public Timer(int top, Callback.Void onTimeout, TimerMode mode = TimerMode.Oneshot, Callback.Void onTick = null)
    {
        this.top = top;
        this.onTimeout = onTimeout;
        this.onTick = onTick;
        this.mode = mode;

        Value = top;
        Running = false;
    }

    public void Tick()
    {
        if (!Running)
        {
            return;
        }

        DecrementTimer();
    }

    public void SetTimeoutCallback(Callback.Void callback)
    {
        onTimeout = callback;
    }

    public void Start()
    {
        if (logCallbacks && !Running)
        {
            Debug.Log($"Starting timer '{name}'");
        }

        Running = true;
        Value = top;
    }

    public void Stop()
    {
        if (logCallbacks && Running)
        {
            Debug.Log($"Stopping timer '{name}'");
        }

        Running = false;
        Value = top;
    }

    public void Pause()
    {
        Running = false;
    }

    public void SetTop(int top)
    {
        this.top = top;
    }

    public void TickThrough()
    {
        while (Running && Value > 0)
        {
            Tick();
        }
    }

    private void DecrementTimer()
    {
        Value -= 1;
        if (Value <= 0)
        {
            TimerReachedZero();
        }
        else
        {
            if (onTick != null)
            {
                TimerTickedNotZero();
            }
        }
    }

    private void TimerReachedZero()
    {
        if (logCallbacks)
        {
            Debug.Log($"Invoking onTimeout callback for '{name}'");
        }
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

    private void TimerTickedNotZero()
    {
        if (logCallbacks)
        {
            Debug.Log($"Invoking onTick callback for {name}");
        }
        onTick();
    }
}
