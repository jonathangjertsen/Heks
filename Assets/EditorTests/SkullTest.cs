using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SkullTest
    {
        [Test]
        public void ConstructorDoesNotCrash()
        {
            GetSkull();
        }

        private Skull GetSkull()
        {
            Skull skull = new Skull();
            BaseCreatureWithTestPoints creature = new BaseCreatureWithTestPoints();
            creature.InitWithMocks(100f);
            skull.Init(creature, new CreatureFsmMock<SkullState>(), new PlayerLocatorMock());
            return skull;
        }
    }
}
