using System;
using UnityEngine;

[Serializable]
public class Bird : Creature, ICreatureController, ITakesDamage, IDealsDamage
{
    [Space]
    [Header("Chasing behaviour")]
    [SerializeField] float visionRadius = 15f;
    [SerializeField] float overshoot = 0.01f;
    [SerializeField] float maxVelocityX = 10.0f;
    [SerializeField] float maxVelocityY = 10.0f;

    [Space]
    [Header("SysCollision")]
    [Range(1f, 5f)] [SerializeField] float collisionDefense;
    [Range(0f, 100f)] [SerializeField] float collisionAttack;

    public float CollisionDefense { get => collisionDefense; set => collisionDefense = value; }
    public float CollisionAttack { get => Alive() ? collisionAttack : 0; set => collisionAttack = value; }

    private Vector2 vectorToPlayer;
    private Vector2 home;
    BaseCreature creature;
    public ICreatureFsm<BirdState> fsm { get; private set; }
    private IPlayerLocator playerLocator;

    public void Init(BaseCreature creature, ICreatureFsm<BirdState> fsm, IPlayerLocator playerLocator)
    {
        this.creature = creature;
        this.fsm = fsm;
        this.playerLocator = playerLocator;

        creature.SetDeathStartedCallback(() => fsm.State = BirdState.Dead);
        creature.SetHurtFinishedCallback(OnHurtCompleted);

        this.fsm.State = BirdState.MoveHome;

        home = creature.physics.Position();
    }

    public void FixedUpdate()
    {
        creature.FixedUpdate();

        if (fsm.State == BirdState.Dead)
        {
            creature.physics.Accelerate(new Vector2(0, -0.5f));
            return;
        }

        if (PlayerIsNullOrDead())
        {
            ApproachHome();
            return;
        }

        FindPlayerLocation();
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

        creature.physics.ApproachVelocity(new Vector2(
            Math.Min(vectorToPlayer.x, maxVelocityX) * (1 + overshoot),
            Math.Min(vectorToPlayer.y, maxVelocityY) * (1 + overshoot)
        ));
    }

    private bool CloseToPlayer()
    {
        return vectorToPlayer.magnitude < visionRadius;
    }

    private void ApproachHome()
    {
        creature.physics.ApproachVelocity(home - creature.physics.Position());
        fsm.State = BirdState.MoveHome;
    }

    private bool Alive()
    {
        return fsm.State != BirdState.Dead;
    }

    private void OnHurtCompleted()
    {
        if (fsm.State == BirdState.Dead)
        {
            return;
        }

        fsm.State = CloseToPlayer() ? BirdState.MoveToPlayer : BirdState.MoveHome;
    }

    public void TakeDamage(float amount)
    {
        creature.Hurt(amount, 100f);
        if (!Alive())
        {
            return;
        }
        fsm.State = BirdState.Hurt;
    }

    private bool PlayerIsNullOrDead()
    {
        return playerLocator == null || !playerLocator.IsAlive();
    }

    private void FindPlayerLocation()
    {
        Vector2 playerPosition = playerLocator.HeadPosition;
        vectorToPlayer = playerPosition - creature.physics.Position();
        creature.FlipX = vectorToPlayer.x > 0;
        creature.physics.LookAt(playerPosition);
    }

    public void TriggeredWith(ICollisionSystemParticipator other)
    {
    }

    public void ExitedTriggerWith(ICollisionSystemParticipator other)
    {
    }

    public ICollisionSystemParticipator GetCollisionSystemParticipator() => this;

    public void CollidedWith(ICollisionSystemParticipator other)
    {
        CollisionSystem.RegisterCollision(this, other);
    }

    public void ExitedCollisionWith(ICollisionSystemParticipator other)
    {
    }

    public void DealDamage(float amount)
    {
    }
}
