namespace Tests
{
    internal class TakesStatusEffectMock : CollisionSystemParticipatorMock, ITakesStatusEffect
    {
        public IStatusEffect lastEffectTaken;
        public IDealsStatusEffect lastEffectDealer;

        public void TakeStatusEffect(IStatusEffect statusEffect, IDealsStatusEffect dealer)
        {
            lastEffectTaken = statusEffect;
            lastEffectDealer = dealer;
        }
    }
}