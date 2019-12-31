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
        public void Octopus_ConstructorDoesNotCrash()
        {
            GetOctopus();
        }

        [Test]
        public void Octopus_NextFrameDoesNotCrash()
        {
            Octopus octopus = GetOctopus();
            octopus.NextFrame();
        }

        private Octopus GetOctopus()
        {
            BaseCreatureMock creature = new BaseCreatureMock();
            creature.MockInit();
            Octopus octopus = new Octopus();
            octopus.Init(creature, new CreatureFsmMock<OctopusState>());
            return octopus;
        }
    }
}
