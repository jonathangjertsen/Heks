using UnityEngine;

public class FlameBehaviour : BaseCollisionSystemParticipatorWrapper
{
    public Flame flame;

    private void Start()
    {
        flame.Init(transform.position);
    }

    public override ICollisionSystemParticipator GetCollisionSystemParticipator() => flame;
}
