using System;
using UnityEngine;

public enum OctopusState
{
    Alive,
    Dead
}

[Serializable]
public class Octopus
{
    public float freqX = 0.3f;
    public float freqY = 0.2f;
    public float ampX = 0.02f;
    public float ampY = 0.03f;
    public float uprightTorque = 4;

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

    public void SetPhysics(ICreaturePhysics physics)
    {
        this.physics = physics;
    }

    public void SetFsm(ICreatureFsm<OctopusState> fsm)
    {
        this.fsm = fsm;
    }

    public void SetHealth(ICreatureHealth health)
    {
        this.health = health;
    }

    protected OctopusState FsmState
    {
        get => fsm.State;
        set => fsm.State = value;
    }

    public void Die()
    {
        FsmState = OctopusState.Dead;
    }

    public void OnCollisionEnter2D()
    {
        if (FsmState != OctopusState.Dead)
        {
            health.Health -= 10;
        }
    }
}

public class OctopusBehaviour : BaseCreatureBehaviour<OctopusState>
{
    public Sprite sprite;
    public Octopus octopus;

    new private void Start()
    {
        base.Start();
        octopus.SetPhysics(creature.physics);
        octopus.SetFsm(fsm);
        octopus.SetHealth(creature.health);

        fsm.Add(OctopusState.Alive, sprite, null);
        fsm.Add(OctopusState.Dead, sprite, null);
        FsmState = OctopusState.Alive;
    }

    public override void Die()
    {
        base.Die();
        octopus.Die();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        octopus.NextFrame();
    }

    private void OnCollisionEnter2D()
    {
        octopus.OnCollisionEnter2D();
    }
}
