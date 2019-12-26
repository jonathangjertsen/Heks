using UnityEngine;
using System;
using System.Collections.Generic;
using FrameTimer;
using CreaturePhysics;
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

public class PlayerScript : MonoBehaviour
{
    public float maxHealth = 100;
    public float regenPer = 0.02f;
    private float health;

    public float chargeTop = 50;
    private float charge;
    private int chargeDirection = 1;

    public float headOffsetX = 4.87f;
    public float headOffsetY = 6.06f;

    CreaturePhysics.CreaturePhysics physics;
    float axCoeffX = 0.01f;
    float axCoeffY = 0.03f;
    float rotCoeff = 1f;
    float maxVelocityX = 10.0f;
    float maxVelocityY = 10.0f;

    int castTorque = 150;

    public HealthBarScript healthBar;
    public HealthBarScript chargeBar;

    // Management of "hurt" and "angry" states
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
    PlayerState state;

    // Audio
    public AudioClip YellClip;
    public AudioClip HurtClip;
    public AudioClip ChargeClip;
    public AudioClip CastClip;
    public AudioSource SfxSource;

    // Sprites
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
    SpriteRenderer spriteRenderer;

    // Manage flipx
    private bool flipX;
    private Vector3 initialScale;
    public bool FlipX {
        get => flipX;
        set {
            if (value)
            {
                transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            }
            else
            {
                transform.localScale = initialScale;
            }
            flipX = value;
            healthBar.FlipX = value;
            chargeBar.FlipX = value;
        }
    }

    public float Health { get => health; set {
            float cappedHealth = value;
            if (value >= maxHealth)
            {
                cappedHealth = maxHealth;
            }
            else if (value <= 0)
            {
                cappedHealth = 0;
                Die();
            }
            healthBar.SetHealth(cappedHealth / maxHealth);
            health = cappedHealth;
        }
    }
    public float Charge
    {
        get => charge; set
        {
            chargeBar.SetHealth(value / chargeTop);
            charge = value;
        }
    }

    public float HeadOffsetX => headOffsetX * transform.localScale.x;

    public float HeadOffsetY => headOffsetY * transform.localScale.y;

    // Ran when the object is started
    void Start()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        physics = new CreaturePhysics.CreaturePhysics(
            rigidBody2d,
            axCoeffX: axCoeffX,
            axCoeffY: axCoeffY,
            rotCoeff: rotCoeff,
            maxVelocityY: maxVelocityY,
            maxVelocityX: maxVelocityX
        );

        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        state = PlayerState.Flying;
        Health = maxHealth;
        Charge = 0;

        // Init timers
        hurtTimer = new Timer(hurtTimerTop, onHurtTimerExpired);
        angryTimer = new Timer(angryTimerTop, onAngryTimerExpired);
        flyingToIdleTimer = new Timer(flyingToIdleTimerTop, onFlyingToIdleTimerExpired);
        castTimer = new Timer(castTimerTop, onCastTimerExpired);
        var timerList = new List<Timer>() { hurtTimer, angryTimer, flyingToIdleTimer, castTimer };
        timers = new TimerCollection(timerList);
    }

    public bool Alive()
    {
        return state != PlayerState.Dead;
    }

    private void Die()
    {
        state = PlayerState.Dead;
        spriteRenderer.sprite = DeadSprite;

        timers.StopAll();
        healthBar.Hide();
        chargeBar.Hide();
    }

    // Update the player's position based on input
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

    private void EnterChargeState()
    {
        // Play the charge clip
        SfxSource.clip = ChargeClip;
        SfxSource.Play();

        // Use the sprite for charging
        spriteRenderer.sprite = ChargingSprite;

        // Set FSM state
        state = PlayerState.Charging;
    }

    private void CastSpell()
    {
        // Play the cast clip
        SfxSource.clip = CastClip;
        SfxSource.Play();

        // Use the sprite for casting
        spriteRenderer.sprite = CastingSprite;

        // Set FSM state
        state = PlayerState.Casting;

        // Countdown to cast finish
        castTimer.Start();

        // Spawn a spell
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

        // Do recoil
        if (FlipX)
        {
            rigidBody2d.angularVelocity += castTorque;
        }
        else
        {
            rigidBody2d.angularVelocity -= castTorque;
        }

        Charge = 0;
    }

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

    private void UpdateCastCycleStates()
    {
        if (Input.GetKey("space"))
        {
            if (state == PlayerState.Flying)
            {
                EnterChargeState();
            }

            if (state == PlayerState.Charging)
            {
                AdvanceChargeTimer();
            }
        }
        else
        {
            if (state == PlayerState.Charging)
            {
                CastSpell();
            }
        }
    }

    private void onCastTimerExpired()
    {
        spriteRenderer.sprite = FlyingSprite;
        state = PlayerState.Flying;
    }

    private void onHurtTimerExpired()
    {
        SfxSource.clip = YellClip;
        SfxSource.Play();

        spriteRenderer.sprite = AngrySprite;
        state = PlayerState.Angry;

        hurtTimer.Stop();
        angryTimer.Start();
    }

    private void onAngryTimerExpired()
    {
        spriteRenderer.sprite = FlyingSprite;
        state = PlayerState.Flying;

        angryTimer.Stop();
    }

    private void onFlyingToIdleTimerExpired()
    {
        spriteRenderer.sprite = StandingSprite;
        state = PlayerState.Standing;
    }

    private void UpdateToIdleIfIdle()
    {
        if (rigidBody2d.velocity.magnitude <= 1.0)
        {
            if (state == PlayerState.Flying)
            {
                state = PlayerState.Still;
                flyingToIdleTimer.Start();
            }
        }
        else
        {
            flyingToIdleTimer.Stop();
        }

        if (((state == PlayerState.Standing)
             || (state == PlayerState.Still))
            && (Input.anyKey)) {
            state = PlayerState.Flying;
            spriteRenderer.sprite = FlyingSprite;
        }
    }

    private void RegenerateHealth()
    {
        health += regenPer;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    private void FixedUpdate()
    {
        if (state == PlayerState.Dead)
        {
            return;
        }

        UpdatePositionBasedOnInput();
        UpdateCastCycleStates();
        UpdateToIdleIfIdle();
        RegenerateHealth();
        healthBar.SetHealth(this.Health / maxHealth);
        timers.TickAll();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        state = PlayerState.Hurt;
        spriteRenderer.sprite = HurtSprite;
        hurtTimer.Start();

        Health -= 10;

        SfxSource.clip = HurtClip;
        SfxSource.Play();
    }
}
