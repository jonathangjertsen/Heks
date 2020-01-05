using UnityEngine;

namespace Tests
{
    internal class DealsStatusEffectMock : CollisionSystemParticipatorMock, IDealsStatusEffect
    {
        public IStatusEffect effectToDeal;
        public ITakesStatusEffect lastReceiver;
        public Vector2 position;

        public IStatusEffect DealStatusEffect(ITakesStatusEffect taker)
        {
            lastReceiver = taker;
            return effectToDeal;
        }

        public Vector2 Position()
        {
            return position;
        }
    }
}