using UnityEngine;
using System;
using System.Collections.Generic;

enum BirdState
{
    MoveHome,
    MoveToPlayer,
    Dead,
};

public class BirdScript : MonoBehaviour, IFlipX
{
    CreatureFsm.CreatureFsm fsm;

    public float maxHealth = 100;
    public float regenPer = 0.02f;
    CreatureHealth health;

    CreaturePhysics.CreaturePhysics physics;
    public float axCoeffX = 0.02f;
    public float axCoeffY = 0.06f;
    public float visionRadius = 15f;
    public float overshoot = 0.01f;
    public float maxVelocity = 5.0f;

    float homeX;
    float homeY;
    public float rotCoeff = 1.3f;

    public PlayerScript player;

    public AudioClip CryClip;

    public Sprite DefaultSprite;
    public Sprite ChargingSprite;
    public Transform playerTransform;
    public Bar healthBar;

    Rigidbody2D rigidBody2d;
    private BirdState FsmState { get => (BirdState)fsm.State; set => fsm.State = (int)value; }
    public bool FlipX { get => physics.FlipX; set => physics.FlipX = value; }

    void Start()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        physics = new CreaturePhysics.CreaturePhysics(
            this,
            axCoeffX: axCoeffX,
            axCoeffY: axCoeffY,
            rotCoeff: rotCoeff,
            maxVelocityY: maxVelocity,
            maxVelocityX: maxVelocity
        );

        homeX = rigidBody2d.position.x;
        homeY = rigidBody2d.position.y;

        health = new CreatureHealth(healthBar, maxHealth, onZeroHealth: Die);

        var fsmSprites = new Dictionary<int, Sprite>
        {
            { (int)BirdState.MoveHome, DefaultSprite },
            { (int)BirdState.MoveToPlayer, ChargingSprite },
            { (int)BirdState.Dead, DefaultSprite }
        };
        var fsmClips = new Dictionary<int, AudioClip>
        {
            { (int)BirdState.MoveToPlayer, CryClip }
        };
        fsm = new CreatureFsm.CreatureFsm(gameObject, fsmSprites, fsmClips)
        {
            State = (int)BirdState.MoveHome
        };
    }

    void ApproachHome()
    {
        physics.ApproachVelocity(
            (homeX - rigidBody2d.position.x),
            (homeY - rigidBody2d.position.y)
        );
        FsmState = BirdState.MoveHome;
    }

    void ApproachPlayer(float distanceToPlayerX, float distanceToPlayerY)
    {
        physics.ApproachVelocity(
            Math.Min(distanceToPlayerX, maxVelocity) * (1 + overshoot),
            Math.Min(distanceToPlayerY, maxVelocity) * (1 + overshoot)
        );
        FsmState = BirdState.MoveToPlayer;
    }

    void FixedUpdate()
    {
        if (FsmState == BirdState.Dead)
        {
            rigidBody2d.velocity += new Vector2(0, -0.5f);
            return;
        }

        // Calculate distance to player
        float distanceToPlayerX = playerTransform.position.x + player.HeadOffsetX - rigidBody2d.position.x;
        float distanceToPlayerY = playerTransform.position.y + player.HeadOffsetY - rigidBody2d.position.y;
        FlipX = distanceToPlayerX > 0;
        physics.LookAt(player.transform);

        if (player.Alive)
        {
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

        RegenerateHealth();
    }

    private void RegenerateHealth()
    {
        health.Health += regenPer;
    }

    void Die()
    {
        FsmState = BirdState.Dead;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        health.Health -= 10;
        physics.Recoil(100);
    }
}
