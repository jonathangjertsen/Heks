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
            gameState.Init(fadeIn, pauseMenu, sceneLoader);

            Assert.False(sceneLoader.didReload);
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

        [Test]
        public void Restart_NotPaused()
        {
            gameState.LevelRestarted();
            AssertIsUnpaused();
            Assert.True(sceneLoader.didReload);
        }

        [Test]
        public void RestartWhilePaused_NotPaused()
        {
            gameState.Paused();
            gameState.LevelRestarted();
            AssertIsUnpaused();
            Assert.True(sceneLoader.didReload);
        }

        [Test]
        public void Exit_NotPaused()
        {
            gameState.LevelExited();
            AssertIsUnpaused();
        }

        [Test]
        public void ExitWhilePaused_NotPaused()
        {
            gameState.Paused();
            gameState.LevelExited();
            AssertIsUnpaused();
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

            AssertIsUnpaused();
            Assert.True(sceneLoader.didReload);
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

        private void AssertIsUnpaused()
        {
            Assert.False(pauseMenu.isActive);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.timeScale, 1f);
        }
    }
}
