using UnityEngine;

public class PlayerBehaviour : BaseCreatureBehaviour<PlayerState>
{
    [SerializeField] BarBehaviour chargeBar;
    [SerializeField] GameStateBehaviour gameState;
    [SerializeField] SpellSpawnerBehaviour spellSpawn;

    [Space] [Header("Movement")]
    public Player self;

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

    private void FixedUpdate() => self.FixedUpdate();

    private void Update()
    {
        PlayerInput.Instance().Latch();
    }

    private void OnTriggerEnter2D()
    {
        self.OnTriggerEnter2D();
    }

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
