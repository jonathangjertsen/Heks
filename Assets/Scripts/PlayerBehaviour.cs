using UnityEngine;

public enum PlayerState
{
    Flying,
    Still,
    Standing,
    Charging,
    Casting,
    Hurt,
    Angry,
    Dead,
};

public class PlayerBehaviour : BaseCreature<PlayerState>, IFlipX
{
    // Health
    public float regenPer = 0.02f;
    public float headOffsetX = 4.87f;
    public float headOffsetY = 6.06f;

    // Things related to charging
    public BarBehaviour chargeBar;
    public float chargeTop = 50;
    private float charge;
    private int chargeDirection = 1;

    // Things related to casting
    public int castTorque = 150;

    // Timers
    public int hurtTimerTop = 30;
    public int angryTimerTop = 60;
    public int flyingToIdleTimerTop = 60;
    public int castTimerTop = 30;

    // FSM state
    public AudioClip YellClip;
    public AudioClip HurtClip;
    public AudioClip ChargeClip;
    public AudioClip CastClip;
    public Sprite FlyingSprite;
    public Sprite StandingSprite;
    public Sprite ChargingSprite;
    public Sprite CastingSprite;
    public Sprite HurtSprite;
    public Sprite AngrySprite;
    public Sprite DeadSprite;

    public GameStateBehaviour gameState;

    // Spell management
    public SpellSpawnBehaviour spellSpawn;

    // Simple properties
    public float Charge
    {
        get => charge;
        set
        {
            chargeBar.FillTo(value / chargeTop);
            charge = value;
        }
    }
    private float HeadOffsetX => headOffsetX * transform.localScale.x;
    private float HeadOffsetY => headOffsetY * transform.localScale.y;
    public Vector2 HeadPosition => new Vector2(transform.position.x + HeadOffsetX, transform.position.y + HeadOffsetY);

    public override void Die()
    {
        base.Die();
        FsmState = PlayerState.Dead;
        gameState.PlayerDied();
    }

    public bool Alive()
    {
        return FsmState != PlayerState.Dead;
    }

    // Timer callbacks

    private void OnAngryTimerExpired()
    {
        FsmState = PlayerState.Flying;
        timers.Stop("angry");
    }

    private void OnCastTimerExpired()
    {
        FsmState = PlayerState.Flying;
    }

    private void OnFlyingToIdleTimerExpired()
    {
        FsmState = PlayerState.Standing;
    }

    private void OnHurtTimerExpired()
    {
        FsmState = PlayerState.Angry;
        timers.Stop("hurt");
        timers.Start("angry");
    }

    // Unity

    private new void Start()
    {
        base.Start();

        Charge = 0;
        chargeBar.FillTo(0);
        bars.Add(chargeBar);

        fsm.Add(PlayerState.Angry, AngrySprite, YellClip);
        fsm.Add(PlayerState.Hurt, HurtSprite, HurtClip);
        fsm.Add(PlayerState.Casting, CastingSprite, CastClip);
        fsm.Add(PlayerState.Dead, DeadSprite, null);
        fsm.Add(PlayerState.Flying, FlyingSprite, null);
        fsm.Add(PlayerState.Standing, StandingSprite, null);
        fsm.Add(PlayerState.Still, FlyingSprite, null);
        fsm.Add(PlayerState.Charging, ChargingSprite, ChargeClip);
        FsmState = PlayerState.Flying;

        timers.Add("hurt", new Timer(hurtTimerTop, OnHurtTimerExpired));
        timers.Add("angry", new Timer(angryTimerTop, OnAngryTimerExpired));
        timers.Add("flyingToIdle", new Timer(flyingToIdleTimerTop, OnFlyingToIdleTimerExpired));
        timers.Add("cast", new Timer(castTimerTop, OnCastTimerExpired));

        flipXItems.Add(spellSpawn);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (FsmState == PlayerState.Dead)
        {
            return;
        }

        UpdatePositionBasedOnInput();
        UpdateCastCycleStates();
        UpdateToIdleIfIdle();
        RegenerateHealth();

        if (Input.GetKey("x"))
        {
            Die();
        }

        base.FixedUpdate();
    }

    private void OnTriggerEnter2D()
    {
        if (FsmState == PlayerState.Dead)
        {
            return;
        }

        if (FsmState != PlayerState.Hurt && FsmState != PlayerState.Angry)
        {
            FsmState = PlayerState.Hurt;
            timers.Start("hurt");
            health.Health -= 10;
        }
    }

    // Helpers

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

    private void CastSpell()
    {
        FsmState = PlayerState.Casting;
        timers.Start("cast");
        spellSpawn.Cast(physics.Velocity(), charge / chargeTop);
        physics.Recoil(castTorque);
        Charge = 0;
    }

    private void RegenerateHealth()
    {
        health.Health += regenPer;
    }

    private void UpdateCastCycleStates()
    {
        if (Input.GetKey("space"))
        {
            if (FsmState == PlayerState.Flying)
            {
                FsmState = PlayerState.Charging;
            }
            else if (FsmState == PlayerState.Charging)
            {
                AdvanceChargeTimer();
            }
        }
        else if (FsmState == PlayerState.Charging)
        {
            CastSpell();
        }
    }

    private void UpdatePositionBasedOnInput()
    {
        bool updateX = false;
        bool updateY = false;
        Vector2 target = new Vector2(0, 0);

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            updateX = true;
            target.x = maxVelocityX;
            FlipX = false;
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            updateX = true;
            target.x = -maxVelocityX;
            FlipX = true;
        }
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            updateY = true;
            target.y = +maxVelocityY;
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            updateY = true;
            target.y = -maxVelocityY;
        }
        physics.ApproachVelocity(updateX, updateY, target);
        physics.ApproachAngularVelocity(target);
    }

    private void UpdateToIdleIfIdle()
    {
        if (physics.IsIdle())
        {
            if (FsmState == PlayerState.Flying)
            {
                FsmState = PlayerState.Still;
                timers.Start("flyingToIdle");
            }
        }
        else
        {
            timers.Stop("flyingToIdle");
        }

        if ((FsmState == PlayerState.Standing || FsmState == PlayerState.Still) && Input.anyKey)
        {
            FsmState = PlayerState.Flying;
        }
    }
}
