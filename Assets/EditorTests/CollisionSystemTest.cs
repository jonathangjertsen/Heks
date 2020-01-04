using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CollisionSystemTest
    {
        [Test]
        public void CollisionSystemTestWithNoSystems()
        {
            var first = new CollisionSystemParticipatorMock();
            var second = new CollisionSystemParticipatorMock();
            CollisionSystem.RegisterCollision(first, second);
        }

        [Test]
        public void CollisionSystemTestWithTakesDamageButNoDealsDamage()
        {
            var first = new TakesDamageMock();
            var second = new CollisionSystemParticipatorMock();
            CollisionSystem.RegisterCollision(first, second);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, first.lastDamageTaken);
        }

        [Test]
        public void CollisionSystemTestWithDealsDamageButNoTakesDamage()
        {
            var first = new CollisionSystemParticipatorMock();
            var second = new DealsDamageMock();
            CollisionSystem.RegisterCollision(first, second);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, second.lastDamageGiven);
        }

        [Test]
        public void CollisionSystemTestWithDealsDamageAndTakesDamage()
        {
            var first = new TakesDamageMock();
            var second = new DealsDamageMock();

            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, first.lastDamageTaken);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, second.lastDamageGiven);

            CollisionSystem.RegisterCollision(first, second);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(1f, first.lastDamageTaken);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(1f, second.lastDamageGiven);
        }

        [Test]
        public void CollisionSystemTestWithDealsDamageAndTakesDamage_NoDoubleCounting()
        {
            var first = new DealsDamageMock();
            var second = new TakesDamageMock();

            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, first.lastDamageGiven);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, second.lastDamageTaken);

            CollisionSystem.RegisterCollision(first, second);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, first.lastDamageGiven);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(-1f, second.lastDamageTaken);
        }

        [Test]
        public void CollisionSystemTestWithTakesStatusEffectButNoDealsStatusEffect()
        {
            var first = new TakesStatusEffectMock();
            var second = new CollisionSystemParticipatorMock();
            CollisionSystem.RegisterCollision(first, second);
            Assert.IsNull(first.lastEffectTaken);
        }

        [Test]
        public void CollisionSystemTestWithDealsStatusEffectButNoTakesStatusEffect()
        {
            var first = new CollisionSystemParticipatorMock();
            var second = new DealsStatusEffectMock();
            CollisionSystem.RegisterCollision(first, second);

            Assert.IsNull(second.lastReceiver);
            Assert.IsNull(second.effectToDeal);
        }

        [Test]
        public void CollisionSystemTestWithDealsStatusEffectAndTakesStatusEffect()
        {
            var first = new TakesStatusEffectMock();
            var second = new DealsStatusEffectMock
            {
                effectToDeal = new StatusEffect(StatusEffectType.Burn, 100f)
            };  

            CollisionSystem.RegisterCollision(first, second);

            Assert.AreEqual(first.lastEffectTaken.Type, second.effectToDeal.Type);
            Assert.AreEqual(first.lastEffectTaken.Intensity, second.effectToDeal.Intensity);
        }

        [Test]
        public void CollisionSystemTestWithDealsStatusEffectAndTakesStatusEffect_NoDoubleCounting()
        {
            var first = new DealsStatusEffectMock();
            var second = new TakesStatusEffectMock();
            Assert.IsNull(second.lastEffectTaken);
            Assert.IsNull(first.lastReceiver);
            Assert.IsNull(first.effectToDeal);

            CollisionSystem.RegisterCollision(first, second);
            Assert.IsNull(second.lastEffectTaken);
            Assert.IsNull(first.lastReceiver);
            Assert.IsNull(first.effectToDeal);
        }
    }
}
