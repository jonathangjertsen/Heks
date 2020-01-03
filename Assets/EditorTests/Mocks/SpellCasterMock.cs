using System;
using UnityEngine;

namespace Tests
{
    public class SpellCasterMock : ISpellCaster
    {
        public bool FlipX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Cast(Vector2 initialVelocity, float charge)
        {
            throw new NotImplementedException();
        }
    }

}
