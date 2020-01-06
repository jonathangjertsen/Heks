public interface IEventBus
{
    void PlayerDied();
    void Paused();
    void Unpaused();
    void LevelRestarted();
    void LevelExited();
    void PlayerDamaged(float magnitude);
    void ZoomOutStart();
    void ZoomOutStop();
}
