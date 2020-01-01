using System;
using UnityEngine;

public enum SkullState
{
    GroundedWaiting,
    GroundedCanHop,
    Hurt,
    InAir,
    Dead
}

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

    private TimerCollection timers;
    private ICreatureFsm<SkullState> fsm;
    private ICreaturePhysics physics;
    private IFlipX flipX;
    private ICreatureHealth health;

    public void Init(BaseCreature creature, ICreatureFsm<SkullState> fsm)
    {
        this.fsm = fsm;
        timers = creature.timers;
        physics = creature.physics;
        flipX = creature.flipXItems;
        health = creature.health;

        creature.SetOnDeathStartedCallback(() => fsm.State = SkullState.Dead);
        creature.SetOnHurtFinishedCallback(() => fsm.UnsetSprite(SkullState.Hurt));

        this.fsm.State = SkullState.InAir;

        timers.Add("hop", new Timer(hopTimerTop, OnHopTimerExpired, TimerMode.Repeat));
        timers.Add("collisionExitToNotGrounded", new Timer(collisionExitToNotGroundedTimerTop, OnCollisionExitToNotGroundedTimerExpired, TimerMode.Oneshot));
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

    public void NewPlayerPosition(Vector2 playerPosition, bool playerAlive)
    {
        if ((fsm.State == SkullState.Dead) || !playerAlive)
        {
            return;
        }

        float distanceToPlayerX = playerPosition.x - physics.Position().x;
        flipX.FlipX = distanceToPlayerX < 0;

        if (fsm.State == SkullState.InAir)
        {
            physics.ApproachVelocity(true, false, new Vector2(distanceToPlayerX, 0));
            physics.LookAt(playerPosition);
        }

        if (fsm.State == SkullState.GroundedCanHop)
        {
            physics.Jump(hopForce);
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
            timers.Start("hop");
            timers.Stop("collisionExitToNotGrounded");
        }
        else
        {
            health.Health -= 10;
            fsm.SetSprite(SkullState.Hurt);
            timers.Start("hurt");
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
            timers.Stop("hop");
            timers.Start("collisionExitToNotGrounded");
        }
    }
}

public class SkullBehaviour : BaseCreatureBehaviour<SkullState>
{
    public Skull skull;

    [Space] [Header("Sprites")]
    public Sprite GroundedSprite;
    public Sprite InAirSprite;
    public Sprite DeadSprite;
    public Sprite HurtSprite;

    private new void Start()
    {
        base.Start();

        fsm.Add(SkullState.GroundedCanHop, GroundedSprite, null);
        fsm.Add(SkullState.GroundedWaiting, GroundedSprite, null);
        fsm.Add(SkullState.InAir, InAirSprite, null);
        fsm.Add(SkullState.Dead, DeadSprite, null);
        fsm.Add(SkullState.Hurt, HurtSprite, null);

        skull.Init(creature, fsm);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        skull.NewPlayerPosition(player.HeadPosition, player != null && player.Alive());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        skull.OnCollisionEnter2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        skull.OnCollisionExit2D(collision);
    }
}
