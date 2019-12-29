using UnityEngine;
using System;

public enum BirdState
{
    MoveHome,
    MoveToPlayer,
    Hurt,
    Dead,
};

public class BirdScript : BaseCreature<BirdState>, IFlipX
{
    public float regenPer = 0.02f;
    public float visionRadius = 15f;
    public float overshoot = 0.01f;

    Timer hurtTimer;
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

    new void Start()
    {
        base.Start();

        home = physics.Position();

        fsm.Add(BirdState.MoveHome, DefaultSprite, null);
        fsm.Add(BirdState.MoveToPlayer, ChargingSprite, CryClip);
        fsm.Add(BirdState.Hurt, HurtSprite, CryClip);
        fsm.Add(BirdState.Dead, DeadSprite, null);
        FsmState = BirdState.MoveHome;

        hurtTimer = new Timer(hurtTimerTop, OnHurtTimerExpired);
    }

    void ApproachHome()
    {
        physics.ApproachVelocity(home - physics.Position());
        FsmState = BirdState.MoveHome;
    }

    void DoBehaviourCloseToPlayer()
    {
        if (FsmState == BirdState.Hurt)
        {
            vectorToPlayer *= -1;
        }
        else
        {
            FsmState = BirdState.MoveToPlayer;
        }

        physics.ApproachVelocity(new Vector2(
            Math.Min(vectorToPlayer.x, maxVelocityX) * (1 + overshoot),
            Math.Min(vectorToPlayer.y, maxVelocityY) * (1 + overshoot)
        ));
    }

    bool CloseToPlayer()
    {
        return vectorToPlayer.magnitude < visionRadius;
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();

        if (FsmState == BirdState.Dead)
        {
            physics.Accelerate(new Vector2(0, -0.5f));
            return;
        }

        if (player == null)
        {
            ApproachHome();
            return;
        }

        vectorToPlayer = player.HeadPosition - physics.Position();

        FlipX = vectorToPlayer.x > 0;
        physics.LookAt(player.transform);

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
        health.Health += regenPer;
    }

    void OnCollisionEnter2D()
    {
        if (FsmState == BirdState.Dead)
        {
            return;
        }

        FsmState = BirdState.Hurt;
        hurtTimer.Start();

        health.Health -= 10;
        physics.Recoil(100);
    }

    void OnHurtTimerExpired()
    {
        if (FsmState == BirdState.Dead)
        {
            return;
        }

        FsmState = CloseToPlayer() ? BirdState.MoveToPlayer : BirdState.MoveHome;
    }
}
