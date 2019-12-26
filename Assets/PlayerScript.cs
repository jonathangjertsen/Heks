using UnityEngine;
using System.Collections.Generic;
using FrameTimer;

enum PlayerState
{
    Flying,
    Still,
    Standing,
    Charging,
    Casting,
    Hurt,
    Angry,
    Dead,
};

public class PlayerScript : MonoBehaviour, IFlipX
{
    // Health
    public Bar healthBar;
    private CreatureHealth health;
    public float maxHealth = 100;
    public float regenPer = 0.02f;
    public float headOffsetX = 4.87f;
    public float headOffsetY = 6.06f;

    // Things related to charging
    public Bar chargeBar;
    public float chargeTop = 50;
    private float charge;
    private int chargeDirection = 1;

    // Things related to casting
    public int castTorque = 150;

    // Physics
    CreaturePhysics.CreaturePhysics physics;
    public float axCoeffX = 0.01f;
    public float axCoeffY = 0.03f;
    public float rotCoeff = 1f;
    public float maxVelocityX = 10.0f;
    public float maxVelocityY = 10.0f;

    // Timers
    public int hurtTimerTop = 30;
    public int angryTimerTop = 60;
    public int flyingToIdleTimerTop = 60;
    public int castTimerTop = 30;
    Timer hurtTimer;
    Timer angryTimer;
    Timer flyingToIdleTimer;
    Timer castTimer;
    TimerCollection timers;

    // FSM state
    CreatureFsm.CreatureFsm fsm;
    public AudioClip YellClip;
    public AudioClip HurtClip;
    public AudioClip ChargeClip;
    public AudioClip CastClip;
    public Sprite FlyingSprite;
    public Sprite StandingSprite;
    public Sprite ChargingSprite;
    public Sprite CastingSprite;
    public Sprite HurtSprite;
    public Sprite AngrySprite;
    public Sprite DeadSprite;

    // Spell management
    public GameObject spell;
    public Transform spawnPoint;

    // References to other components
    Rigidbody2D rigidBody2d;

    // Manage flipx
    BarCollection bars;
    private bool flipX;
    FlipXCollection flipXItems;

    // Simple properties
    public bool Alive => FsmState != PlayerState.Dead;
    public float Charge
    {
        get => charge;
        set {
            chargeBar.FillTo(value / chargeTop);
            charge = value;
        }
    }
    public bool FlipX
    {
        get => flipX;
        set {
            flipX = value;
            flipXItems.FlipX = value;
        }
    }
    private PlayerState FsmState { get => (PlayerState)fsm.State; set => fsm.State = (int)value; }
    public float HeadOffsetX => headOffsetX * transform.localScale.x;
    public float HeadOffsetY => headOffsetY * transform.localScale.y;

    // Timer callbacks

    private void OnAngryTimerExpired()
    {
        FsmState = PlayerState.Flying;
        angryTimer.Stop();
    }

    private void OnCastTimerExpired()
    {
        FsmState = PlayerState.Flying;
    }

    private void OnFlyingToIdleTimerExpired()
    {
        FsmState = PlayerState.Standing;
    }

    private void OnHurtTimerExpired()
    {
        FsmState = PlayerState.Angry;
        hurtTimer.Stop();
        angryTimer.Start();
    }

    // OnZeroHealth callback

    private void Die()
    {
        FsmState = PlayerState.Dead;
        timers.StopAll();
        bars.Hide();
    }

    // Unity

    void Start()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        physics = new CreaturePhysics.CreaturePhysics(
            this,
            axCoeffX: axCoeffX,
            axCoeffY: axCoeffY,
            rotCoeff: rotCoeff,
            maxVelocityY: maxVelocityY,
            maxVelocityX: maxVelocityX
        );

        health = new CreatureHealth(healthBar, maxHealth, onZeroHealth: Die);
        Charge = 0;

        bars = new BarCollection(new List<Bar>() { healthBar, chargeBar });

        var fsmSprites = new Dictionary<int, Sprite>
        {
            { (int)PlayerState.Angry, AngrySprite },
            { (int)PlayerState.Casting, CastingSprite },
            { (int)PlayerState.Charging, ChargingSprite },
            { (int)PlayerState.Dead, DeadSprite },
            { (int)PlayerState.Flying, FlyingSprite },
            { (int)PlayerState.Hurt, HurtSprite },
            { (int)PlayerState.Standing, StandingSprite },
            { (int)PlayerState.Still, FlyingSprite }
        };

