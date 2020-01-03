using UnityEngine;

public class WrapperRigidbody2d : IRigidBody2d
{
    private Rigidbody2D r;

    public WrapperRigidbody2d(Rigidbody2D r)
    {
        this.r = r;
    }

    public float AngularVelocity { get => r.angularVelocity; set => r.angularVelocity = value; }
    public float Rotation { get => r.rotation; set => r.rotation = value; }
    public Vector2 Velocity { get => r.velocity; set => r.velocity = value; }
}
