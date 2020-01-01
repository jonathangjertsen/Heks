using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerTest
    {
        [Test]
        public void Player_ConstructorDoesNotCrash()
        {
            var player = GetPlayer();
        }

        [Test]
        public void Player_CallDieThenAliveReturnsFalse()
        {
            var player = GetPlayer();
            Assert.True(player.Alive());
            player.Die();
            Assert.False(player.Alive());
        }

        private Player GetPlayer()
        {
            Player player = new Player();
            BaseCreatureMock creature = new BaseCreatureMock();
            creature.MockInit();
            player.Init(
                creature,
                new CreatureFsmMock<PlayerState>(),
                new BarMock(),
                new SpellCasterMock(),
                new PlayerInputMock()
            );
            return player;
        }
    }
}
