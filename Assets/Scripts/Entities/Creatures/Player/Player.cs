using System;
using UnityEngine;

[Serializable]
public class Player : Creature, IPlayerLocator, ICreatureController, IDealsDamage, ITakesDamage, ITakesStatusEffect
{
    [Space]
    [Header("Movement")]
    [SerializeField] float maxVelocityX = 10.0f;
    [SerializeField] float maxVelocityY = 10.0f;
    [Range(0f, 10f)]
    [SerializeField] float uprightTorque = 4;

    [Space]
    [Header("Charging and casting")]
    [SerializeField] float chargeTop = 50;
    [SerializeField] int castTorque = 150;
    float charge;
    int chargeDirection = 1;
    IBarDisplay chargeBar;
    float Charge
    {
        get => charge; set
        {
            chargeBar.FillTo((charge = value) / chargeTop);
        }
    }

    [Space]
    [Header("FSM timers")]
    [SerializeField] int hurtTimerTop = 30;
    [SerializeField] int angryTimerTop = 60;
    [SerializeField] int flyingToIdleTimerTop = 60;
    [SerializeField] int castTimerTop = 30;

    [Space]
    [Header("Head position")]
    [SerializeField] float headOffsetX = 4.87f;
    [SerializeField] float headOffsetY = 6.06f;

    private float HeadOffsetX => headOffsetX * creature.physics.Size.x;
    private float HeadOffsetY => headOffsetY * creature.physics.Size.y;
    public Vector2 HeadPosition => new Vector2(creature.physics.Position().x + HeadOffsetX, creature.physics.Position().y + HeadOffsetY);

    [Space]
    [Header("SysCollision")]
    [Range(1f, 5f)] [SerializeField] float collisionDefense;
    [Range(1f, 4f)] [SerializeField] float defenseReductionWhileCharging = 1f;
    [Range(1f, 4f)] [SerializeField] float defenseReductionWhilePlunging = 2f;
    [Range(1f, 4f)] [SerializeField] float attackIncreaseWhileCharging = 3f;
    [Range(0f, 100f)] [SerializeField] float collisionAttack;

    public float CollisionDefense {
        get {
            if (fsm.State == PlayerState.Charging)
            {
                return collisionDefense / defenseReductionWhileCharging;
            }
            else if (fsm.State == PlayerState.Plunging)
            {
                return collisionDefense / defenseReductionWhilePlunging;
            }
            return collisionDefense;
        }
        set => collisionDefense = value;
    }
    public float CollisionAttack {
        get {
            if (!IsAlive())
            {
                return 0;
            }

            if(fsm.State == PlayerState.Plunging)
            {
                return collisionAttack * attackIncreaseWhileCharging;
            }

            return collisionAttack;
        }
        set => collisionAttack = value;
    }

    BaseCreature creature;
    ISpellCaster spellSpawner;
    ICreatureFsm<PlayerState> fsm;
    IEventBus events;
    IPlayerInput input;
    PlayerState stateBeforePlunge;

    public void Init(BaseCreature creature, ICreatureFsm<PlayerState> fsm, IBarDisplay chargeBar, ISpellCaster spellSpawner, IPlayerInput input, IEventBus events)
    {
        this.chargeBar = chargeBar;
        this.fsm = fsm;
        this.spellSpawner = spellSpawner;
        this.input = input;
        this.events = events;

        this.creature = creature;
        creature.SetDeathStartedCallback(Die);
        creature.SetHurtFinishedCallback(OnHurtCompleted);
        creature.FlipXItems.Add(spellSpawner);

        InitTimers();
        InitCharge();

        fsm.State = PlayerState.Flying;
    }

    private void Die()
    {
        creature.timers.Stop("flyingToIdle");
        creature.timers.Stop("cast");
        creature.timers.Stop("angry");
        fsm.State = PlayerState.Dead;
        events.PlayerDied();
    }

    public bool IsAlive()
    {
        return fsm.State != PlayerState.Dead;
    }

    public void OnHurtCompleted()
    {
        fsm.State = PlayerState.Angry;
        creature.timers.Start("angry");
    }

