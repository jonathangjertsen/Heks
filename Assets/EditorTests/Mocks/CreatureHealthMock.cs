namespace Tests
{
    class CreatureHealthMock : ICreatureHealth
    {
        public float Health { get; private set; }

        public void Heal(float amount)
        {
            Health += amount;
        }

        public void Hurt(float amount)
        {
            Health -= amount;
        }
    }

}
