using System;

namespace Tests
{
    public class ChargeEffectMock : ICanBeActivated
    {
        public bool isActive;
        public bool IsActive()
        {
            return isActive;
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }
    }
}
