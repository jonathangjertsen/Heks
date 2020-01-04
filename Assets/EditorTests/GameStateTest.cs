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
        private FadeInMock fadeIn;
        private SceneLoaderMock sceneLoader;
        private ChargeEffectMock chargeEffect;
        private int deathToGameOverStartTop = 3;

        [SetUp]
        public void SetUp()
        {
            gameState = new GameState
            {
                deathToGameOverStartTop = deathToGameOverStartTop
            };

            pauseMenu = new PauseMenuMock
            {
                isActive = true
            };
            fadeIn = new FadeInMock();
            sceneLoader = new SceneLoaderMock();
            chargeEffect = new ChargeEffectMock()
            {
                isActive = false
            };
            gameState.Init(fadeIn, pauseMenu, chargeEffect, sceneLoader);

            Assert.False(sceneLoader.didReload);
        }

        [Test]
        public void InitDoesNotCrash()
        {
        }

        [Test]
        public void PauseUnpause()
        {
            AssertIsUnpaused(false);

            gameState.Paused();
            AssertIsPaused();

            gameState.Unpaused();
            AssertIsUnpaused(false);
        }

        [Test]
        public void Restart_NotPaused()
        {
            gameState.LevelRestarted();
            AssertIsUnpaused(false);
            Assert.True(sceneLoader.didReload);
        }

        [Test]
        public void RestartWhilePaused_NotPaused()
        {
            gameState.Paused();
            gameState.LevelRestarted();
            AssertIsUnpaused(false);
            Assert.True(sceneLoader.didReload);
        }

        [Test]
        public void Exit_NotPaused()
        {
            gameState.LevelExited();
            AssertIsUnpaused(false);
        }

        [Test]
        public void ExitWhilePaused_NotPaused()
        {
            gameState.Paused();
            gameState.LevelExited();
            AssertIsUnpaused(false);
        }

        [Test]
        public void PlayerDied()
        {
            gameState.PlayerDied();
            ClockThroughFadeDelay();
        }

        [Test]
        public void PlayerDiedThenRestarted()
        {
            gameState.PlayerDied();
            ClockThroughFadeDelay();
            gameState.LevelRestarted();

            Assert.True(sceneLoader.didReload);
        }

        [Test]
        public void PlayerPausedThenRestarted()
        {
            gameState.Paused();
            gameState.LevelRestarted();

            AssertIsUnpaused(false);
            Assert.True(sceneLoader.didReload);
        }

        [Test]
        public void ChargeEffectSlowdown()
        {
            gameState.ChargeStart();
            AssertIsUnpaused(true);
            gameState.ChargeStop();
            AssertIsUnpaused(false);
        }

        [Test]
        public void ChargeEffectAndPauseInteractions()
        {
            gameState.ChargeStart();
            AssertIsUnpaused(true);
            gameState.Paused();
            AssertIsPaused();
            gameState.ChargeStop();
            AssertIsPaused();
            gameState.Unpaused();
            AssertIsUnpaused(false);
            gameState.ChargeStart();
            AssertIsUnpaused(true);
            gameState.Paused();
            AssertIsPaused();
            gameState.Unpaused();
            AssertIsUnpaused(true);
            gameState.ChargeStop();
            AssertIsUnpaused(false);
        }

        [Test]
        public void ChargeEffectAndRestartInteractions()
        {
            gameState.ChargeStart();
            AssertIsUnpaused(true);
            gameState.LevelRestarted();
            AssertIsUnpaused(false);
        }

        [Test]
        public void ChargeEffectAndExitInteractions()
        {
            gameState.ChargeStart();
            AssertIsUnpaused(true);
            gameState.LevelExited();
            AssertIsUnpaused(false);
        }

        private void ClockThroughFadeDelay()
        {
            gameState.FixedUpdate();
            for (int i = 0; i < deathToGameOverStartTop; i++)
            {
                Assert.False(fadeIn.fadeStarted);
                gameState.FixedUpdate();
            }
            Assert.True(fadeIn.fadeStarted);
        }

        private void AssertIsPaused()
        {
            Assert.True(pauseMenu.isActive);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.timeScale, 0f);
        }

        private void AssertIsUnpaused(bool charging)
        {
            Assert.False(pauseMenu.isActive);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.timeScale, charging ? gameState.chargeSlowdown : 1f);
        }
    }
}
