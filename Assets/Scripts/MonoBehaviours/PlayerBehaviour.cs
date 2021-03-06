﻿using UnityEngine;

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
    [SerializeField] AudioClip PlungeClip;

    [Space] [Header("Sprites")]
    [SerializeField] Sprite FlyingSprite;
    [SerializeField] Sprite StandingSprite;
    [SerializeField] Sprite ChargingSprite;
    [SerializeField] Sprite CastingSprite;
    [SerializeField] Sprite HurtSprite;
    [SerializeField] Sprite AngrySprite;
    [SerializeField] Sprite DeadSprite;
    [SerializeField] Sprite PlungingSprite;

    // Unity
    private new void Start()
    {
        base.Start();

        NotNull.Check(chargeBar);
        NotNull.Check(gameState);
        NotNull.Check(spellSpawn);
        NotNull.Check(YellClip);
        NotNull.Check(HurtClip);
        NotNull.Check(ChargeClip);
        NotNull.Check(CastClip);
        NotNull.Check(PlungeClip);
        NotNull.Check(FlyingSprite);
        NotNull.Check(StandingSprite);
        NotNull.Check(ChargingSprite);
        NotNull.Check(CastingSprite);
        NotNull.Check(HurtSprite);
        NotNull.Check(AngrySprite);
        NotNull.Check(DeadSprite);
        NotNull.Check(PlungingSprite);

        self.Init(creature, fsm, chargeBar, spellSpawn, PlayerInput.Instance(), gameState.gameState);
    }

    private void Update()
    {
        PlayerInput.Instance().Latch();
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
        fsm.Add(PlayerState.Plunging, PlungingSprite, PlungeClip);
    }

    public override ICreatureController GetCreatureController() => self;
}
