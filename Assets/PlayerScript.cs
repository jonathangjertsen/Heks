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
    Angry
};

public class PlayerScript : MonoBehaviour
{
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

    int hurtTimer;
    int hurtTimerTop = 30;

    int angryTimer;
    int angryTimerTop = 60;

    int flyingToIdleTimer;
    int flyingToIdleTimerTop = 60;

    PlayerState state;

    public AudioClip YellClip;
    public AudioClip HurtClip;
    public AudioClip ChargeClip;
    public AudioClip CastClip;
    public AudioSource SfxSource;

    public Sprite FlyingSprite;
    public Sprite StandingSprite;
    public Sprite ChargingSprite;
    public Sprite CastingSprite;
    public Sprite HurtSprite;
    public Sprite AngrySprite;

    Animator animator;
    Rigidbody2D rigidBody2d;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        state = PlayerState.Flying;
    }

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

    private void ApproachAngularVelocity(float velocityXTarget, float velocityYTarget)
    {
        float targetRotation = (float)Math.Atan2(velocityYTarget, velocityXTarget);
        float currentRotation = rigidBody2d.rotation;
        rigidBody2d.angularVelocity += rotCoeff * (targetRotation - currentRotation);
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
            spriteRenderer.flipX = false;
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            updateX = true;
            velocityXTarget = -maxVelocityX;
            spriteRenderer.flipX = true;
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

    private void UpdateCastCycleStates()
    {
        if (Input.GetKey("space"))
        {
            if (state == PlayerState.Flying)
            {
                SfxSource.clip = ChargeClip;
                SfxSource.Play();

                spriteRenderer.sprite = ChargingSprite;
                state = PlayerState.Charging;
            }
        }
        else
        {
            if (state == PlayerState.Charging)
            {
                SfxSource.clip = CastClip;
                SfxSource.Play();

                spriteRenderer.sprite = CastingSprite;
                state = PlayerState.Casting;
                castTimer = castTimerTop;
                if (spriteRenderer.flipX)
                {
                    rigidBody2d.angularVelocity += castTorque;
                }
                else
                {
                    rigidBody2d.angularVelocity -= castTorque;
                }
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

    private void FixedUpdate()
    {
        UpdatePositionBasedOnInput();
        UpdateCastCycleStates();
        UpdateHurtStates();
        UpdateToIdleIfIdle();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        state = PlayerState.Hurt;
        spriteRenderer.sprite = HurtSprite;
        hurtTimer = hurtTimerTop;

        SfxSource.clip = HurtClip;
        SfxSource.Play();
    }
}
