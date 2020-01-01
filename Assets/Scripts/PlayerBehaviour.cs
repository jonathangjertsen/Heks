using System;
using System.Collections.Generic;
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

public enum KeyInput
{
    Left,
    Right,
    Up,
    Down,
    Space
}

public interface IPlayerInput
{
    void Latch();
    bool IsHeld(KeyInput key);
    bool IsAnyHeld();
}

public class PlayerInput : IPlayerInput
{
    private Dictionary<string, KeyInput> stringToKeyInput;
    private Dictionary<KeyInput, bool> keysHeld;

    public PlayerInput()
    {
        stringToKeyInput = new Dictionary<string, KeyInput>
        {
            { "up", KeyInput.Up },
            { "left", KeyInput.Left },
            { "down", KeyInput.Down },
            { "right", KeyInput.Right },
            { "w", KeyInput.Up },
            { "a", KeyInput.Left },
            { "s", KeyInput.Down },
            { "d", KeyInput.Right },
            { "space", KeyInput.Space },
        };
        InitKeysHeld();
    }

    private void InitKeysHeld()
    {
        keysHeld = new Dictionary<KeyInput, bool>
        {
            { KeyInput.Up, false },
            { KeyInput.Left, false },
            { KeyInput.Down, false },
            { KeyInput.Right, false },
            { KeyInput.Space, false },
        };
    }

    public void Latch()
    {
        InitKeysHeld();
        foreach(KeyValuePair<string, KeyInput> pair in stringToKeyInput)
        {
            if (Input.GetKey(pair.Key))
            {
                keysHeld[pair.Value] = true;
            }
        }
    }

    public bool IsHeld(KeyInput key)
    {
        return keysHeld[key];
    }

    public bool IsAnyHeld()
    {
        foreach(KeyValuePair<KeyInput, bool> pair in keysHeld)
        {
            if (pair.Value)
            {
                return true;
            }
        }
        return false;
    }
}

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
    float Charge { get => charge; set => chargeBar.FillTo((charge = value) / chargeTop); }

    [Space]
    [Header("FSM timers")]
    [SerializeField] int hurtTimerTop = 30;
    [SerializeField] int angryTimerTop = 60;
    [SerializeField] int flyingToIdleTimerTop = 60;
    [SerializeField] int castTimerTop = 30;

    ISpellCaster spellSpawner;
    ICreatureFsm<PlayerState> fsm;
    ICreaturePhysics physics;
    ICreatureHealth health;
    IPlayerInput input;
    TimerCollection timers;
    BarCollection bars;

    public void Init(BaseCreature creature, ICreatureFsm<PlayerState> fsm, IBarDisplay chargeBar, ISpellCaster spellSpawner, IPlayerInput input)
    {
        this.chargeBar = chargeBar;
        this.fsm = fsm;
        this.spellSpawner = spellSpawner;
        this.input = input;

        timers = creature.timers;
        bars = creature.bars;
        physics = creature.physics;
        health = creature.health;

        InitTimers();
        InitCharge();
    }

    public void Die()
    {
        fsm.State = PlayerState.Dead;
    }

    public bool Alive()
    {
        return fsm.State != PlayerState.Dead;
    }

    public void OnHurtCompleted()
    {
        fsm.State = PlayerState.Angry;
        timers.Start("angry");
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

    public void FixedUpdate()
    {
        bool right = input.IsHeld(KeyInput.Right);
        bool left = input.IsHeld(KeyInput.Left);
        bool down = input.IsHeld(KeyInput.Down);
        bool up = input.IsHeld(KeyInput.Up);
        bool space = input.IsHeld(KeyInput.Space);

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
        UpdateToIdleIfIdle(input.IsAnyHeld());
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
        timers.Add("angry", new Timer(angryTimerTop, () => fsm.State = PlayerState.Flying));
        timers.Add("flyingToIdle", new Timer(flyingToIdleTimerTop, () => fsm.State = PlayerState.Standing));
        timers.Add("cast", new Timer(castTimerTop, () => fsm.State = PlayerState.Flying));
    }

    private void InitCharge()
    {
        Charge = 0;
        chargeBar.FillTo(0);
        bars.Add(chargeBar);
    }

    private void CastSpell()
    {
        fsm.State = PlayerState.Casting;
        timers.Start("cast");
        spellSpawner.Cast(physics.Velocity(), charge / chargeTop);
        physics.Recoil(castTorque);
        Charge = 0;
    }
}

public class PlayerBehaviour : BaseCreatureBehaviour<PlayerState>
{
    [SerializeField] BarBehaviour chargeBar;
    [SerializeField] GameStateBehaviour gameState;
    [SerializeField] SpellSpawnBehaviour spellSpawn;

    [Space] [Header("Movement")]
    [SerializeField] Player self;

    [Space] [Header("Head position")]
    [SerializeField] float headOffsetX = 4.87f;
    [SerializeField] float headOffsetY = 6.06f;

    [Space] [Header("Audio clips")]
    [SerializeField] AudioClip YellClip;
    [SerializeField] AudioClip HurtClip;
    [SerializeField] AudioClip ChargeClip;
    [SerializeField] AudioClip CastClip;

    [Space] [Header("Sprites")]
    [SerializeField] Sprite FlyingSprite;
    [SerializeField] Sprite StandingSprite;
    [SerializeField] Sprite ChargingSprite;
    [SerializeField] Sprite CastingSprite;
    [SerializeField] Sprite HurtSprite;
    [SerializeField] Sprite AngrySprite;
    [SerializeField] Sprite DeadSprite;

    IPlayerInput input;

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

        input = new PlayerInput();

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

        self.Init(creature, fsm, chargeBar, spellSpawn, input);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (fsm.State == PlayerState.Dead)
        {
            return;
        }

        creature.FlipX = input.IsHeld(KeyInput.Right) ? false : input.IsHeld(KeyInput.Left) ? true : creature.FlipX;
        self.FixedUpdate();

        if (Input.GetKey("x"))
        {
            Die();
        }
    }

    private void Update()
    {
        input.Latch();
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