    private bool Left() => input.IsHeld(PlayerInputKey.Left);
    private bool Right() => input.IsHeld(PlayerInputKey.Right);
    private float HorizontalDirection() => Right() ? 1 : Left() ? -1 : 0;
    private float VerticalDirection() => Up() ? 1 : Down() ? -1 : 0;
    private bool Up() => input.IsHeld(PlayerInputKey.Up);
    private bool Down() => input.IsHeld(PlayerInputKey.Down);
    private bool Space() => input.IsHeld(PlayerInputKey.Space);
    private bool Any() => input.IsAnyHeld();
    private bool UpdateX() => Left() ^ Right();
    private bool UpdateY() => Up() ^ Down();
    private bool HardDown() => Down() && !(Up() || Right() || Left());

    private void SetStateFromKeyboardInput()
    {
        if (HardDown())
        {
            if (fsm.State != PlayerState.Plunging)
            {
                events.ZoomOutStart();
                fsm.State = PlayerState.Plunging;
            }
        }
        else
        {
            if (fsm.State == PlayerState.Plunging)
            {
                events.ZoomOutStop();
                fsm.State = PlayerState.Flying;
            }
        }
    }

    private Vector2 GetTargetFromKeyboardInput()
    {
        if (HardDown())
        {
            return new Vector2(0, maxVelocityY * -2);
        }
        else
        {
            return new Vector2(
                maxVelocityX * HorizontalDirection(),
                maxVelocityY * VerticalDirection()
            );
        }
    }

    public void FixedUpdate()
    {
        creature.FixedUpdate();

        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        if (input.IsHeld(PlayerInputKey.DebugDie))
        {
            creature.health.Hurt(creature.maxHealth * 2);
        }

        creature.physics.GetUpright(uprightTorque);
        creature.FlipX = Right() ? false : Left() ? true : creature.FlipX;
        SetStateFromKeyboardInput();
        Vector2 target = GetTargetFromKeyboardInput();
        creature.physics.ApproachVelocity(UpdateX(), UpdateY(), target);
        creature.physics.ApproachAngularVelocity(target);
        UpdateCastCycleStates(Space());
        UpdateToIdleIfIdle(Any());
    }

    private void UpdateCastCycleStates(bool charging)
    {
        if (charging)
        {
            if (fsm.State == PlayerState.Flying)
            {
                fsm.State = PlayerState.Charging;
            }
            else if (fsm.State == PlayerState.Charging)
            {
                AdvanceChargeTimer();
            }
        }
        else if (fsm.State == PlayerState.Charging)
        {
            CastSpell();
        }
    }

    private void UpdateToIdleIfIdle(bool anyKeyHeld)
    {
        if (creature.physics.IsIdle())
        {
            if (fsm.State == PlayerState.Flying)
            {
                fsm.State = PlayerState.Still;
                creature.timers.Start("flyingToIdle");
            }
        }
        else
        {
            creature.timers.Stop("flyingToIdle");
        }

        if ((fsm.State == PlayerState.Standing || fsm.State == PlayerState.Still) && anyKeyHeld)
        {
            fsm.State = PlayerState.Flying;
        }
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

    private void InitTimers()
    {
        creature.timers.Add("angry", angryTimerTop, () => fsm.State = PlayerState.Flying);
        creature.timers.Add("flyingToIdle", flyingToIdleTimerTop, () => fsm.State = PlayerState.Standing);
        creature.timers.Add("cast", castTimerTop, () => fsm.State = PlayerState.Flying);
    }

    private void InitCharge()
    {
        Charge = 0;
        chargeBar.FillTo(0);
        creature.bars.Add(chargeBar);
    }

    private void CastSpell()
    {
        fsm.State = PlayerState.Casting;
        creature.timers.Start("cast");
        spellSpawner.Cast(creature.physics.Velocity(), charge / chargeTop);
        creature.physics.Recoil(castTorque);
        Charge = 0;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
    }

    public void TakeDamage(float amount)
    {
        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        if (fsm.State != PlayerState.Hurt && fsm.State != PlayerState.Angry)
        {
            fsm.State = PlayerState.Hurt;
            creature.Hurt(amount, -400);
            events.PlayerDamaged(amount);
        }
    }

    override public void TriggeredWith(ICollisionSystemParticipator other)
    {
        CollisionSystem.RegisterCollision(this, other);
    }

    public void DealDamage(float amount)
    {
    }

    public void TakeStatusEffect(IStatusEffect statusEffect, IDealsStatusEffect dealer)
    {
        if (statusEffect.Type == StatusEffectType.Burn)
        {
            // Do flame
        }
    }
}
