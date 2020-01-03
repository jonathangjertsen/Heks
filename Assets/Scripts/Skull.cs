using System;
using UnityEngine;

[Serializable]
public class Skull
{
    [Space] [Header("Jumping and chasing behaviour")]
    [SerializeField] float visionRadius = 15f;
    [SerializeField] int hopTimerTop = 50;
    [SerializeField] int hopForce = 10;
    [SerializeField] int hurtTimerTop = 10;

    [Space] [Header("Glitch filtering")]
    [SerializeField] int collisionExitToNotGroundedTimerTop = 10;

    private BaseCreature creature;
    private IPlayerLocator playerLocator;
    private ICreatureFsm<SkullState> fsm;
    private IFlipX flipX;

    public void Init(BaseCreature creature, ICreatureFsm<SkullState> fsm, IPlayerLocator playerLocator)
    {
        this.fsm = fsm;
        this.creature = creature;
        flipX = creature.FlipXItems;

        creature.SetDeathStartedCallback(() => fsm.State = SkullState.Dead);
        creature.SetHurtFinishedCallback(() => fsm.UnsetSprite(SkullState.Hurt));

        this.fsm.State = SkullState.InAir;

        creature.timers.Add("hop", new Timer(hopTimerTop, OnHopTimerExpired, TimerMode.Repeat));
        creature.timers.Add("collisionExitToNotGrounded", new Timer(collisionExitToNotGroundedTimerTop, OnCollisionExitToNotGroundedTimerExpired, TimerMode.Oneshot));
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

        if ((fsm.State == SkullState.Dead) || !playerAlive)
        {
            return;
        }

        float distanceToPlayerX = playerPosition.x - creature.physics.Position().x;
        flipX.FlipX = distanceToPlayerX < 0;

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
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (fsm.State == SkullState.Dead)
        {
            return;
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            fsm.State = SkullState.GroundedWaiting;
            creature.timers.Start("hop");
            creature.timers.Stop("collisionExitToNotGrounded");
        }
        else
        {
            creature.health.Hurt(10);
            fsm.SetSprite(SkullState.Hurt);
            creature.timers.Start("hurt");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (fsm.State == SkullState.Dead)
        {
            return;
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            fsm.State = SkullState.GroundedWaiting;
            creature.timers.Stop("hop");
            creature.timers.Start("collisionExitToNotGrounded");
        }
    }
}
