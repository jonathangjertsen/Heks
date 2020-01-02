﻿using System;
using UnityEngine;

[Serializable]
public class Octopus
{
    [Range(-0.1f, 0.1f)]
    [SerializeField] float freqX = 0.3f;

    [Range(-0.1f, 0.1f)]
    [SerializeField] float freqY = 0.2f;

    [Range(0.0f, 0.04f)]
    [SerializeField] float ampX = 0.02f;

    [Range(0.0f, 0.04f)]
    [SerializeField] float ampY = 0.03f;

    [Range(0f, 10f)]
    [SerializeField] float uprightTorque = 4;

    private ICreaturePhysics physics;
    private ICreatureFsm<OctopusState> fsm;
    private ICreatureHealth health;
    private int counter;

    public Octopus()
    {
        counter = 0;
    }

    public void NextFrame()
    {
        counter++;
        Vector2 target = new Vector2(
            ampX * Mathf.Cos(freqX * counter),
            ampY * Mathf.Sin(freqY * counter)
        );
        physics.GetUpright(uprightTorque);
        physics.AccelerateRelative(target);
    }

    public void Init(BaseCreature creature, ICreatureFsm<OctopusState> fsm)
    {
        this.fsm = fsm;
        health = creature.health;
        physics = creature.physics;
        creature.SetOnDeathStartedCallback(() => fsm.State = OctopusState.Dead);
        fsm.State = OctopusState.Alive;
    }

    public void Die()
    {
        fsm.State = OctopusState.Dead;
    }

    public void OnCollisionEnter2D()
    {
        if (fsm.State != OctopusState.Dead)
        {
            health.Hurt(10);
        }
    }
}