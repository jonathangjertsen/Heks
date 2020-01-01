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
    Space,
    DebugDie
}

public interface IPlayerInput
{
    void Latch();
    bool IsHeld(KeyInput key);
    bool IsAnyHeld();
}

public class PlayerInput : IPlayerInput
{
    // Allow a singleton interface
    static PlayerInput instance;
    public static PlayerInput Instance()
    {
        if (instance == null)
        {
            instance = new PlayerInput();
        }
        return instance;
    }

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
            { "x", KeyInput.DebugDie }
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
            { KeyInput.DebugDie, false },
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

        if (input.IsHeld(KeyInput.DebugDie))
        {
            creature.health.Hurt(creature.maxHealth * 2);
        }

        creature.FlipX = input.IsHeld(KeyInput.Right) ? false : input.IsHeld(KeyInput.Left) ? true : creature.FlipX;

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

public class PlayerBehaviour : BaseCreatureBehaviour<PlayerState>
{
    [SerializeField] BarBehaviour chargeBar;
    [SerializeField] GameStateBehaviour gameState;
    [SerializeField] SpellSpawnBehaviour spellSpawn;

    [Space] [Header("Movement")]
    [SerializeField] Player self;

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

    // Unity

    private new void Start()
    {
        base.Start();
        self.Init(creature, fsm, chargeBar, spellSpawn, PlayerInput.Instance(), gameState.gameState);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        self.FixedUpdate();
    }

    private void Update()
    {
        PlayerInput.Instance().Latch();
    }

    private void OnTriggerEnter2D()
    {
        self.OnTriggerEnter2D();
    }

    public bool Alive()
    {
        return self.Alive();
    }

    public Vector2 HeadPosition => self.HeadPosition;

    protected override void AddFsmStates()
    {
        fsm.Add(PlayerState.Angry, AngrySprite, YellClip);
        fsm.Add(PlayerState.Hurt, HurtSprite, HurtClip);
        fsm.Add(PlayerState.Casting, CastingSprite, CastClip);
        fsm.Add(PlayerState.Dead, DeadSprite, null);
        fsm.Add(PlayerState.Flying, FlyingSprite, null);
        fsm.Add(PlayerState.Standing, StandingSprite, null);
        fsm.Add(PlayerState.Still, FlyingSprite, null);
        fsm.Add(PlayerState.Charging, ChargingSprite, ChargeClip);
    }
}
