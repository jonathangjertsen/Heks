using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

        public void LookAt(Transform transform)
        {
            throw new System.NotImplementedException();
        }

        public Vector2 Position()
        {
            throw new System.NotImplementedException();
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

    class CreatureHealthMock : ICreatureHealth
    {
        public float Health { get; set; }
    }

    class CreatureFsmMock : ICreatureFsm<OctopusState>
    {
        public OctopusState State { get; set; }

        public void Add(OctopusState state, Sprite sprite, AudioClip clip)
        {
            throw new System.NotImplementedException();
        }
    }

    public class OctopusTest
    {
        [Test]
        public void Octopus_ConstructorDoesNotCrash()
        {
            GetOctopus();
        }

        [Test]
        public void Octopus_NextFrameDoesNotCrash()
        {
            Octopus octopus = GetOctopus();
            octopus.NextFrame();
        }

        private Octopus GetOctopus()
        {
            Octopus octopus = new Octopus();
            octopus.SetFsm(new CreatureFsmMock());
            octopus.SetHealth(new CreatureHealthMock());
            octopus.SetPhysics(new CreaturePhysicsMock());
            return octopus;
        }
    }
}
