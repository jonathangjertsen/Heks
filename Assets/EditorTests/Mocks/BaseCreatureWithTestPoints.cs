namespace Tests
{
    public class BaseCreatureWithTestPoints : BaseCreature
    {
        public bool mock_onDeathStartCalled = false;
        public bool mock_onDeathFinishedCalled = false;
        public bool mock_onHurtCompletedCalled = false;

        public void InitWithMocks(float maxHealth)
        {
            this.maxHealth = maxHealth;
            Init(
                new CreaturePhysics(
                    new RigidBodyMock(),
                    new TransformMock(),
                    new CreaturePhysicsProperties()
                ),
                new BarMock()
            );
            health.PrependZeroHealthCallback(() => mock_onDeathStartCalled = true);
            SetDeathFinishedCallback(() => mock_onDeathFinishedCalled = true);
            SetHurtFinishedCallback(() => mock_onHurtCompletedCalled = true);
        }
    }

}
