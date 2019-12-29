using UnityEngine;
using System;

public class CreaturePhysics : BasePhysics
{
    readonly Vector2 axCoeff;
    readonly float rotCoeff;
    readonly Vector2 maxJerk;
    readonly float idleThreshold;

    public CreaturePhysics(
        MonoBehaviour bh,
        Vector2 axCoeff,
        Vector2 maxJerk,
        float rotCoeff = 1f,
        float idleThreshold = 1f
    ) : base(bh)
    {
        this.axCoeff = axCoeff;
        this.rotCoeff = rotCoeff;
        this.maxJerk = maxJerk;
        this.idleThreshold = idleThreshold;
        FlipX = false;
    }

    public void ApproachVelocity(bool updateX, bool updateY, Vector2 target)
    {
        float axX = 0;
        float axY = 0;

        if (updateX)
        {
            axX = Math.Min(axCoeff.x * (target.x - rigidBody2d.velocity.x), maxJerk.x);
        }

        if (updateY)
        {
            axY = Math.Min(axCoeff.y * (target.y - rigidBody2d.velocity.y), maxJerk.y);
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
        float currentRotation = rigidBody2d.rotation;
        rigidBody2d.angularVelocity += rotCoeff * (targetRotation - currentRotation);
    }

    public void ApproachAngle(Vector2 diff)
    {
        float targetRotation = (float)System.Math.Atan2(diff.y, diff.x);
        float currentRotation = rigidBody2d.rotation;
        rigidBody2d.rotation += rotCoeff * (targetRotation - currentRotation);
    }

    public void LookAt(Transform transform)
    {
        if (FlipX)
        {
            this.transform.right = transform.position - this.transform.position;
        }
        else
        {
            this.transform.right = this.transform.position - transform.position;
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
        return rigidBody2d.velocity.magnitude < idleThreshold;
    }
}
