using UnityEngine;

public enum SkullState
{
    GroundedWaiting,
    GroundedCanHop,
    Hurt,
    InAir,
    Dead
}

public class SkullBehaviour : BaseCreatureBehaviour<SkullState>, IFlipX
{
    public Sprite GroundedSprite;
    public Sprite InAirSprite;
    public Sprite DeadSprite;
    public Sprite HurtSprite;

    public float visionRadius = 15f;
    public int hopTimerTop = 50;
    public int hopForce = 10;
    public int hurtTimerTop = 10;
    public int collisionExitToNotGroundedTimerTop = 10;

    public override void Die()
    {
        base.Die();
        FsmState = SkullState.Dead;
    }

    private new void Start()
    {
        base.Start();

        timers.Add("hop", new Timer(hopTimerTop, OnHopTimerExpired, TimerMode.Repeat));
        timers.Add("collisionExitToNotGrounded", new Timer(collisionExitToNotGroundedTimerTop, OnCollisionExitToNotGroundedTimerExpired, TimerMode.Oneshot));
        timers.Add("hurt", new Timer(hurtTimerTop, OnHurtTimerExpired));

        fsm.Add(SkullState.GroundedCanHop, GroundedSprite, null);
        fsm.Add(SkullState.GroundedWaiting, GroundedSprite, null);
        fsm.Add(SkullState.InAir, InAirSprite, null);
        fsm.Add(SkullState.Dead, DeadSprite, null);
        fsm.Add(SkullState.Hurt, HurtSprite, null);
        FsmState = SkullState.InAir;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if ((FsmState == SkullState.Dead) || (player == null))
        {
            return;
        }

        float distanceToPlayerX = player.HeadPosition.x - transform.position.x;
        FlipX = distanceToPlayerX < 0;

        if (FsmState == SkullState.InAir)
        {
            physics.ApproachVelocity(true, false, new Vector2(distanceToPlayerX, 0));
            physics.LookAt(player.transform);
        }

        if (FsmState == SkullState.GroundedCanHop)
        {
            physics.Jump(hopForce);
            FsmState = SkullState.GroundedWaiting;
        }
    }

    private void OnHurtTimerExpired()
    {
        fsm.UnsetSprite(SkullState.Hurt);
    }

    private void OnHopTimerExpired()
    {
        if (FsmState == SkullState.GroundedWaiting)
        {
            FsmState = SkullState.GroundedCanHop;
        }
    }

    private void OnCollisionExitToNotGroundedTimerExpired()
    {
        if (FsmState == SkullState.GroundedWaiting)
        {
            FsmState = SkullState.InAir;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (FsmState == SkullState.Dead)
        {
            return;
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            FsmState = SkullState.GroundedWaiting;
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (FsmState == SkullState.Dead)
        {
            return;
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            FsmState = SkullState.GroundedWaiting;
            timers.Stop("hop");
            timers.Start("collisionExitToNotGrounded");
        }
    }
}
