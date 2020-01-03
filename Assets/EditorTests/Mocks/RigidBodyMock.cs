using UnityEngine;

namespace Tests
{
    public class RigidBodyMock : IRigidBody2d
    {
        public float AngularVelocity { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
    }

}
