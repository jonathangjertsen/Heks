using System;
using UnityEngine;

[Serializable]
public class Skull : Creature, ICreatureController, ICollisionSystemParticipator, ITakesDamage, IDealsDamage
{
    [Space] [Header("Jumping and chasing behaviour")]
    [SerializeField] float visionRadius = 15f;
    [SerializeField] int hopTimerTop = 50;
    [SerializeField] int hopForce = 10;
    [SerializeField] int hurtTimerTop = 10;
    [Range(0f, 10f)]
    [SerializeField] float uprightTorque = 4;

    [Space] [Header("Glitch filtering")]
    [SerializeField] int collisionExitToNotGroundedTimerTop = 10;

    [Space]
    [Header("SysCollision")]
    [Range(1f, 5f)] [SerializeField] float collisionDefense;
    [Range(0f, 100f)] [SerializeField] float collisionAttack;

    public float CollisionDefense { get => collisionDefense; set => collisionDefense = value; }
    public float CollisionAttack { get => Alive() ? collisionAttack : 0; set => collisionAttack = value; }

    private BaseCreature creature;
    private IPlayerLocator playerLocator;
    private ICreatureFsm<SkullState> fsm;

    public void Init(BaseCreature creature, ICreatureFsm<SkullState> fsm, IPlayerLocator playerLocator)
    {
        this.fsm = fsm;
        this.creature = creature;

        creature.SetDeathStartedCallback(() => fsm.State = SkullState.Dead);
        creature.SetHurtFinishedCallback(() => fsm.UnsetSprite(SkullState.Hurt));

        this.fsm.State = SkullState.InAir;

        creature.timers.Add("hop", hopTimerTop, OnHopTimerExpired, TimerMode.Repeat);
        creature.timers.Add("collisionExitToNotGrounded", collisionExitToNotGroundedTimerTop, OnCollisionExitToNotGroundedTimerExpired, TimerMode.Oneshot);
        this.playerLocator = playerLocator;
    }

    private void OnHopTimerExpired()
    {
        if (fsm.State == SkullState.GroundedWaiting)
        {
            fsm.State = SkullState.GroundedCanHop;
        }
    }

    private void OnCollisionExitToNotGroundedTimerExpired()
    {
        if (fsm.State == SkullState.GroundedWaiting)
        {
            fsm.State = SkullState.InAir;
        }
    }

    public void FixedUpdate()
    {
        creature.FixedUpdate();

        Vector2 playerPosition = playerLocator.HeadPosition;
        bool playerAlive = playerLocator != null && playerLocator.IsAlive();

        if (!Alive() || !playerAlive)
        {
            return;
        }

        float distanceToPlayerX = playerPosition.x - creature.physics.Position().x;
        creature.FlipX = distanceToPlayerX < 0;

        if (fsm.State == SkullState.InAir)
        {
            creature.physics.ApproachVelocity(true, false, new Vector2(distanceToPlayerX, 0));
            creature.physics.LookAt(playerPosition);
        }

        if (fsm.State == SkullState.GroundedCanHop)
        {
            creature.physics.Jump(hopForce);
            fsm.State = SkullState.GroundedWaiting;
        }

        creature.physics.GetUpright(uprightTorque);
    }

    override public void CollidedWith(ICollisionSystemParticipator other)
    {
        if (!Alive())
        {
            return;
        }

        if (other.As(out IGround ground))
        {
            fsm.State = SkullState.GroundedWaiting;
            creature.timers.Start("hop");
            creature.timers.Stop("collisionExitToNotGrounded");
        }

        CollisionSystem.RegisterCollision(this, other);
    }

    override public void ExitedCollisionWith(ICollisionSystemParticipator other)
    {
        if (fsm.State == SkullState.Dead)
        {
            return;
        }

        if (other.As(out IGround ground))
        {
            fsm.State = SkullState.GroundedWaiting;
            creature.timers.Stop("hop");
            creature.timers.Start("collisionExitToNotGrounded");
        }
    }

    public void TakeDamage(float amount)
    {
        creature.health.Hurt(10);
        fsm.SetSprite(SkullState.Hurt);
        creature.timers.Start("hurt");
    }

    public void DealDamage(float amount)
    {
    }

    private bool Alive()
    {
        return fsm.State != SkullState.Dead;
    }
}
