public interface ICollisionSystemParticipator : ISystemParticipator, ICollisionSystemParticipatorWrapper
{
    void CollidedWith(ICollisionSystemParticipator other);
    void ExitedCollisionWith(ICollisionSystemParticipator other);
    void TriggeredWith(ICollisionSystemParticipator other);
    void ExitedTriggerWith(ICollisionSystemParticipator other);
}
