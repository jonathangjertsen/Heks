using UnityEngine;
using System;

enum BirdState
{
    MoveHome,
    MoveToPlayer,
};

public class BirdScript : MonoBehaviour
{
    float axCoeffX = 0.01f;
    float axCoeffY = 0.03f;
    float homeX = 0f;
    float homeY = 0f;
    float rotCoeff = 1.3f;
    float maxVelocityX = 5.0f;
    float maxVelocityY = 5.0f;
    float visionRadius = 15f;

    private BirdState state;

    public AudioClip CryClip;
    public AudioSource CrySource;

    public Sprite DefaultSprite;
    public Sprite ChargingSprite;
    public Transform player;

    Animator animator;
    Rigidbody2D rigidBody2d;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CrySource.clip = CryClip;

        state = BirdState.MoveHome;

        homeX = rigidBody2d.position.x;
        homeY = rigidBody2d.position.y;
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
            homeX - rigidBody2d.position.x,
            homeY - rigidBody2d.position.y
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
            distanceToPlayerX,
            distanceToPlayerY
        );

        state = BirdState.MoveToPlayer;
    }

    void FixedUpdate()
    {
        // Calculate distance to player
        float distanceToPlayerX = player.position.x - rigidBody2d.position.x;
        float distanceToPlayerY = player.position.y - rigidBody2d.position.y;

        // Determine whether to approach the player or the home
        if(Math.Pow(distanceToPlayerX, 2) + Math.Pow(distanceToPlayerY, 2) < visionRadius)
        {
            ApproachPlayer(distanceToPlayerX, distanceToPlayerY);
        }
        else
        {
            ApproachHome();
        }

        // Watch the player
        ApproachAngle(distanceToPlayerX, distanceToPlayerY);
        spriteRenderer.flipX = distanceToPlayerX > 0;
    }
}
