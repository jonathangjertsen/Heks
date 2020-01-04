using System;
using UnityEngine;

[Serializable]
public class BaseCreature
{
    [Header("Health")]
    public float maxHealth;
    public float regenPer = 0.02f;

    [Space] [Header("Damage")]
    public int hurtTimerTop = 60;

    [Space] [Header("Death timing")]
    public int deathToShrinkStartTimerTop = 100;
    public int shrinkTimerTop = 200;

    public FlipXCollection FlipXItems { get; protected set; }
    public BarCollection bars;
    public CreatureHealth health;
    public TimerCollection timers;
    public ICreaturePhysics physics;

    public void Init(ICreaturePhysics physics, IBarDisplay healthBar)
    {
        this.physics = physics;

        bars = new BarCollection();
        bars.Add(healthBar);
        health = new CreatureHealth(healthBar, maxHealth);

        timers = new TimerCollection();
        timers.Add("deathToShrinkStart", deathToShrinkStartTimerTop, () => timers.Start("shrink"));
        timers.Add("shrink", shrinkTimerTop, null, onTick: Shrinking);
        timers.Add("hurt", hurtTimerTop, null);

        FlipXItems = new FlipXCollection();
        FlipXItems.Add(bars);
        FlipXItems.Add(physics);
    }

    public void SetDeathStartedCallback(CreatureHealth.OnZeroHealth callback)
    {
        health.PrependZeroHealthCallback(() => {
            callback();
            Die();
        });
    }

    public void SetDeathFinishedCallback(Callback.Void callback)
    {
        timers.SetTimeoutCallback("shrink", callback);
    }

    public void SetHurtCallback(Callback.FloatIn callback)
    {
        health.SetHurtCallback(callback);
    }

    public void SetHurtFinishedCallback(Callback.Void callback)
    {
        timers.SetTimeoutCallback("hurt", callback);
    }

    public bool FlipX
    {
        get => FlipXItems.FlipX;
        set => FlipXItems.FlipX = value;
    }

    private void Shrinking()
    {
        float proportion = timers.Value("shrink") / (float)shrinkTimerTop;
        physics.Size = physics.InitialSize * proportion;
    }

    public void Die()
    {
        timers.StopAll();
        timers.Start("deathToShrinkStart");
    }

    public void FixedUpdate()
    {
        RegenerateHealth();
        timers.TickAll();
    }

    public void Hurt(float damage, float recoilTorque = 0f)
    {
        if (health.Health > 0)
        {
            timers.Start("hurt");
            health.Hurt(damage);
            physics.Recoil(recoilTorque);
        }
    }

    private void RegenerateHealth()
    {
        health.Heal(regenPer);
    }
}
