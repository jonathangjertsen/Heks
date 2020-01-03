using UnityEngine;

public class OctopusBehaviour : BaseCreatureBehaviour<OctopusState>
{
    public Sprite sprite;
    public Octopus octopus;

    new private void Start()
    {
        base.Start();

        NotNull.Check(sprite);

        octopus.Init(creature, fsm);
    }

    protected override void AddFsmStates()
    {
        fsm.Add(OctopusState.Alive, sprite, null);
        fsm.Add(OctopusState.Dead, sprite, null);
    }

    public override ICreatureController GetCreatureController() => octopus;
}
