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
