using UnityEngine;

public class SkullScript : MonoBehaviour, IFlipX
{
    public PlayerScript player;
    private CreaturePhysics.CreaturePhysics physics;
    public float visionRadius = 15f;
    private bool grounded = false;
    public int hopTimerTop = 50;
    public int hopForce = 10;
    public int collisionExitToNotGroundedTimerTop = 10;
    FrameTimer.Timer collisionExitToNotGroundedTimer;
    FrameTimer.Timer hopTimer;

    bool canHop = true;
    public bool FlipX { get => physics.FlipX; set => physics.FlipX = value; }

    void Start()
    {
        hopTimer = new FrameTimer.Timer(hopTimerTop, OnHopTimerExpired, FrameTimer.TimerMode.Repeat);
        collisionExitToNotGroundedTimer = new FrameTimer.Timer(collisionExitToNotGroundedTimerTop, OnCollisionExitToNotGroundedTimerExpired, FrameTimer.TimerMode.Oneshot);
        physics = new CreaturePhysics.CreaturePhysics(this);
    }

    void OnHopTimerExpired()
    {
        canHop = true;
    }

    void OnCollisionExitToNotGroundedTimerExpired()
    {
        grounded = false;
    }

    void FixedUpdate()
    {
        float distanceToPlayerX = player.transform.position.x + player.HeadOffsetX - transform.position.x;
        FlipX = distanceToPlayerX < 0;

        if (!grounded)
        {
            physics.ApproachVelocity(true, false, distanceToPlayerX, 0);
            physics.LookAt(player.transform);
        }

        if (canHop && grounded)
        {
            physics.Jump(hopForce);
            canHop = false;
        }

        hopTimer.Tick();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = true;
            canHop = false;
            hopTimer.Start();
            collisionExitToNotGroundedTimer.Stop();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = false;
            hopTimer.Stop();
            collisionExitToNotGroundedTimer.Start();
        }
    }
}
