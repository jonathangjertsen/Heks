using System;
using UnityEngine;

public interface ICreaturePhysics : IBasePhysics
{
    void GetUpright(float torque);
    void ApproachVelocity(Vector2 target);
    void ApproachVelocity(bool updateX, bool updateY, Vector2 target);
    void ApproachAngularVelocity(Vector2 target);
    void ApproachAngle(Vector2 diff);
    void LookAt(Vector2 transform);
    void Jump(float force);
    void Recoil(float torque);
    bool IsIdle();
}

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

public class CreaturePhysics : BasePhysics, ICreaturePhysics
{
    private readonly Vector2 axCoeff;
    private readonly float rotCoeff;
    private readonly Vector2 maxJerk;
    private readonly float idleThreshold;

    public CreaturePhysics(IRigidBody2d rigidBody2d, ITransform transform, CreaturePhysicsProperties properties) : base(rigidBody2d, transform)
    {
        axCoeff = new Vector2(properties.axCoeffX, properties.axCoeffY);
        rotCoeff = properties.rotCoeff;
        maxJerk = new Vector2(properties.maxJerkX, properties.maxJerkY);
        idleThreshold = properties.idleThreshold;
        FlipX = false;
    }

    public void ApproachVelocity(bool updateX, bool updateY, Vector2 target)
    {
        float axX = 0;
        float axY = 0;

        if (updateX)
        {
            axX = Math.Min(axCoeff.x * (target.x - rigidBody2d.Velocity.x), maxJerk.x);
        }

        if (updateY)
        {
            axY = Math.Min(axCoeff.y * (target.y - rigidBody2d.Velocity.y), maxJerk.y);
        }

        if (updateX || updateY)
        {
            Accelerate(new Vector2(axX, axY));
        }
    }

    public void ApproachVelocity(Vector2 target)
    {
        ApproachVelocity(true, true, target);
    }

    public void ApproachAngularVelocity(Vector2 target)
    {
        float targetRotation = (float)System.Math.Atan2(target.y, target.x);
        float currentRotation = rigidBody2d.Rotation;
        rigidBody2d.AngularVelocity += rotCoeff * (targetRotation - currentRotation);
    }

    public void ApproachAngle(Vector2 diff)
    {
        float targetRotation = (float)System.Math.Atan2(diff.y, diff.x);
        float currentRotation = rigidBody2d.Rotation;
        rigidBody2d.Rotation += rotCoeff * (targetRotation - currentRotation);
    }

    public void LookAt(Vector2 position)
    {
        Vector3 position3 = position;
        if (FlipX)
        {
            this.transform.Right = position3 - this.transform.Position;
        }
        else
        {
            this.transform.Right = this.transform.Position - position3;
        }
    }

    public void Jump(float force)
    {
        Accelerate(new Vector2(0, force));
    }

    public void Recoil(float torque)
    {
        Torque(torque);
    }

    public bool IsIdle()
    {
        return rigidBody2d.Velocity.magnitude < idleThreshold;
    }

    public void GetUpright(float torque)
    {
        float dir = AngleDegrees() < 180 ? -1f : +1f;
        rigidBody2d.AngularVelocity += torque * dir;
    }
}
