using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum TimerMode
{
    Oneshot,
    Repeat,
}

public class Timer
{
    private readonly int top;
    private Timeout onTimeout;
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

    public int Value => timer;

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

    public void SetTimeoutCallback(Timeout callback)
    {
        onTimeout = callback;
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

public class TimerNotFoundException : Exception
{
    public TimerNotFoundException(string name, Dictionary<string, Timer> timers)
        : base($"No timer named '{name}'. Available timers: {string.Join(", ", timers.Select(kvp => "'" + kvp.Key + "'"))}")
    {
    }
}

public class TimerCollection
{
    private Dictionary<string, Timer> startedTimers;
    private Dictionary<string, Timer> stoppedTimers;

    private readonly Dictionary<string, Timer> timers;
    public bool logCallbacks = false;

    public TimerCollection()
    {
        timers = new Dictionary<string, Timer>();
        startedTimers = new Dictionary<string, Timer>();
        stoppedTimers = new Dictionary<string, Timer>();
    }

    public void Add(string name, Timer timer)
    {
        timer.logCallbacks = logCallbacks;
        timer.name = name;
        timers.Add(name, timer);
    }

    public int Value(string name)
    {
        return Get(name).Value;
    }

    public bool Running(string name)
    {
        return Get(name).Running;
    }

    public void PropagateStartAndStop()
    {
        foreach (KeyValuePair<string, Timer> pair in startedTimers)
        {
            pair.Value.Start();
        }
        startedTimers = new Dictionary<string, Timer>();

        foreach (KeyValuePair<string, Timer> pair in stoppedTimers)
        {
            pair.Value.Stop();
        }
        stoppedTimers = new Dictionary<string, Timer>();
    }

    public void TickAll()
    {
        foreach (KeyValuePair<string, Timer> pair in timers)
        {
            pair.Value.Tick();
        }

        PropagateStartAndStop();
    }

    public void Start(string name)
    {
        if (!startedTimers.ContainsKey(name))
        {
            startedTimers.Add(name, Get(name));
        }
    }

    public void Stop(string name)
    {
        if (!stoppedTimers.ContainsKey(name))
        {
            stoppedTimers.Add(name, Get(name));
        }
    }

    public void StopAll()
    {
        foreach (KeyValuePair<string, Timer> pair in timers)
        {
            pair.Value.Stop();
        }
    }

    private Timer Get(string name)
    {
        if (timers.TryGetValue(name, out Timer timer))
        {
            return timer;
        }
        else
        {
            throw new TimerNotFoundException(name, timers);
        }
    }
}