﻿using System;
using System.Collections.Generic;

public class TimerCollection
{
    private Dictionary<string, Timer> startedTimers;
    private Dictionary<string, Timer> pausedTimers;
    private Dictionary<string, Timer> stoppedTimers;

    private readonly Dictionary<string, Timer> timers;
    public bool logCallbacks = false;

    public TimerCollection()
    {
        timers = new Dictionary<string, Timer>();
        startedTimers = new Dictionary<string, Timer>();
        stoppedTimers = new Dictionary<string, Timer>();
        pausedTimers = new Dictionary<string, Timer>();
    }

    public void Add(string name, int top, Callback.Void onTimeout, TimerMode mode = TimerMode.Oneshot, Callback.Void onTick = null)
    {
        var timer = new Timer(top, onTimeout, mode, onTick);
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

    public void TickThrough(string name)
    {
        Get(name).TickThrough();
        PropagateStartAndStop();
    }

    public void SetTimeoutCallback(string name, Callback.Void callback)
    {
        Get(name).SetTimeoutCallback(callback);
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

        foreach (KeyValuePair<string, Timer> pair in pausedTimers)
        {
            pair.Value.Pause();
        }
        pausedTimers = new Dictionary<string, Timer>();
    }

    public void TickAll()
    {
        foreach (KeyValuePair<string, Timer> pair in timers)
        {
            pair.Value.Tick();
        }

        PropagateStartAndStop();
    }

    public void Pause(string name)
    {
        if (!pausedTimers.ContainsKey(name))
        {
            pausedTimers.Add(name, Get(name));
        }
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
