namespace Tests
{
    public class CollisionSystemParticipatorMock : ICollisionSystemParticipator
    {
        public bool As<T>(out T converted)
        {
            return ConvertToInterface.As<T>(this, out converted);
        }

        public void CollidedWith(ICollisionSystemParticipator other)
        {
            throw new System.NotImplementedException();
        }

        public void ExitedCollisionWith(ICollisionSystemParticipator other)
        {
            throw new System.NotImplementedException();
        }

        public void ExitedTriggerWith(ICollisionSystemParticipator other)
        {
            throw new System.NotImplementedException();
        }

        public ICollisionSystemParticipator GetCollisionSystemParticipator()
        {
            throw new System.NotImplementedException();
        }

        public void TriggeredWith(ICollisionSystemParticipator other)
        {
            throw new System.NotImplementedException();
        }
    }
}
