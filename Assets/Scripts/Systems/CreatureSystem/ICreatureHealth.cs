public interface ICreatureHealth
{
    float Health { get; }
    void Heal(float amount);
    void Hurt(float amount);
}
