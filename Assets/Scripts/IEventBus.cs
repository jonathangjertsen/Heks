public interface IEventBus
{
    void PlayerDied();
    void Paused();
    void Unpaused();
    void ChargeStart();
    void ChargeStop();
    void LevelRestarted();
    void LevelExited();
}
