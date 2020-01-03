using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BirdTest
    {
        private PlayerLocatorMock playerLocator;
        private ICreatureFsm<BirdState> fsm;
        private BaseCreatureWithTestPoints creature;
        private Bird bird;

        [SetUp]
        public void SetUp()
        {
            playerLocator = new PlayerLocatorMock();
            fsm = new CreatureFsmMock<BirdState>();
            creature = CreatureTest.GetCreature();
            bird = new Bird();
            bird.Init(creature, fsm, playerLocator);
        }

        [Test]
        public void ConstructorDoesNotCrash()
        {
        }

        [Test]
        public void InitialStateIsMoveHome()
        {
            Assert.AreEqual(BirdState.MoveHome, bird.fsm.State);
        }

        [Test]
        public void FixedUpdateNoPlayer_MoveHome()
        {
            bird.FixedUpdate();
            Assert.AreEqual(BirdState.MoveHome, bird.fsm.State);
        }

        [Test]
        public void FixedUpdateClosePlayer_MoveToPlayer()
        {
            PutPlayerClose();

            bird.FixedUpdate();

            Assert.AreEqual(BirdState.MoveToPlayer, bird.fsm.State);
        }

        [Test]
        public void FixedUpdateCloseDeadPlayer_MoveHome()
        {
            PutPlayerClose();
            playerLocator.isAlive = false;

            bird.FixedUpdate();

            Assert.AreEqual(BirdState.MoveHome, bird.fsm.State);
        }

        [Test]
        public void FixedUpdateFarFromPlayer_MoveHome()
        {
            PutPlayerFar();

            bird.FixedUpdate();

            Assert.AreEqual(BirdState.MoveHome, bird.fsm.State);
        }

        [Test]
        public void HurtCreature_BirdIsHurt()
        {
            creature.Hurt(10);
            Assert.AreEqual(BirdState.Hurt, bird.fsm.State);
        }

        [Test]
        public void HurtCreature_BirdIsHurtUntilTimerExpires()
        {
            PutPlayerClose();
            creature.Hurt(10);
            creature.timers.PropagateStartAndStop();
            Assert.True(creature.timers.Running("hurt"));
            while(creature.timers.Running("hurt"))
            {
                Assert.AreEqual(BirdState.Hurt, bird.fsm.State);
                bird.FixedUpdate();
            }
            bird.FixedUpdate();

            Assert.AreEqual(BirdState.MoveToPlayer, bird.fsm.State);
        }

        private void PutPlayerClose()
        {
            playerLocator.HeadPosition = new Vector2(0, 0);
            playerLocator.isAlive = true;
        }

        private void PutPlayerFar()
        {
            playerLocator.HeadPosition = new Vector2(10000, 10000);
            playerLocator.isAlive = true;
        }
    }
}
