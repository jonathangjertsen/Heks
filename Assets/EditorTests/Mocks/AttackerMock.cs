namespace Tests
{
    public class AttackerMock : SysCollisionParticipatorMock, IDealsDamage
    {
        public float CollisionAttack { get; set; }
        public void DealDamage(float amount) { }
    }

}
