using UnityEngine;

public class BirdBehaviour : BaseCreatureBehaviour<BirdState>
{
    public Transform playerTransform;

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
        bird.Init(creature, fsm);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        if (player != null)
        {
            bird.NewPlayerPosition(player.HeadPosition, player.Alive());
        }
    }

    private void OnCollisionEnter2D()
    {
        creature.Hurt(10, 100);
        bird.Hurt();
    }

    protected override void AddFsmStates()
    {
        fsm.Add(BirdState.MoveHome, DefaultSprite, null);
        fsm.Add(BirdState.MoveToPlayer, ChargingSprite, CryClip);
        fsm.Add(BirdState.Hurt, HurtSprite, CryClip);
        fsm.Add(BirdState.Dead, DeadSprite, null);
    }
}
