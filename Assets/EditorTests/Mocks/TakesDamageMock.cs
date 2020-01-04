namespace Tests
{
    internal class TakesDamageMock : CollisionSystemParticipatorMock, ITakesDamage
    {
        public float defense = 1f;
        public float lastDamageTaken = -1f;

        public float CollisionDefense { get => defense; set => defense = value; }

        public void TakeDamage(float amount)
        {
            lastDamageTaken = amount;
        }
    }
}