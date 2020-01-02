using UnityEngine;

public class BirdBehaviour : BaseCreatureBehaviour<BirdState>
{
    public PlayerBehaviour playerLocator;

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
        bird.Init(creature, fsm, playerLocator);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        bird.FixedUpdate();
    }

    private void OnCollisionEnter2D()
    {
        creature.Hurt(10, 100);
        bird.Hurt(10);
    }

    protected override void AddFsmStates()
    {
        fsm.Add(BirdState.MoveHome, DefaultSprite, null);
        fsm.Add(BirdState.MoveToPlayer, ChargingSprite, CryClip);
        fsm.Add(BirdState.Hurt, HurtSprite, CryClip);
        fsm.Add(BirdState.Dead, DeadSprite, null);
    }
}
