using UnityEngine;

public class BirdBehaviour : BaseCreatureBehaviour<BirdState>
{
    public PlayerLocatorBehaviour playerLocator;

    public Bird bird;

    [Space] [Header("Audio clips")]
    public AudioClip CryClip;

    [Space] [Header("Sprites")]
    [SerializeField] Sprite DefaultSprite;
    [SerializeField] Sprite ChargingSprite;
    [SerializeField] Sprite HurtSprite;
    [SerializeField] Sprite DeadSprite;

    private new void Start()
    {
        base.Start();

        NotNull.Check(playerLocator);
        NotNull.Check(CryClip);
        NotNull.Check(DefaultSprite);
        NotNull.Check(ChargingSprite);
        NotNull.Check(HurtSprite);
        NotNull.Check(DeadSprite);

        bird.Init(creature, fsm, playerLocator);
    }

    protected override void AddFsmStates()
    {
        fsm.Add(BirdState.MoveHome, DefaultSprite, null);
        fsm.Add(BirdState.MoveToPlayer, ChargingSprite, CryClip);
        fsm.Add(BirdState.Hurt, HurtSprite, CryClip);
        fsm.Add(BirdState.Dead, DeadSprite, null);
    }

    public override ICreatureController GetCreatureController() => bird;
}
