public abstract class BaseCollisionSystemParticipator : SystemParticipator, ICollisionSystemParticipator
{
    public void CollidedWith(ICollisionSystemParticipator other) {}
    public virtual void ExitedCollisionWith(ICollisionSystemParticipator other){}
    public virtual void ExitedTriggerWith(ICollisionSystemParticipator other) {}
    public virtual void TriggeredWith(ICollisionSystemParticipator other) {}

    public ICollisionSystemParticipator GetCollisionSystemParticipator() => this;
}
