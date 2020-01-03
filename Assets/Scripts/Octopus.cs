using System;
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

    private BaseCreature creature;
    private ICreatureFsm<OctopusState> fsm;
    private int counter;

    public Octopus()
    {
        counter = 0;
    }

    public void FixedUpdate()
    {
        creature.FixedUpdate();

        counter++;
        Vector2 target = new Vector2(
            ampX * Mathf.Cos(freqX * counter),
            ampY * Mathf.Sin(freqY * counter)
        );
        creature.physics.GetUpright(uprightTorque);
        creature.physics.AccelerateRelative(target);
    }

    public void Init(BaseCreature creature, ICreatureFsm<OctopusState> fsm)
    {
        this.fsm = fsm;
        this.creature = creature;
        creature.SetDeathStartedCallback(() => fsm.State = OctopusState.Dead);
        fsm.State = OctopusState.Alive;
    }

    public void OnCollisionEnter2D()
    {
        if (fsm.State != OctopusState.Dead)
        {
            creature.health.Hurt(10);
        }
    }
}
