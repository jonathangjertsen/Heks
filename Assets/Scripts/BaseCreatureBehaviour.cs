using System;
using UnityEngine;

public abstract class BaseCreatureBehaviour<StateEnum> : MonoBehaviour where StateEnum : struct, Enum
{
    public BaseCreature creature;
    protected CreatureFsm<StateEnum> fsm;

    [Space]
    [Header("Debug")]
    public bool logFsmChanges = false;
    public bool logTimerCallbacks = false;

    [Space]
    [Header("Behaviour references")]
    public BarBehaviour healthBar;

    protected void Start()
    {
        creature.Init(
            new WrapperRigidbody2d(GetComponent<Rigidbody2D>()),
            new WrapperTransform(transform),
            healthBar
        );
        creature.SetOnDeathFinishedCallback(() => Destroy(this));
        creature.timers.logCallbacks = logTimerCallbacks;

        fsm = new CreatureFsm<StateEnum>(this)
        {
            logChanges = logFsmChanges
        };

        AddFsmStates();
    }

    protected void FixedUpdate()
    {
        creature.FixedUpdate();
    }

    protected virtual void AddFsmStates()
    {
    }
}
