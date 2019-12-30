using UnityEngine;

public enum OctpusState
{
    Alive,
    Dead
}

public class OctopusBehaviour : BaseCreatureBehaviour<OctpusState>
{
    public float freqX;
    public float freqY;
    public float ampX;
    public float ampY;

    public Sprite sprite;

    private int counter;

    new private void Start()
    {
        base.Start();

        fsm.Add(OctpusState.Alive, sprite, null);
        fsm.Add(OctpusState.Dead, sprite, null);
        FsmState = OctpusState.Alive;

        counter = 0;
    }

    public override void Die()
    {
        base.Die();
        FsmState = OctpusState.Dead;
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        counter += 1;

        Vector2 target = new Vector2(
            ampX * Mathf.Cos(freqX * counter),
            ampY * Mathf.Sin(freqY * counter)
        );

        physics.GetUpright(0.3f);
        physics.AccelerateRelative(target);
    }

    private void OnCollisionEnter2D()
    {
        if (FsmState != OctpusState.Dead)
        {
            health.Health -= 10;
        }
    }
}
