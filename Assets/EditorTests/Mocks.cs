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
        public float Health { get; private set; }

        public void Heal(float amount)
        {
            Health += amount;
        }

        public void Hurt(float amount)
        {
            Health -= amount;
        }
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
        public bool FlipX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Cast(Vector2 initialVelocity, float charge)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerInputMock : IPlayerInput
    {
        public bool IsAnyHeld()
        {
            throw new NotImplementedException();
        }

        public bool IsHeld(KeyInput key)
        {
            throw new NotImplementedException();
        }

        public bool IsPressedThisFrame(KeyInput key)
        {
            throw new NotImplementedException();
        }

        public void Latch()
        {
            throw new NotImplementedException();
        }
    }

    public class EventBusMock : IEventBus
    {
        public void ChargeStart()
        {
            throw new NotImplementedException();
        }

        public void ChargeStop()
        {
            throw new NotImplementedException();
        }

        public void LevelRestarted()
        {
            throw new NotImplementedException();
        }

        public void Paused()
        {
            throw new NotImplementedException();
        }

        public void PlayerDied()
        {
            throw new NotImplementedException();
        }

        public void Unpaused()
        {
            throw new NotImplementedException();
        }
    }

    public class RigidBodyMock : IRigidBody2d
    {
        public float AngularVelocity { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
    }

    public class TransformMock : ITransform
    {
        public Vector3 EulerAngles { get; set; }
        public Vector3 LocalScale { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Right { get; set; }
    }

    public class BaseCreatureMock : BaseCreature
    {
        public bool mock_onDeathStartCalled = false;
        public bool mock_onDeathFinishedCalled = false;
        public bool mock_onHurtCompletedCalled = false;

        public void MockInit()
        {
            physicsProperties = new CreaturePhysicsProperties();
            Init(
                new RigidBodyMock(),
                new TransformMock(),
                new BarMock()
            );
            health.PrependZeroHealthCallback(() => mock_onDeathStartCalled = true);
            SetOnDeathFinishedCallback(() => mock_onDeathFinishedCalled = true);
            SetOnHurtFinishedCallback(() => mock_onHurtCompletedCalled = true);
        }
    }
}
