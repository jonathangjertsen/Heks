public class CreatureHealth
{
    private readonly Bar healthBar;
    private readonly float maxHealth;
    private float currentHealth;
    private readonly OnZeroHealth onZeroHealth;

    public delegate void OnZeroHealth();

    public CreatureHealth(Bar healthBar, float maxHealth, OnZeroHealth onZeroHealth)
    {
        this.healthBar = healthBar;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.onZeroHealth = onZeroHealth;
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
