using UnityEngine;

public class FlameBehaviour : BaseCollisionSystemParticipatorWrapper
{
    public Flame flame;

    private void Start()
    {
    }

    public override ICollisionSystemParticipator GetCollisionSystemParticipator() => flame;
}
