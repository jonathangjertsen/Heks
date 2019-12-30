using System;
using UnityEngine;

public enum BirdState
{
    MoveHome,
    MoveToPlayer,
    Hurt,
    Dead,
}


[Serializable]
public class Bird
{
    [Space]
    [Header("Chasing behaviour")]
    public float visionRadius = 15f;
    public float overshoot = 0.01f;
    public float maxVelocityX = 10.0f;
    public float maxVelocityY = 10.0f;

    private Vector2 vectorToPlayer;
    private Vector2 home;
    private ICreaturePhysics physics;
    private ICreatureFsm<BirdState> fsm;
    private IFlipX flipX;

    public void Init(ICreaturePhysics physics, ICreatureFsm<BirdState> fsm, IFlipX flipX)
    {
        this.physics = physics;
        this.fsm = fsm;
        this.flipX = flipX;

        this.fsm.State = BirdState.MoveHome;

        home = this.physics.Position();
    }

    public void ApproachHome()
    {
        physics.ApproachVelocity(home - physics.Position());
        fsm.State = BirdState.MoveHome;
    }

    public void NewPlayerPosition(Vector2 playerPosition, bool playerAlive)
    {
        if (fsm.State == BirdState.Dead)
        {
            physics.Accelerate(new Vector2(0, -0.5f));
            return;
        }

        if (!playerAlive)
        {
            ApproachHome();
            return;
        }

        vectorToPlayer = playerPosition - physics.Position();

        flipX.FlipX = vectorToPlayer.x > 0;
        physics.LookAt(playerPosition);

        if (CloseToPlayer())
        {
            DoBehaviourCloseToPlayer();
        }
        else
        {
            ApproachHome();
        }
    }

    private void DoBehaviourCloseToPlayer()
    {
        if (fsm.State == BirdState.Hurt)
        {
            vectorToPlayer *= -1;
        }
        else
        {
            fsm.State = BirdState.MoveToPlayer;
        }

        physics.ApproachVelocity(new Vector2(
            Math.Min(vectorToPlayer.x, maxVelocityX) * (1 + overshoot),
            Math.Min(vectorToPlayer.y, maxVelocityY) * (1 + overshoot)
        ));
    }

    public void Die()
    {
        fsm.State = BirdState.Dead;
    }

    public bool Alive()
    {
        return fsm.State != BirdState.Dead;
    }

    private bool CloseToPlayer()
    {
        return vectorToPlayer.magnitude < visionRadius;
    }

    public void Hurt()
    {
        fsm.State = BirdState.Hurt;
    }

    public void OnHurtCompleted()
    {
        if (fsm.State == BirdState.Dead)
        {
            return;
        }

        fsm.State = CloseToPlayer() ? BirdState.MoveToPlayer : BirdState.MoveHome;
    }
}

public class BirdBehaviour : BaseCreatureBehaviour<BirdState>
{
    public Transform playerTransform;

    public Bird bird;

    [Space] [Header("Audio clips")]
    public AudioClip CryClip;

    [Space] [Header("Sprites")]
    public Sprite DefaultSprite;
    public Sprite ChargingSprite;
    public Sprite HurtSprite;
    public Sprite DeadSprite;

    public override void Die()
    {
        bird.Die();
        base.Die();
    }

    private new void Start()
    {
        base.Start();

        fsm.Add(BirdState.MoveHome, DefaultSprite, null);
        fsm.Add(BirdState.MoveToPlayer, ChargingSprite, CryClip);
        fsm.Add(BirdState.Hurt, HurtSprite, CryClip);
        fsm.Add(BirdState.Dead, DeadSprite, null);

        bird.Init(creature.physics, fsm, creature.flipXItems);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        bird.NewPlayerPosition(player.HeadPosition, player != null && player.Alive());
    }

    private void OnCollisionEnter2D()
    {
        if (bird.Alive())
        {
            creature.Hurt(10, 100);
            bird.Hurt();
        }
    }

    override public void OnHurtCompleted()
    {
        bird.OnHurtCompleted();
    }
}
