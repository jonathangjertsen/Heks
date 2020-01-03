using System;

namespace Tests
{
    public class PauseMenuMock : IPauseMenu
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
