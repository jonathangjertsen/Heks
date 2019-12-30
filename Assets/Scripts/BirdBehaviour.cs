﻿using System;
using UnityEngine;

public enum BirdState
{
    MoveHome,
    MoveToPlayer,
    Hurt,
    Dead,
}

public class BirdBehaviour : BaseCreatureBehaviour<BirdState>
{
    public float regenPer = 0.02f;
    public float visionRadius = 15f;
    public float overshoot = 0.01f;
    private Timer hurtTimer;
    public int hurtTimerTop = 60;

    public AudioClip CryClip;

    public Sprite DefaultSprite;
    public Sprite ChargingSprite;
    public Sprite HurtSprite;
    public Sprite DeadSprite;
    public Transform playerTransform;

    private Vector2 vectorToPlayer;
    private Vector2 home;

    public override void Die()
    {
        base.Die();
        FsmState = BirdState.Dead;
    }

    private new void Start()
    {
        base.Start();

        home = creature.physics.Position();

        fsm.Add(BirdState.MoveHome, DefaultSprite, null);
        fsm.Add(BirdState.MoveToPlayer, ChargingSprite, CryClip);
        fsm.Add(BirdState.Hurt, HurtSprite, CryClip);
        fsm.Add(BirdState.Dead, DeadSprite, null);
        FsmState = BirdState.MoveHome;

        hurtTimer = new Timer(hurtTimerTop, OnHurtTimerExpired);
    }

    private void ApproachHome()
    {
        creature.physics.ApproachVelocity(home - creature.physics.Position());
        FsmState = BirdState.MoveHome;
    }

    private void DoBehaviourCloseToPlayer()
    {
        if (FsmState == BirdState.Hurt)
        {
            vectorToPlayer *= -1;
        }
        else
        {
            FsmState = BirdState.MoveToPlayer;
        }

        creature.physics.ApproachVelocity(new Vector2(
            Math.Min(vectorToPlayer.x, creature.maxVelocityX) * (1 + overshoot),
            Math.Min(vectorToPlayer.y, creature.maxVelocityY) * (1 + overshoot)
        ));
    }

    private bool CloseToPlayer()
    {
        return vectorToPlayer.magnitude < visionRadius;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (FsmState == BirdState.Dead)
        {
            creature.physics.Accelerate(new Vector2(0, -0.5f));
            return;
        }

        if (player == null)
        {
            ApproachHome();
            return;
        }

        vectorToPlayer = player.HeadPosition - creature.physics.Position();

        creature.FlipX = vectorToPlayer.x > 0;
        creature.physics.LookAt(player.transform);

        if (player.Alive())
        {
            if (CloseToPlayer())
            {
                DoBehaviourCloseToPlayer();
            }
            else
            {
                ApproachHome();
            }
        }
        else
        {
            ApproachHome();
        }

        RegenerateHealth();
    }

    private void RegenerateHealth()
    {
        creature.health.Health += regenPer;
    }

    private void OnCollisionEnter2D()
    {
        if (FsmState == BirdState.Dead)
        {
            return;
        }

        FsmState = BirdState.Hurt;
        hurtTimer.Start();

        creature.health.Health -= 10;
        creature.physics.Recoil(100);
    }

    private void OnHurtTimerExpired()
    {
        if (FsmState == BirdState.Dead)
        {
            return;
        }

        FsmState = CloseToPlayer() ? BirdState.MoveToPlayer : BirdState.MoveHome;
    }
}
