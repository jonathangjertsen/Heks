using UnityEngine;

public class SkullBehaviour : BaseCreatureBehaviour<SkullState>
{
    public PlayerLocatorBehaviour playerLocator;

    public Skull skull;

    [Space] [Header("Sprites")]
    public Sprite GroundedSprite;
    public Sprite InAirSprite;
    public Sprite DeadSprite;
    public Sprite HurtSprite;

    private new void Start()
    {
        base.Start();

        if (playerLocator == null)
        {
            throw new System.Exception("No PlayerLocator registered");
        }

        skull.Init(creature, fsm, playerLocator);
    }

    protected override void AddFsmStates()
    {
        fsm.Add(SkullState.GroundedCanHop, GroundedSprite, null);
        fsm.Add(SkullState.GroundedWaiting, GroundedSprite, null);
        fsm.Add(SkullState.InAir, InAirSprite, null);
        fsm.Add(SkullState.Dead, DeadSprite, null);
        fsm.Add(SkullState.Hurt, HurtSprite, null);
    }

    public override ICreatureController GetCreatureController() => skull;
}
