﻿using UnityEngine;

public class SkullBehaviour : BaseCreatureBehaviour<SkullState>
{
    public Skull skull;
    public PlayerBehaviour playerLocator;

    [Space] [Header("Sprites")]
    public Sprite GroundedSprite;
    public Sprite InAirSprite;
    public Sprite DeadSprite;
    public Sprite HurtSprite;

    private new void Start()
    {
        base.Start();
        skull.Init(creature, fsm, playerLocator);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        skull.FixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        skull.OnCollisionEnter2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        skull.OnCollisionExit2D(collision);
    }

    protected override void AddFsmStates()
    {
        fsm.Add(SkullState.GroundedCanHop, GroundedSprite, null);
        fsm.Add(SkullState.GroundedWaiting, GroundedSprite, null);
        fsm.Add(SkullState.InAir, InAirSprite, null);
        fsm.Add(SkullState.Dead, DeadSprite, null);
        fsm.Add(SkullState.Hurt, HurtSprite, null);
    }
}
