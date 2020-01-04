namespace Tests
{
    internal class DealsDamageMock : CollisionSystemParticipatorMock, IDealsDamage
    {
        public float attack = 1f;
        public float lastDamageGiven = -1f;

        public float CollisionAttack { get => attack; set => attack = value; }

        public void DealDamage(float amount)
        {
            lastDamageGiven = amount;
        }
    }
}