using System;
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

[Serializable]
public class Player
{
    public float maxVelocityX = 10.0f;
    public float maxVelocityY = 10.0f;

    [Space]
    [Header("Charging and casting")]
    public float chargeTop = 50;
    public int castTorque = 150;
    private float charge;
    private int chargeDirection = 1;
    private IBarDisplay chargeBar;
    private float Charge { get => charge; set => chargeBar.FillTo((charge = value) / chargeTop); }

    [Space]
    [Header("FSM timers")]
    public int hurtTimerTop = 30;
    public int angryTimerTop = 60;
    public int flyingToIdleTimerTop = 60;
    public int castTimerTop = 30;

    private ISpellCaster spellSpawner;
    private ICreatureFsm<PlayerState> fsm;
    private ICreaturePhysics physics;
    private ICreatureHealth health;
    private TimerCollection timers;
    private BarCollection bars;

    public void Init(ICreaturePhysics physics, ICreatureFsm<PlayerState> fsm, TimerCollection timers, BarCollection bars, IBarDisplay chargeBar, ISpellCaster spellSpawner, ICreatureHealth health)
    {
        this.chargeBar = chargeBar;
        this.fsm = fsm;
        this.timers = timers;
        this.bars = bars;
        this.physics = physics;
        this.spellSpawner = spellSpawner;
        this.health = health;

        this.timers.Add("angry", new Timer(angryTimerTop, OnAngryTimerExpired));
        this.timers.Add("flyingToIdle", new Timer(flyingToIdleTimerTop, OnFlyingToIdleTimerExpired));
        this.timers.Add("cast", new Timer(castTimerTop, OnCastTimerExpired));

        Charge = 0;
        chargeBar.FillTo(0);
        this.bars.Add(chargeBar);
    }

    public void Die()
    {
        fsm.State = PlayerState.Dead;
    }

    public bool Alive()
    {
        return fsm.State != PlayerState.Dead;
    }

    private void OnAngryTimerExpired()
    {
        fsm.State = PlayerState.Flying;
        timers.Stop("angry");
    }

    private void OnCastTimerExpired()
    {
        fsm.State = PlayerState.Flying;
    }

    private void OnFlyingToIdleTimerExpired()
    {
        fsm.State = PlayerState.Standing;
    }

    public void OnHurtCompleted()
    {
        fsm.State = PlayerState.Angry;
        timers.Stop("hurt");
        timers.Start("angry");
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

    private void CastSpell()
    {
        fsm.State = PlayerState.Casting;
        timers.Start("cast");
        spellSpawner.Cast(physics.Velocity(), charge / chargeTop);
        physics.Recoil(castTorque);
        Charge = 0;
    }

    public void UpdateCastCycleStates(bool charging)
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

    public void OnTriggerEnter2D()
    {
        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        if (fsm.State != PlayerState.Hurt && fsm.State != PlayerState.Angry)
        {
            fsm.State = PlayerState.Hurt;
            timers.Start("hurt");
            health.Health -= 10;
        }
    }

    public void UpdateToIdleIfIdle(bool anyKeyHeld)
    {
        if (physics.IsIdle())
        {
            if (fsm.State == PlayerState.Flying)
            {
                fsm.State = PlayerState.Still;
                timers.Start("flyingToIdle");
            }
        }
        else
        {
            timers.Stop("flyingToIdle");
        }

        if ((fsm.State == PlayerState.Standing || fsm.State == PlayerState.Still) && anyKeyHeld)
        {
            fsm.State = PlayerState.Flying;
        }
    }

    public void KeyInput(bool right, bool left, bool up, bool down, bool space)
    {
        bool any = right || left || up || down || space;

        // Movement
        Vector2 target = new Vector2(0, 0);
        bool updateX = right ^ left;
        bool updateY = up ^ down;
        target.x = maxVelocityX * (right ? 1 : left ? -1 : 0);
        target.y = maxVelocityY * (up ? 1 : down ? -1 : 0);
        physics.ApproachVelocity(updateX, updateY, target);
        physics.ApproachAngularVelocity(target);

        // Cast
        UpdateCastCycleStates(space);

        // Idle
        UpdateToIdleIfIdle(any);
    }
}

public class PlayerBehaviour : BaseCreatureBehaviour<PlayerState>
{
    public BarBehaviour chargeBar;
    public GameStateBehaviour gameState;
    public SpellSpawnBehaviour spellSpawn;

    [Space] [Header("Movement")]
    public Player self;

    [Space] [Header("Head position")]
    public float headOffsetX = 4.87f;
    public float headOffsetY = 6.06f;

    [Space] [Header("Audio clips")]
    public AudioClip YellClip;
    public AudioClip HurtClip;
    public AudioClip ChargeClip;
    public AudioClip CastClip;

    [Space] [Header("Sprites")]
    public Sprite FlyingSprite;
    public Sprite StandingSprite;
    public Sprite ChargingSprite;
    public Sprite CastingSprite;
    public Sprite HurtSprite;
    public Sprite AngrySprite;
    public Sprite DeadSprite;

    // Simple properties
    
    private float HeadOffsetX => headOffsetX * transform.localScale.x;
    private float HeadOffsetY => headOffsetY * transform.localScale.y;
    public Vector2 HeadPosition => new Vector2(transform.position.x + HeadOffsetX, transform.position.y + HeadOffsetY);

    public override void Die()
    {
        base.Die();
        self.Die();
        gameState.PlayerDied();
    }

    // Unity

    private new void Start()
    {
        base.Start();

        fsm.Add(PlayerState.Angry, AngrySprite, YellClip);
        fsm.Add(PlayerState.Hurt, HurtSprite, HurtClip);
        fsm.Add(PlayerState.Casting, CastingSprite, CastClip);
        fsm.Add(PlayerState.Dead, DeadSprite, null);
        fsm.Add(PlayerState.Flying, FlyingSprite, null);
        fsm.Add(PlayerState.Standing, StandingSprite, null);
        fsm.Add(PlayerState.Still, FlyingSprite, null);
        fsm.Add(PlayerState.Charging, ChargingSprite, ChargeClip);
        fsm.State = PlayerState.Flying;

        creature.flipXItems.Add(spellSpawn);

        self.Init(creature.physics, fsm, creature.timers, creature.bars, chargeBar, spellSpawn, creature.health);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        bool right = Input.GetKey("d") || Input.GetKey("right");
        bool left = Input.GetKey("a") || Input.GetKey("left");
        bool up = Input.GetKey("w") || Input.GetKey("up");
        bool down = Input.GetKey("s") || Input.GetKey("down");
        bool space = Input.GetKey("space");
        creature.FlipX = right ? false : left ? true : creature.FlipX;
        self.KeyInput(right, left, up, down, space);

        if (Input.GetKey("x"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D()
    {
        self.OnTriggerEnter2D();
    }

    public bool Alive()
    {
        return self.Alive();
    }

    public override void OnHurtCompleted()
    {
        self.OnHurtCompleted();
    }
}
