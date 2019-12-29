using System.Collections.Generic;
using System;
using UnityEngine;

public enum TimerMode
{
    Oneshot,
    Repeat,
}

public class Timer
{
    private readonly int top;
    private readonly Timeout onTimeout;
    private readonly OnTick onTick;
    private int timer;
    private readonly TimerMode mode;
    public bool logCallbacks = false;
    public string name = "unnamed";

    public delegate void OnTick();
    public delegate void Timeout();

    public Timer(int top, Timeout onTimeout, TimerMode mode = TimerMode.Oneshot, OnTick onTick = null)
    {
        this.top = top;
        this.onTimeout = onTimeout;
        this.onTick = onTick;
        this.mode = mode;

        timer = top;
        Running = false;
    }

    public int Value { get => timer; }

    public void Tick()
    {
        if (!Running)
        {
            return;
        }

        timer -= 1;
        if (timer <= 0)
        {
            if (logCallbacks)
            {
                Debug.Log($"Invoking onTimeout callback for {name}");
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
        else
        {
            if (onTick != null)
            {
                if (logCallbacks)
                {
                    Debug.Log($"Invoking onTick callback for {name}");
                }
                onTick();
            }
        }
    }

    public void Start()
    {
        Running = true;
        timer = top;
    }

    public void Stop()
    {
        Running = false;
        timer = top;
    }

    public void Pause()
    {
        Running = false;
    }

    public bool Running { get; set; }
}

public class TimerCollection
{
    private readonly Dictionary<string, Timer> timers;
    public bool logCallbacks = false;

    public TimerCollection()
    {
        timers = new Dictionary<string, Timer>();
    }

    public void Add(string name, Timer timer)
    {
        timer.logCallbacks = logCallbacks;
        timer.name = name;
        timers.Add(name, timer);
    }

    public int Value(string name)
    {
        return timers[name].Value;
    }

    public void TickAll()
    {
        foreach(KeyValuePair<string, Timer> pair in timers)
        {
            pair.Value.Tick();
        }
    }

    public void Start(string name)
    {
        if (timers.TryGetValue(name, out Timer timer))
        {
            timer.Start();
        }
        else
        {
            throw new System.Exception($"No timer named {name}");
        }

    }

    public void Stop(string name)
    {
        if (timers.TryGetValue(name, out Timer timer))
        {
            timer.Stop();
        }
        else
        {
            throw new System.Exception($"No timer named {name}");
        }
    }

    public void StopAll()
    {
        foreach(KeyValuePair<string, Timer> pair in timers)
        {
            pair.Value.Stop();
        }
    }
}