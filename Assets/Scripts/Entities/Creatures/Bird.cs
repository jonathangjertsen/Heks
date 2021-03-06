﻿using System;
using UnityEngine;

[Serializable]
public class Bird : Creature, ICreatureController, ITakesDamage, IDealsDamage, ITakesStatusEffect
{
    [Space]
    [Header("Chasing behaviour")]
    [SerializeField] float visionRadius = 15f;
    [SerializeField] float overshoot = 0.01f;
    [SerializeField] float maxVelocity = 10.0f;
    [Range(0f, 10f)]
    [SerializeField] float uprightTorque = 4;

    [Space]
    [Header("SysCollision")]
    [Range(1f, 5f)] [SerializeField] float collisionDefense;
    [Range(0f, 100f)] [SerializeField] float collisionAttack;

    [Space]
    [Header("Reaction to burns")]
    [Range(1, 100)] [SerializeField] int framesPerBurnDamage = 1;

    public float CollisionDefense { get => collisionDefense; set => collisionDefense = value; }
    public float CollisionAttack { get => Alive() ? collisionAttack : 0; set => collisionAttack = value; }

    private Vector2 vectorToPlayer;
    private IDealsStatusEffect threat;
    private Vector2 home;
    BaseCreature creature;
    public ICreatureFsm<BirdState> fsm { get; private set; }
    private IPlayerLocator playerLocator;
    private int tick = 0;

    public void Init(BaseCreature creature, ICreatureFsm<BirdState> fsm, IPlayerLocator playerLocator)
    {
        creature.SetDeathStartedCallback(() => fsm.State = BirdState.Dead);
        creature.SetHurtFinishedCallback(OnHurtCompleted);
        creature.timers.Add("burn", 0, onTimeout: () => fsm.State = BirdState.MoveHome);

        this.creature = creature;
        this.fsm = fsm;
        this.playerLocator = playerLocator;
        this.fsm.State = BirdState.MoveHome;

        home = creature.physics.Position();
        tick = 0;

        if (framesPerBurnDamage == 0)
        {
            throw new System.Exception("Cannot have framesPerBurnDamage=0");
        }
    }

    public void FixedUpdate()
    {
        tick++;
        creature.FixedUpdate();

        if (fsm.State == BirdState.Dead)
        {
            creature.physics.Accelerate(new Vector2(0, -0.5f));
            return;
        }

        creature.physics.GetUpright(uprightTorque);

        if (PlayerIsNullOrDead())
        {
            ApproachHome();
            return;
        }

        if (Burned())
        {
            DoBurnedBehaviour();
            return;
        }

        FindPlayerLocation();
        if (CloseToPlayer())
        {
            DoBehaviourCloseToPlayer();
            return;
        }

        ApproachHome();
    }

    private bool Burned() => fsm.State == BirdState.Burned;

    public void TakeStatusEffect(IStatusEffect statusEffect, IDealsStatusEffect dealer)
    {
        if (fsm.State == BirdState.Dead)
        {
            return;
        }

        switch(statusEffect.Type)
        {
            case StatusEffectType.Burn:
                fsm.State = BirdState.Burned;
                creature.timers.SetTop("burn", (int)statusEffect.Intensity * framesPerBurnDamage);
                creature.timers.Start("burn");
                threat = dealer;
                break;
        }
    }

    private void DoBehaviourCloseToPlayer()
    {
        if (fsm.State == BirdState.Hurt)
        {
            vectorToPlayer *= -1;
        }
        else
        {
            fsm.State = BirdState.MoveToPlayer;
        }

        // TODO bug if negative
        creature.physics.ApproachVelocity(new Vector2(
            Math.Min(vectorToPlayer.x, maxVelocity) * (1 + overshoot),
            Math.Min(vectorToPlayer.y, maxVelocity) * (1 + overshoot)
        ));
    }

    private void DoBurnedBehaviour()
    {
        if (tick % framesPerBurnDamage == 0)
        {
            creature.Hurt(1f, 10f);
        }
        DoScaredBehaviour();
    }

    private void DoScaredBehaviour()
    {
        Vector2 vectorToThreatNormalized = (threat.Position() - creature.physics.Position()).normalized;
        creature.physics.ApproachVelocity(-vectorToThreatNormalized * maxVelocity * 1.2f);
        creature.FlipX = vectorToThreatNormalized.x < 0;
    }

    private bool CloseToPlayer()
    {
        return vectorToPlayer.magnitude < visionRadius;
    }

    private void ApproachHome()
    {
        creature.physics.ApproachVelocity((home - creature.physics.Position()).normalized * maxVelocity);
        fsm.State = BirdState.MoveHome;
    }

    private bool Alive()
    {
        return fsm.State != BirdState.Dead;
    }

    private void OnHurtCompleted()
    {
        if (fsm.State == BirdState.Dead)
        {
            return;
        }

        fsm.State = CloseToPlayer() ? BirdState.MoveToPlayer : BirdState.MoveHome;
    }

    public void TakeDamage(float amount)
    {
        creature.Hurt(amount, 100f);
        if (!Alive())
        {
            return;
        }
        fsm.State = BirdState.Hurt;
    }

    private bool PlayerIsNullOrDead()
    {
        return playerLocator == null || !playerLocator.IsAlive();
    }

    private void FindPlayerLocation()
    {
        Vector2 playerPosition = playerLocator.HeadPosition;
        vectorToPlayer = playerPosition - creature.physics.Position();
        creature.FlipX = vectorToPlayer.x > 0;
        creature.physics.LookAt(playerPosition);
    }

    override public void CollidedWith(ICollisionSystemParticipator other)
    {
        CollisionSystem.RegisterCollision(this, other);
    }

    public void DealDamage(float amount)
    {
    }
}