        var fsmClips = new Dictionary<int, AudioClip>
        {
            { (int)PlayerState.Hurt, HurtClip },
            { (int)PlayerState.Charging, ChargeClip },
            { (int)PlayerState.Casting, CastClip },
            { (int)PlayerState.Angry, YellClip }
        };

        fsm = new CreatureFsm.CreatureFsm(gameObject, fsmSprites, fsmClips)
        {
            State = (int)PlayerState.Flying
        };

        // Init timers
        hurtTimer = new Timer(hurtTimerTop, OnHurtTimerExpired);
        angryTimer = new Timer(angryTimerTop, OnAngryTimerExpired);
        flyingToIdleTimer = new Timer(flyingToIdleTimerTop, OnFlyingToIdleTimerExpired);
        castTimer = new Timer(castTimerTop, OnCastTimerExpired);
        timers = new TimerCollection(new List<Timer>() { hurtTimer, angryTimer, flyingToIdleTimer, castTimer });

        flipXItems = new FlipXCollection(new List<IFlipX>() { bars, physics });
    }

    private void FixedUpdate()
    {
        if (FsmState == PlayerState.Dead)
        {
            return;
        }

        UpdatePositionBasedOnInput();
        UpdateCastCycleStates();
        UpdateToIdleIfIdle();
        RegenerateHealth();
        timers.TickAll();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        FsmState = PlayerState.Hurt;
        hurtTimer.Start();
        health.Health -= 10;
    }

    // Helpers

    private void AdvanceChargeTimer()
    {
        if (Charge >= chargeTop)
        {
            chargeDirection = -1;
        }
        else if (Charge <= 0)
        {
            chargeDirection = +1;
        }

        Charge += chargeDirection;
    }

    private void CastSpell()
    {
        FsmState = PlayerState.Casting;
        castTimer.Start();

        GameObject bullet = Instantiate(spell, spawnPoint.position, spawnPoint.rotation);
        Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
        if (FlipX)
        {
            bulletBody.velocity = new Vector3(-0.1f * charge, 0.24f * charge, 0);
            bulletBody.angularVelocity = 200 * charge;
        }
        else
        {
            bulletBody.velocity = new Vector3(0.1f * charge, 0.24f * charge, 0);
            bulletBody.angularVelocity = -200 * charge;
        }

        physics.Recoil(castTorque);
        Charge = 0;
    }

    private void RegenerateHealth()
    {
        health.Health += regenPer;
    }

    private void UpdateCastCycleStates()
    {
        if (Input.GetKey("space"))
        {
            if (FsmState == PlayerState.Flying)
            {
                FsmState = PlayerState.Charging;
            }
            else if (FsmState == PlayerState.Charging)
            {
                AdvanceChargeTimer();
            }
        }
        else
        {
            if (FsmState == PlayerState.Charging)
            {
                CastSpell();
            }
        }
    }

    private void UpdatePositionBasedOnInput()
    {
        bool updateX = false;
        bool updateY = false;
        float velocityXTarget = 0;
        float velocityYTarget = 0;

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            updateX = true;
            velocityXTarget = +maxVelocityX;
            FlipX = false;
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            updateX = true;
            velocityXTarget = -maxVelocityX;
            FlipX = true;
        }
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            updateY = true;
            velocityYTarget = +maxVelocityY;
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            updateY = true;
            velocityYTarget = -maxVelocityY;
        }
        physics.ApproachVelocity(updateX, updateY, velocityXTarget, velocityYTarget);
        physics.ApproachAngularVelocity(velocityXTarget, velocityYTarget);
    }

    private void UpdateToIdleIfIdle()
    {
        if (rigidBody2d.velocity.magnitude <= 1.0)
        {
            if (FsmState == PlayerState.Flying)
            {
                FsmState = PlayerState.Still;
                flyingToIdleTimer.Start();
            }
        }
        else
        {
            flyingToIdleTimer.Stop();
        }

        if (((FsmState == PlayerState.Standing || (FsmState == PlayerState.Still) && (Input.anyKey)))) {
            FsmState = PlayerState.Flying;
        }
    }
}
