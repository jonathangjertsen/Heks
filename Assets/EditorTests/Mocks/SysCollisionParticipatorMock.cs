using System;
using UnityEngine;

namespace Tests
{
    public class SysCollisionParticipatorMock : SystemParticipator, ICollisionSystemParticipator
    {
        public void CollidedWith(ICollisionSystemParticipator other)
        {
        }

        public void ExitedCollisionWith(ICollisionSystemParticipator other)
        {
        }

        public void ExitedTriggerWith(ICollisionSystemParticipator other)
        {
            throw new NotImplementedException();
        }

        public ICollisionSystemParticipator GetCollisionSystemParticipator() => this;

        public void OnTriggerEnter2D(Collider2D other)
        {
            throw new NotImplementedException();
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            throw new NotImplementedException();
        }

        public void TriggeredWith(ICollisionSystemParticipator other)
        {
            throw new NotImplementedException();
        }
    }

}
