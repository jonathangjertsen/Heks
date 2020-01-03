using System;
using UnityEngine;

[Serializable]
public class Bird
{
    [Space]
    [Header("Chasing behaviour")]
    [SerializeField] float visionRadius = 15f;
    [SerializeField] float overshoot = 0.01f;
    [SerializeField] float maxVelocityX = 10.0f;
    [SerializeField] float maxVelocityY = 10.0f;

    private Vector2 vectorToPlayer;
    private Vector2 home;
    BaseCreature creature;
    public ICreatureFsm<BirdState> fsm { get; private set; }
    private IFlipX flipX;
    private IPlayerLocator playerLocator;

    public void Init(BaseCreature creature, ICreatureFsm<BirdState> fsm, IPlayerLocator playerLocator)
    {
        this.creature = creature;
        this.fsm = fsm;
        this.playerLocator = playerLocator;

        flipX = creature.FlipXItems;
        creature.SetDeathStartedCallback(() => fsm.State = BirdState.Dead);
        creature.SetHurtFinishedCallback(OnHurtCompleted);
        creature.SetHurtCallback(Hurt);

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

    private void Hurt(float amount)
    {
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
        flipX.FlipX = vectorToPlayer.x > 0;
        creature.physics.LookAt(playerPosition);
    }
}
