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
        public void ConstructorDoesNotCrash()
        {
            GetPlayer();
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
                new PlayerInputMock(),
                new EventBusMock()
            );
            return player;
        }
    }
}
