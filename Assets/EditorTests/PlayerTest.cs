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
            player.Die();
            Assert.False(player.Alive());
        }

        private Player GetPlayer()
        {
            Player player = new Player();
            BaseCreatureMock creature = new BaseCreatureMock();
            creature.MockInit();
            player.Init(
                creature.physics,
                new CreatureFsmMock<PlayerState>(),
                creature.timers,
                creature.bars,
                new BarMock(),
                new SpellCasterMock(),
                new CreatureHealthMock()
             );
            return player;
        }
    }
}
