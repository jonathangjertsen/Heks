using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
    public class CreatureTest
    {
        private int deathToShrinkStartTimerTop = 2;
        private int shrinkTimerTop = 2;
        private int hurtTimerTop = 2;
        private float maxHealth = 100f;
        private float regenPer = 2f;

        [Test]
        public void InitializerDoesNotCrash()
        {
            GetCreature();
        }

        [Test]
        public void FlipXPersists()
        {
            var creature = GetCreature();
            creature.FlipX = true;
            Assert.IsTrue(creature.FlipX);
            creature.FlipX = false;
            Assert.IsFalse(creature.FlipX);
            creature.FlipX = true;
            Assert.IsTrue(creature.FlipX);
        }

        [Test]
        public void DeathSequence()
        {
            var creature = GetCreature();
            Assert.IsFalse(creature.timers.Running("deathToShrinkStart"));
            creature.Die();

            for (int i = 0; i < deathToShrinkStartTimerTop; i++)
            {
                creature.FixedUpdate();
                Assert.IsTrue(creature.timers.Running("deathToShrinkStart"));
                Assert.IsFalse(creature.timers.Running("shrink"));
            }

            for (int i = 0; i < shrinkTimerTop; i++)
            {
                creature.FixedUpdate();
                Assert.IsFalse(creature.timers.Running("deathToShrinkStart"));
                Assert.IsTrue(creature.timers.Running("shrink"));
            }

            creature.FixedUpdate();
            Assert.IsFalse(creature.timers.Running("shrink"));
        }

        [Test]
        public void HealthRegen()
        {
            var creature = GetCreature();
            Assert.AreApproximatelyEqual(creature.health.Health, creature.maxHealth);

            creature.health.Health = maxHealth / 2;
            for(int i = 0; i < 10; i++)
            {
                Assert.AreApproximatelyEqual(creature.health.Health, maxHealth / 2 + regenPer * i);
                creature.FixedUpdate();
            }
        }

        [Test]
        public void ZeroHealthCausesDeath()
        {
            var creature = GetCreature();

            Assert.IsFalse(creature.mock_onDeathStartCalled);
            creature.health.Health = 0;
            creature.FixedUpdate();
            Assert.IsTrue(creature.mock_onDeathStartCalled);
        }

        [Test]
        public void HurtTimerCompletes()
        {
            var creature = GetCreature();
            var hurtPer = 1f;

            Assert.IsFalse(creature.mock_onHurtCompletedCalled);

            creature.Hurt(hurtPer);
            for (int i = 0; i < hurtTimerTop; i++)
            {
                Assert.IsFalse(creature.mock_onHurtCompletedCalled);
                creature.FixedUpdate();
            }

            creature.FixedUpdate();
            Assert.IsTrue(creature.mock_onHurtCompletedCalled);
        }

        [Test]
        public void HurtAffectsHealth()
        {
            var creature = GetCreature();
            var hurtPer = 1f;

            Assert.IsFalse(creature.mock_onHurtCompletedCalled);
            creature.regenPer = 0f;
            for (int i = 0; i < 10; i++)
            {
                Assert.AreApproximatelyEqual(creature.health.Health, maxHealth - i * hurtPer);

                creature.Hurt(hurtPer);
                creature.FixedUpdate();
            }
        }

        [Test]
        public void HurtCausesDeath()
        {
            int nRounds = 10;

            var creature = GetCreature();
            var hurtPer = maxHealth / (nRounds + 1);

            Assert.IsFalse(creature.mock_onHurtCompletedCalled);
            creature.regenPer = 0f;
            for (int i = 0; i < nRounds; i++)
            {
                Assert.AreApproximatelyEqual(creature.health.Health, maxHealth - i * hurtPer, 0.01f);
                Assert.IsFalse(creature.mock_onDeathStartCalled);

                creature.Hurt(hurtPer);
                creature.FixedUpdate();
            }

            creature.Hurt(hurtPer);
            creature.FixedUpdate();
            Assert.IsTrue(creature.mock_onDeathStartCalled);
        }

        private BaseCreatureMock GetCreature()
        {
            BaseCreatureMock creature = new BaseCreatureMock
            {
                deathToShrinkStartTimerTop = deathToShrinkStartTimerTop,
                shrinkTimerTop = shrinkTimerTop,
                regenPer = regenPer,
                maxHealth = maxHealth,
                hurtTimerTop = hurtTimerTop
            };
            creature.MockInit();
            return creature;
        }
    }
}
