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
    private ICreaturePhysics physics;
    private ICreatureFsm<BirdState> fsm;
    private IFlipX flipX;

    public void Init(BaseCreature creature, ICreatureFsm<BirdState> fsm)
    {
        flipX = creature.FlipXItems;
        physics = creature.physics;
        creature.SetOnDeathStartedCallback(() => fsm.State = BirdState.Dead);
        creature.SetOnHurtFinishedCallback(OnHurtCompleted);
        this.fsm = fsm;

        this.fsm.State = BirdState.MoveHome;

        home = this.physics.Position();
    }

    public void ApproachHome()
    {
        physics.ApproachVelocity(home - physics.Position());
        fsm.State = BirdState.MoveHome;
    }

    private bool Alive()
    {
        return fsm.State != BirdState.Dead;
    }

    public void Hurt()
    {
        if (!Alive())
        {
            return;
        }

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

    private bool CloseToPlayer()
    {
        return vectorToPlayer.magnitude < visionRadius;
    }
}
