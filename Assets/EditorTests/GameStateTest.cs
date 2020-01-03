using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameStateTest
    {
        private GameState gameState;
        private PauseMenuMock pauseMenu;

        [SetUp]
        public void SetUp()
        {
            gameState = new GameState();
            pauseMenu = new PauseMenuMock();
            pauseMenu.isActive = true;
            var fadeIn = new FadeInMock();
            var sceneLoader = new SceneLoaderMock();
            var chargeEffect = new ChargeEffectMock();
            gameState.Init(fadeIn, pauseMenu, chargeEffect, sceneLoader);
        }

        [Test]
        public void InitDoesNotCrash()
        {
        }

        [Test]
        public void PauseUnpause()
        {
            AssertIsUnpaused();

            gameState.Paused();
            AssertIsPaused();

            gameState.Unpaused();
            AssertIsUnpaused();
        }

        private void AssertIsPaused()
        {
            Assert.True(pauseMenu.isActive);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.timeScale, 0f);
        }

        private void AssertIsUnpaused()
        {
            Assert.False(pauseMenu.isActive);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.timeScale, 1f);
        }
    }
}
