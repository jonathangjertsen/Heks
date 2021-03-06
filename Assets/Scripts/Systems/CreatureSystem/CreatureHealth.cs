﻿using UnityEngine;

public class CreatureHealth : ICreatureHealth
{
    private readonly IBarDisplay healthBar;
    private readonly float maxHealth;
    private float currentHealth;
    private Callback.Void onZeroHealth;
    private Callback.FloatIn onHurt;
    private Callback.FloatIn onHeal;

    public delegate void OnZeroHealth();

    public CreatureHealth(IBarDisplay healthBar, float maxHealth)
    {
        this.healthBar = healthBar;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        Health = maxHealth;
    }

    public void PrependZeroHealthCallback(Callback.Void callback)
    {
        Callback.Void original = onZeroHealth;
        onZeroHealth = () => {
            callback();
            original?.Invoke();
        };
    }

    public void SetHurtCallback(Callback.FloatIn callback)
    {
        if (onHurt != null)
        {
            throw new System.Exception("Can't set hurt callback twice");
        }

        onHurt = callback;

    }

    public void Hurt(float amount)
    {
        Health -= amount;

        onHurt?.Invoke(amount);
    }

    public void Heal(float amount)
    {
        Health += amount;
        onHeal?.Invoke(amount);
    }

    public float Health
    {
        get => currentHealth;
        private set
        {
            float cappedHealth = value;
            if (value >= maxHealth)
            {
                cappedHealth = maxHealth;
            }
            else if (value <= 0.0001f)
            {
                cappedHealth = 0;
                healthBar?.Hide();
                onZeroHealth?.Invoke();
            }
            healthBar?.FillTo(cappedHealth / maxHealth);
            currentHealth = cappedHealth;
        }
    }
}
