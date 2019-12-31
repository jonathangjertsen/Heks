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
    [Range(-0.1f, 0.1f)]
    public float freqX = 0.3f;

    [Range(-0.1f, 0.1f)]
    public float freqY = 0.2f;

    [Range(0.0f, 0.04f)]
    public float ampX = 0.02f;

    [Range(0.0f, 0.04f)]
    public float ampY = 0.03f;

    [Range(0f, 10f)]
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

    public void Init(BaseCreature creature, ICreatureFsm<OctopusState> fsm)
    {
        this.fsm = fsm;
        health = creature.health;
        physics = creature.physics;

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

        fsm.Add(OctopusState.Alive, sprite, null);
        fsm.Add(OctopusState.Dead, sprite, null);

        octopus.Init(creature, fsm);
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
