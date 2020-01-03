using UnityEngine;

public class OctopusBehaviour : BaseCreatureBehaviour<OctopusState>
{
    public Sprite sprite;
    public Octopus octopus;

    new private void Start()
    {
        base.Start();
        octopus.Init(creature, fsm);
    }

    private void FixedUpdate() => octopus.FixedUpdate();

    private void OnCollisionEnter2D()
    {
        octopus.OnCollisionEnter2D();
    }

    protected override void AddFsmStates()
    {
        fsm.Add(OctopusState.Alive, sprite, null);
        fsm.Add(OctopusState.Dead, sprite, null);
    }
}
