using UnityEngine;
using System;

enum BirdState
{
    MoveHome,
    MoveToPlayer,
    Dead,
};

public class BirdScript : MonoBehaviour
{
    public float maxHealth = 100;
    public float regenPer = 0.02f;
    private float health;

    public float axCoeffX = 0.02f;
    public float axCoeffY = 0.06f;
    public float visionRadius = 15f;
    public float overshoot = 0.01f;
    public float maxVelocity = 5.0f;

    float homeX = 0f;
    float homeY = 0f;
    float rotCoeff = 1.3f;

    public PlayerScript player;
    private BirdState state;

    public AudioClip CryClip;
    public AudioSource CrySource;

    public Sprite DefaultSprite;
    public Sprite ChargingSprite;
    public Transform playerTransform;
    public HealthBarScript healthBar;

    Animator animator;
    Rigidbody2D rigidBody2d;
    SpriteRenderer spriteRenderer;

    public float Health
    {
        get => health; set
        {
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

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CrySource.clip = CryClip;

        state = BirdState.MoveHome;

        homeX = rigidBody2d.position.x;
        homeY = rigidBody2d.position.y;

        Health = maxHealth;
    }

    private void ApproachAngle(float velocityXTarget, float velocityYTarget)
    {
        // Update the angular velocity according to the X and Y components of velocity
        float targetRotation = (float)Math.Atan2(velocityYTarget, velocityXTarget);
        float currentRotation = rigidBody2d.rotation;
        rigidBody2d.rotation += rotCoeff * (targetRotation - currentRotation);
    }

    private void ApproachVelocity(float velocityXTarget, float velocityYTarget)
    {
        // Update the velocity vector towards the target vector
        float axX = axCoeffX * (velocityXTarget - rigidBody2d.velocity.x);
        float axY = axCoeffY * (velocityYTarget - rigidBody2d.velocity.y);
        rigidBody2d.velocity += new Vector2(axX, axY);
    }

    void ApproachHome()
    {
        // Approach home
        spriteRenderer.sprite = DefaultSprite;
        ApproachVelocity(
            (homeX - rigidBody2d.position.x) * (1 + overshoot),
            (homeY - rigidBody2d.position.y) * (1 + overshoot)
        );

        state = BirdState.MoveHome;
    }

    void ApproachPlayer(float distanceToPlayerX, float distanceToPlayerY)
    {
        // Approach the player
        if (state != BirdState.MoveToPlayer)
        {
            CrySource.Play();
        }

        spriteRenderer.sprite = ChargingSprite;
        ApproachVelocity(
            Math.Min(distanceToPlayerX, maxVelocity),
            Math.Min(distanceToPlayerY, maxVelocity)
        );

        state = BirdState.MoveToPlayer;
    }

    void FixedUpdate()
    {
        if (state == BirdState.Dead)
        {
            rigidBody2d.velocity += new Vector2(0, -0.5f);
            return;
        }

        // Calculate distance to player
        float distanceToPlayerX = playerTransform.position.x + player.HeadOffsetX - rigidBody2d.position.x;
        float distanceToPlayerY = playerTransform.position.y + player.HeadOffsetY - rigidBody2d.position.y;

        if (player.Alive())
        {
            // Determine whether to approach the player or the home
            if (Math.Pow(distanceToPlayerX, 2) + Math.Pow(distanceToPlayerY, 2) < visionRadius)
            {
                ApproachPlayer(distanceToPlayerX, distanceToPlayerY);
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

        // Watch the player
        ApproachAngle(distanceToPlayerX, distanceToPlayerY);
        spriteRenderer.flipX = distanceToPlayerX > 0;

        RegenerateHealth();
    }

    private void RegenerateHealth()
    {
        health += regenPer;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    void Die()
    {
        state = BirdState.Dead;

        // Doesn't seem like the right way
        Destroy(healthBar.transform.parent.gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Health -= 10;
    }
}
