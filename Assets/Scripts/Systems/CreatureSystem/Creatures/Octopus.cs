using System;
using UnityEngine;

[Serializable]
public class Octopus : Creature, ICreatureController, ICollisionSystemParticipator, ITakesDamage, IDealsDamage
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

    [Space]
    [Header("SysCollision")]
    [Range(1f, 5f)] [SerializeField] float collisionDefense;
    [Range(0f, 100f)] [SerializeField] float collisionAttack;

    public float CollisionDefense { get => collisionDefense; set => collisionDefense = value; }
    public float CollisionAttack { get => collisionAttack; set => collisionAttack = value; }

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

    public void TakeDamage(float amount)
    {
        if (fsm.State != OctopusState.Dead)
        {
            creature.health.Hurt(amount);
        }
    }

    public void DealDamage(float amount)
    {
    }

    public void TriggeredWith(ICollisionSystemParticipator other)
    {
    }

    public void ExitedTriggerWith(ICollisionSystemParticipator other)
    {
    }

    public ICollisionSystemParticipator GetCollisionSystemParticipator() => this;

    public void CollidedWith(ICollisionSystemParticipator other)
    {
        CollisionSystem.RegisterCollision(this, other);
    }

    public void ExitedCollisionWith(ICollisionSystemParticipator other)
    {
    }
}
