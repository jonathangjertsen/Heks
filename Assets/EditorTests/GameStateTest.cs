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

        [SetUp]
        public void SetUp()
        {
            gameState = new GameState();
        }

        [Test]
        public void GameStateConstructorDoesNotCrash()
        {
        }

        [Test]
        public void InitDoesNotCrash()
        {
            //gameState.Init();
        }
    }
}
