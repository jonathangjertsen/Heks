using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BirdTest
    {
        [Test]
        public void ConstructorDoesNotCrash()
        {
            var bird = GetBird();
        }

        [Test]
        public void CallDieThenAliveReturnsFalse()
        {
            var bird = GetBird();
            bird.Die();
            Assert.False(bird.Alive());
        }

        private Bird GetBird()
        {
            Bird bird = new Bird();
            BaseCreatureMock creature = new BaseCreatureMock();
            creature.MockInit();
            bird.Init(creature, new CreatureFsmMock<BirdState>());
            return bird;
        }
    }
}
