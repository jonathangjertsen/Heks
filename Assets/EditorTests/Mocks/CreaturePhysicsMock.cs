using UnityEngine;

namespace Tests
{
    class CreaturePhysicsMock : ICreaturePhysics
    {
        public Vector3 Size { get; set; }
        public Vector3 InitialSize => new Vector3(1, 1, 0);

        public bool FlipX { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Accelerate(Vector2 target)
        {
        }
        public void AccelerateRelative(Vector2 target)
        {
        }

        public float Angle()
        {
            throw new System.NotImplementedException();
        }

        public float AngleDegrees()
        {
            throw new System.NotImplementedException();
        }

        public void ApproachAngle(Vector2 diff)
        {
            throw new System.NotImplementedException();
        }

        public void ApproachAngularVelocity(Vector2 target)
        {
            throw new System.NotImplementedException();
        }

        public void ApproachVelocity(Vector2 target)
        {
            throw new System.NotImplementedException();
        }

        public void ApproachVelocity(bool updateX, bool updateY, Vector2 target)
        {
            throw new System.NotImplementedException();
        }

        public void GetUpright(float v)
        {
        }

        public bool IsIdle()
        {
            throw new System.NotImplementedException();
        }

        public void Jump(float force)
        {
            throw new System.NotImplementedException();
        }

        public void LookAt(Vector2 transform)
        {
            throw new System.NotImplementedException();
        }

        public Vector2 position = new Vector2(0, 0);
        public Vector2 Position()
        {
            return position;
        }

        public void Recoil(float torque)
        {
            throw new System.NotImplementedException();
        }

        public void Torque(float torque)
        {
            throw new System.NotImplementedException();
        }

        public Vector2 UnitVector()
        {
            throw new System.NotImplementedException();
        }

        public Vector2 Velocity()
        {
            throw new System.NotImplementedException();
        }
    }

}
