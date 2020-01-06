public interface IEventBus
{
    void PlayerDied();
    void Paused();
    void Unpaused();
    void LevelRestarted();
    void LevelExited();
}
