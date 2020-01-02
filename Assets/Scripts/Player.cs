using System;
using UnityEngine;

[Serializable]
public class Player
{
    [SerializeField] float maxVelocityX = 10.0f;
    [SerializeField] float maxVelocityY = 10.0f;

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

    BaseCreature creature;
    ISpellCaster spellSpawner;
    ICreatureFsm<PlayerState> fsm;
    IEventBus events;
    IPlayerInput input;

    public void Init(BaseCreature creature, ICreatureFsm<PlayerState> fsm, IBarDisplay chargeBar, ISpellCaster spellSpawner, IPlayerInput input, IEventBus events)
    {
        this.chargeBar = chargeBar;
        this.fsm = fsm;
        this.spellSpawner = spellSpawner;
        this.input = input;
        this.events = events;

        this.creature = creature;
        creature.SetOnDeathStartedCallback(Die);
        creature.SetOnHurtFinishedCallback(OnHurtCompleted);
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

    public bool Alive()
    {
        return fsm.State != PlayerState.Dead;
    }

    public void OnHurtCompleted()
    {
        fsm.State = PlayerState.Angry;
        creature.timers.Start("angry");
    }

    public void OnTriggerEnter2D()
    {
        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        if (fsm.State != PlayerState.Hurt && fsm.State != PlayerState.Angry)
        {
            fsm.State = PlayerState.Hurt;
            creature.Hurt(10, -400);
        }
    }

    public void FixedUpdate()
    {
        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        if (input.IsHeld(PlayerInputKey.DebugDie))
        {
            creature.health.Hurt(creature.maxHealth * 2);
        }

        creature.FlipX = input.IsHeld(PlayerInputKey.Right) ? false : input.IsHeld(PlayerInputKey.Left) ? true : creature.FlipX;

        bool right = input.IsHeld(PlayerInputKey.Right);
        bool left = input.IsHeld(PlayerInputKey.Left);
        bool down = input.IsHeld(PlayerInputKey.Down);
        bool up = input.IsHeld(PlayerInputKey.Up);
        bool space = input.IsHeld(PlayerInputKey.Space);

        // Movement
        Vector2 target = new Vector2(0, 0);
        bool updateX = right ^ left;
        bool updateY = up ^ down;
        target.x = maxVelocityX * (right ? 1 : left ? -1 : 0);
        target.y = maxVelocityY * (up ? 1 : down ? -1 : 0);
        creature.physics.ApproachVelocity(updateX, updateY, target);
        creature.physics.ApproachAngularVelocity(target);

        // Cast
        UpdateCastCycleStates(space);

        // Idle
        UpdateToIdleIfIdle(input.IsAnyHeld());
    }

    private void UpdateCastCycleStates(bool charging)
    {
        if (charging)
        {
            if (fsm.State == PlayerState.Flying)
            {
                fsm.State = PlayerState.Charging;
                events.ChargeStart();
            }
            else if (fsm.State == PlayerState.Charging)
            {
                AdvanceChargeTimer();
            }
        }
        else if (fsm.State == PlayerState.Charging)
        {
            CastSpell();
            events.ChargeStop();
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
        creature.timers.Add("angry", new Timer(angryTimerTop, () => fsm.State = PlayerState.Flying));
        creature.timers.Add("flyingToIdle", new Timer(flyingToIdleTimerTop, () => fsm.State = PlayerState.Standing));
        creature.timers.Add("cast", new Timer(castTimerTop, () => fsm.State = PlayerState.Flying));
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
}
