namespace Tests
{
    internal class TakesStatusEffectMock : CollisionSystemParticipatorMock, ITakesStatusEffect
    {
        public IStatusEffect lastEffectTaken;

        public void TakeStatusEffect(IStatusEffect statusEffect)
        {
            lastEffectTaken = statusEffect;
        }
    }
}