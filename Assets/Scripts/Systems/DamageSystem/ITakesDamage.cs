public interface ITakesDamage
{
    float CollisionDefense { get; set; }
    void TakeDamage(float amount);
}
