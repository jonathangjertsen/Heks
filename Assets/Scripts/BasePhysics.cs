using UnityEngine;

public class BasePhysics : IFlipX
{
    private readonly Vector3 initialScale;
    protected readonly Transform transform;
    protected readonly Rigidbody2D rigidBody2d;

    public BasePhysics(MonoBehaviour bh)
    {
        rigidBody2d = bh.GetComponent<Rigidbody2D>();
        transform = bh.transform;
        initialScale = transform.localScale;
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
        return rigidBody2d.transform.position;
    }

    public void Translate(Vector2 offset)
    {
        rigidBody2d.transform.position += new Vector3(offset.x, offset.y, 0);
    }

    public Vector2 Velocity()
    {
        return rigidBody2d.velocity;
    }
}
