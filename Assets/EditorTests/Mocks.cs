using System;
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

    class CreatureHealthMock : ICreatureHealth
    {
        public float Health { get; set; }
    }

    public class CreatureFsmMock<EnumType> : ICreatureFsm<EnumType> where EnumType : struct, Enum
    {
        public EnumType State { get; set; }

        public void Add(EnumType state, Sprite sprite, AudioClip clip)
        {
            throw new System.NotImplementedException();
        }

        public void SetSprite(EnumType state)
        {
            throw new System.NotImplementedException();
        }

        public void UnsetSprite(EnumType state)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BarMock : IBarDisplay
    {
        public bool FlipX { get; set; }
        public void Hide() { }
        public void FillTo(float proportion) { }
    }

    public class SpellCasterMock : ISpellCaster
    {
        public void Cast(Vector2 initialVelocity, float charge)
        {
            throw new NotImplementedException();
        }
    }

    public class RigidBodyMock : IRigidBody2d
    {
        public float angularVelocity { get; set; }
        public float rotation { get; set; }
        public Vector2 velocity { get; set; }
    }

    public class TransformMock : ITransform
    {
        public Vector3 eulerAngles { get; set; }
        public Vector3 localScale { get; set; }
        public Vector3 position { get; set; }
        public Vector3 right { get; set; }
    }

    public class BaseCreatureMock : BaseCreature
    {
        public bool mock_onDeathStartCalled = false;
        public bool mock_onDeathCompletedCalled = false;
        public bool mock_onHurtCompletedCalled = false;

        public void MockInit()
        {
            physicsProperties = new CreaturePhysicsProperties();
            Init(
                new RigidBodyMock(),
                new TransformMock(),
                new BarMock(),
                OnDeathCompleted,
                OnDeathStart,
                OnHurtCompleted
            );
        }

        public void OnDeathStart()
        {
            mock_onDeathStartCalled = true;
        }

        public void OnDeathCompleted()
        {
            mock_onDeathCompletedCalled = true;
        }

        public void OnHurtCompleted()
        {
            mock_onHurtCompletedCalled = true;
        }
    }
}
