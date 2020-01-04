namespace Tests
{
    internal class DealsStatusEffectMock : CollisionSystemParticipatorMock, IDealsStatusEffect
    {
        public IStatusEffect effectToDeal;
        public ITakesStatusEffect lastReceiver;

        public IStatusEffect DealStatusEffect(ITakesStatusEffect taker)
        {
            lastReceiver = taker;
            return effectToDeal;
        }
    }
}