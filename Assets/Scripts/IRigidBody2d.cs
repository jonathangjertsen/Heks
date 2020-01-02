using UnityEngine;

public interface IRigidBody2d
{
    float AngularVelocity { get; set; }
    float Rotation { get; set; }
    Vector2 Velocity { get; set; }
}
