using UnityEngine;

public class GroundBehaviour : MonoBehaviour, ICollisionSystemParticipatorWrapper
{
    private Ground ground;

    public ICollisionSystemParticipator GetCollisionSystemParticipator() => ground;

    void Start()
    {
        ground = new Ground();    
    }
}
