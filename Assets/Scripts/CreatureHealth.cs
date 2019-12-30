public interface ICreatureHealth
{
    float Health { get; set; }
}

public class CreatureHealth : ICreatureHealth
{
    private readonly BarBehaviour healthBar;
    private readonly float maxHealth;
    private float currentHealth;
    private readonly OnZeroHealth onZeroHealth;

    public delegate void OnZeroHealth();

    public CreatureHealth(BarBehaviour healthBar, float maxHealth, OnZeroHealth onZeroHealth)
    {
        this.healthBar = healthBar;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.onZeroHealth = onZeroHealth;
        Health = maxHealth;
    }

    public float Health
    {
        get => currentHealth;
        set
        {
            float cappedHealth = value;
            if (value >= maxHealth)
            {
                cappedHealth = maxHealth;
            }
            else if (value <= 0)
            {
                cappedHealth = 0;
                healthBar.Hide();
                onZeroHealth();
            }
            healthBar.FillTo(cappedHealth / maxHealth);
            currentHealth = cappedHealth;
        }
    }
}
