public interface IPlayerInput
{
    void Latch();
    bool IsHeld(PlayerInputKey key);
    bool IsPressedThisFrame(PlayerInputKey key);
    bool IsAnyHeld();
}
