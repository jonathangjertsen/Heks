public class Ground : SystemParticipator, ICollisionSystemParticipator, IGround
{
    public ICollisionSystemParticipator GetCollisionSystemParticipator() => this;

    public void CollidedWith(ICollisionSystemParticipator other)
    {
    }

    public void ExitedCollisionWith(ICollisionSystemParticipator other)
    {
    }

    public void TriggeredWith(ICollisionSystemParticipator other)
    {
    }

    public void ExitedTriggerWith(ICollisionSystemParticipator other)
    {
    }
}
