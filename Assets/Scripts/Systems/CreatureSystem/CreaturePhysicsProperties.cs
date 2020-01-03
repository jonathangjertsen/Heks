using System;

[Serializable]
public class CreaturePhysicsProperties
{
    public float axCoeffX = 0.01f;
    public float axCoeffY = 0.03f;
    public float rotCoeff = 1f;
    public float maxJerkX = 5f;
    public float maxJerkY = 5f;
    public float idleThreshold = 1f;
}
