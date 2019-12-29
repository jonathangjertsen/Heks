using UnityEngine;
using System;

public class CreaturePhysics : IFlipX
{
    private readonly Rigidbody2D rigidBody2d;
    private readonly Transform transform;
    readonly Vector2 axCoeff;
    readonly float rotCoeff;
    readonly Vector2 maxVelocity;
    readonly Vector2 maxJerk;
    readonly Vector3 initialScale;
    readonly float idleThreshold;

    private bool flipX;
    public bool FlipX { get => flipX; set
        {
            flipX = value;
            if (value)
            {
                transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            }
            else
            {
                transform.localScale = initialScale;
            }
        }
    }

    public CreaturePhysics(
        MonoBehaviour bh,
        Vector2 axCoeff,
        Vector2 maxVelocity,
        Vector2 maxJerk,
        float rotCoeff = 1f,
        float idleThreshold = 1f
    )
    {
        rigidBody2d = bh.GetComponent<Rigidbody2D>();
        transform = bh.transform;
        initialScale = transform.localScale;
        this.axCoeff = axCoeff;
        this.rotCoeff = rotCoeff;
        this.maxVelocity = maxVelocity;
        this.maxJerk = maxJerk;
        this.idleThreshold = idleThreshold;
        FlipX = false;
    }

    public Vector3 InitialSize
    {
        get => initialScale;
    }

    public Vector3 Size
    {
        get => transform.localScale;
        set => transform.localScale = value;
    }

    public void Accelerate(Vector2 ax)
    {
        rigidBody2d.velocity += ax;
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

    public Vector2 Position()
    {
        return rigidBody2d.transform.position;
    }

    public void Recoil(float torque)
    {
        if (FlipX)
        {
            rigidBody2d.angularVelocity += torque;
        }
        else
        {
            rigidBody2d.angularVelocity -= torque;
        }
    }

    public bool IsIdle()
    {
        return rigidBody2d.velocity.magnitude < idleThreshold;
    }
}
