using UnityEngine;

public class BasePhysics : IFlipX, IBasePhysics
{
    private readonly Vector3 initialScale;
    protected readonly ITransform transform;
    protected readonly IRigidBody2d rigidBody2d;

    public BasePhysics(IRigidBody2d rigidBody2d, ITransform transform)
    {
        this.rigidBody2d = rigidBody2d;
        this.transform = transform;
        initialScale = this.transform.LocalScale;
    }

    private bool flipX;
    public bool FlipX
    {
        get => flipX;
        set
        {
            flipX = value;
            if (value)
            {
                transform.LocalScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            }
            else
            {
                transform.LocalScale = initialScale;
            }
        }
    }

    public Vector3 InitialSize => initialScale;

    public Vector3 Size
    {
        get => transform.LocalScale;
        set => transform.LocalScale = value;
    }

    public void Accelerate(Vector2 ax)
    {
        rigidBody2d.Velocity += ax;
    }

    public void AccelerateRelative(Vector2 relativeAx)
    {
        Vector2 unit = UnitVector();
        Vector2 absoluteAx = new Vector2(
            (unit.x * relativeAx.x) - (unit.y * relativeAx.y),
            (unit.y * relativeAx.x) - (unit.x * relativeAx.y)
        );
        Accelerate(absoluteAx);
    }

    public void Torque(float torque)
    {
        if (FlipX)
        {
            rigidBody2d.AngularVelocity += torque;
        }
        else
        {
            rigidBody2d.AngularVelocity -= torque;
        }
    }

    public Vector2 Position()
    {
        return transform.Position;
    }

    public void Translate(Vector2 offset)
    {
        transform.Position += new Vector3(offset.x, offset.y, 0);
    }

    public Vector2 Velocity()
    {
        return rigidBody2d.Velocity;
    }

    public float AngleDegrees()
    {
        return transform.EulerAngles.z;
    }

    public float Angle()
    {
        return AngleDegrees() * Mathf.PI/ 180;
    }

    public Vector2 UnitVector()
    {
        float angle = Angle();
        return new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        );
    }
}
