using System;

namespace Tests
{
    public class PlayerInputMock : IPlayerInput
    {
        public bool IsAnyHeld()
        {
            throw new NotImplementedException();
        }

        public bool IsHeld(PlayerInputKey key)
        {
            throw new NotImplementedException();
        }

        public bool IsPressedThisFrame(PlayerInputKey key)
        {
            throw new NotImplementedException();
        }

        public void Latch()
        {
            throw new NotImplementedException();
        }
    }

}
