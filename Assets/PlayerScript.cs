using UnityEngine;
using System;

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

    float axCoeffX = 0.01f;
    float axCoeffY = 0.03f;
    float rotCoeff = 1f;
    float maxVelocityX = 10.0f;
    float maxVelocityY = 10.0f;

    int castTimer;
    int castTimerTop = 30;
    int castTorque = 150;

    public HealthBarScript healthBar;
    public HealthBarScript chargeBar;

    // Management of "hurt" and "angry" states
    public int hurtTimerTop = 30;
    public int angryTimerTop = 60;
    int hurtTimer;
    int angryTimer;

    // Management of idle state
    public int flyingToIdleTimerTop = 60;
    int flyingToIdleTimer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        state = PlayerState.Flying;
        Health = maxHealth;
        Charge = 0;
    }

    // Make the player's velocity approach some max velocity
    private void ApproachVelocity(bool updateX, bool updateY, float velocityXTarget, float velocityYTarget)
    {
        float axX = 0;
        float axY = 0;

        if (updateX)
        {
            axX = axCoeffX * (velocityXTarget - rigidBody2d.velocity.x);
        }

        if (updateY)
        {
            axY = axCoeffY * (velocityYTarget - rigidBody2d.velocity.y);
        }

        if (updateX || updateY)
        {
            rigidBody2d.velocity += new Vector2(axX, axY);
        }
    }

    // Make the player's angular velocity approach some maximum
    private void ApproachAngularVelocity(float velocityXTarget, float velocityYTarget)
    {
        float targetRotation = (float)Math.Atan2(velocityYTarget, velocityXTarget);
        float currentRotation = rigidBody2d.rotation;
        rigidBody2d.angularVelocity += rotCoeff * (targetRotation - currentRotation);
    }

    public bool Alive()
    {
        return state != PlayerState.Dead;
    }

    private void Die()
    {
        state = PlayerState.Dead;
        spriteRenderer.sprite = DeadSprite;

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
        ApproachVelocity(updateX, updateY, velocityXTarget, velocityYTarget);
        ApproachAngularVelocity(velocityXTarget, velocityYTarget);
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
        castTimer = castTimerTop;

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

        if (state == PlayerState.Casting)
        {
            castTimer -= 1;
            if (castTimer <= 0)
            {
                spriteRenderer.sprite = FlyingSprite;
                state = PlayerState.Flying;
            }
        }
    }

    private void UpdateHurtStates()
    {
        if (state == PlayerState.Hurt)
        {
            hurtTimer -= 1;
            if (hurtTimer <= 0)
            {
                SfxSource.clip = YellClip;
                SfxSource.Play();

                spriteRenderer.sprite = AngrySprite;
                state = PlayerState.Angry;
                angryTimer = angryTimerTop;
            }
        }

        if (state == PlayerState.Angry)
        {
            angryTimer -= 1;
            if (angryTimer <= 0)
            {
                spriteRenderer.sprite = FlyingSprite;
                state = PlayerState.Flying;
            }
        }
    }

    private void UpdateToIdleIfIdle()
    {
        if (rigidBody2d.velocity.magnitude <= 1.0)
        {
            if (state == PlayerState.Flying)
            {
                state = PlayerState.Still;
                flyingToIdleTimer = flyingToIdleTimerTop;
            }
            else if (state == PlayerState.Still)
            {
                flyingToIdleTimer -= 1;
                if (flyingToIdleTimer <= 0)
                {
                    spriteRenderer.sprite = StandingSprite;
                    state = PlayerState.Standing;
                }
            }
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
        UpdateHurtStates();
        UpdateToIdleIfIdle();
        RegenerateHealth();
        healthBar.SetHealth(this.Health / maxHealth);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        state = PlayerState.Hurt;
        spriteRenderer.sprite = HurtSprite;
        hurtTimer = hurtTimerTop;

        this.Health -= 10;

        SfxSource.clip = HurtClip;
        SfxSource.Play();
    }
}
