using System.Collections.Generic;

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

    public void SetTimeoutCallback(string name, Timer.Timeout callback)
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
