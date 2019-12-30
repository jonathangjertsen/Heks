using System;
using UnityEngine;

public enum OctpusState
{
    Alive,
    Dead
}

[Serializable]
public class Octopus
{
    public float freqX;
    public float freqY;
    public float ampX;
    public float ampY;
    public float uprightTorque;

    private ICreaturePhysics physics;
    private ICreatureFsm<OctpusState> fsm;
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

    public void SetFsm(ICreatureFsm<OctpusState> fsm)
    {
        this.fsm = fsm;
    }

    public void SetHealth(ICreatureHealth health)
    {
        this.health = health;
    }

    protected OctpusState FsmState
    {
        get => fsm.State;
        set => fsm.State = value;
    }

    public void Die()
    {
        FsmState = OctpusState.Dead;
    }

    public void OnCollisionEnter2D()
    {
        if (FsmState != OctpusState.Dead)
        {
            health.Health -= 10;
        }
    }
}

public class OctopusBehaviour : BaseCreatureBehaviour<OctpusState>
{
    public Sprite sprite;
    public Octopus octopus;

    new private void Start()
    {
        base.Start();
        octopus.SetPhysics(physics);
        octopus.SetFsm(fsm);
        octopus.SetHealth(health);

        fsm.Add(OctpusState.Alive, sprite, null);
        fsm.Add(OctpusState.Dead, sprite, null);
        FsmState = OctpusState.Alive;
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
