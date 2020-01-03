using System;
using System.Collections.Generic;
using System.Linq;

public class TimerNotFoundException : Exception
{
    public TimerNotFoundException(string name, Dictionary<string, Timer> timers)
        : base($"No timer named '{name}'. Available timers: {string.Join(", ", timers.Select(kvp => "'" + kvp.Key + "'"))}")
    {
    }
}
