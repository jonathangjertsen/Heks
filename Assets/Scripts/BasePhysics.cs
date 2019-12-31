using UnityEngine;

public interface IBasePhysics : IFlipX
{
    void Accelerate(Vector2 ax);
    void AccelerateRelative(Vector2 target);
    Vector3 Size { get; set; }
    Vector3 InitialSize { get; }
    void Torque(float torque);
    Vector2 Position();
    Vector2 Velocity();
    float AngleDegrees();
    float Angle();
    Vector2 UnitVector();
}

public interface ITransform
{
    Vector3 eulerAngles { get; set; }
    Vector3 localScale { get; set; }
    Vector3 position { get; set; }
    Vector3 right { get; set; }
}

public class TransformWrapper : ITransform
{
    private Transform t;

    public TransformWrapper(Transform t)
    {
        this.t = t;
    }

    public Vector3 eulerAngles { get => t.eulerAngles; set => t.eulerAngles = value; }
    public Vector3 localScale { get => t.localScale; set => t.localScale = value; }
    public Vector3 position { get => t.position; set => t.position = value; }
    public Vector3 right { get => t.right; set => t.right = value; }
}

public interface IRigidBody2d
{
    float angularVelocity { get; set; }
    float rotation { get; set; }
    Vector2 velocity { get; set; }
}

public class RigidBody2dWrapper : IRigidBody2d
{
    private Rigidbody2D r;

    public RigidBody2dWrapper(Rigidbody2D r)
    {
        this.r = r;
    }

    public float angularVelocity { get => r.angularVelocity; set => r.angularVelocity = value; }
    public float rotation { get => r.rotation; set => r.rotation = value; }
    public Vector2 velocity { get => r.velocity; set => r.velocity = value; }
}

public class BasePhysics : IFlipX, IBasePhysics
{
    private readonly Vector3 initialScale;
    protected readonly ITransform transform;
    protected readonly IRigidBody2d rigidBody2d;

    public BasePhysics(IRigidBody2d rigidBody2d, ITransform transform)
    {
        this.rigidBody2d = rigidBody2d;
        this.transform = transform;
        initialScale = this.transform.localScale;
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
                transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            }
            else
            {
                transform.localScale = initialScale;
            }
        }
    }

    public Vector3 InitialSize => initialScale;

    public Vector3 Size
    {
        get => transform.localScale;
        set => transform.localScale = value;
    }

    public void Accelerate(Vector2 ax)
    {
        rigidBody2d.velocity += ax;
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
            rigidBody2d.angularVelocity += torque;
        }
        else
        {
            rigidBody2d.angularVelocity -= torque;
        }
    }

    public Vector2 Position()
    {
        return transform.position;
    }

    public void Translate(Vector2 offset)
    {
        transform.position += new Vector3(offset.x, offset.y, 0);
    }

    public Vector2 Velocity()
    {
        return rigidBody2d.velocity;
    }

    public float AngleDegrees()
    {
        return transform.eulerAngles.z;
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
