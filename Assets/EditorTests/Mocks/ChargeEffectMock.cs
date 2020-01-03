using System;

namespace Tests
{
    public class ChargeEffectMock : ICanBeActivated
    {
        public bool IsActive()
        {
            throw new NotImplementedException();
        }

        public void SetActive(bool active)
        {
            throw new NotImplementedException();
        }
    }

}
