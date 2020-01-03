public interface ISysCollisionParticipator : ISysParticipator, ISysCollisionParticipatorWrapper
{
    void CollidedWith(ISysCollisionParticipator other);
    void ExitedCollisionWith(ISysCollisionParticipator other);
    void TriggeredWith(ISysCollisionParticipator other);
    void ExitedTriggerWith(ISysCollisionParticipator other);
}
