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
