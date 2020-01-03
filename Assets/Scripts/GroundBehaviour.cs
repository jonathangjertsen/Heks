using UnityEngine;

public interface IGround
{
}

public class Ground : SysParticipator, ISysCollisionParticipator, IGround
{
    public ISysCollisionParticipator GetSysCollisionParticipator() => this;

    public void CollidedWith(ISysCollisionParticipator other)
    {
    }

    public void ExitedCollisionWith(ISysCollisionParticipator other)
    {
    }

    public void TriggeredWith(ISysCollisionParticipator other)
    {
    }

    public void ExitedTriggerWith(ISysCollisionParticipator other)
    {
    }
}

public class GroundBehaviour : MonoBehaviour, ISysCollisionParticipatorWrapper
{
    private Ground ground;

    public ISysCollisionParticipator GetSysCollisionParticipator() => ground;

    void Start()
    {
        ground = new Ground();    
    }
}
