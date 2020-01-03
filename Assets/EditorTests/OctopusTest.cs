using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class OctopusTest
    {
        [Test]
        public void ConstructorDoesNotCrash()
        {
            GetOctopus();
        }

        [Test]
        public void NextFrameDoesNotCrash()
        {
            Octopus octopus = GetOctopus();
            octopus.FixedUpdate();
        }

        private Octopus GetOctopus()
        {
            BaseCreatureWithTestPoints creature = CreatureTest.GetCreature();
            Octopus octopus = new Octopus();
            octopus.Init(creature, new CreatureFsmMock<OctopusState>());
            return octopus;
        }
    }
}
