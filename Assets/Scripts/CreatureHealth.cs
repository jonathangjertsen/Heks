public interface IBarDisplay : IFlipX
{
    void FillTo(float proportion);
    void Hide();
}

public interface ICreatureHealth
{
    float Health { get; set; }
}

public class CreatureHealth : ICreatureHealth
{
    private readonly IBarDisplay healthBar;
    private readonly float maxHealth;
    private float currentHealth;
    private OnZeroHealth onZeroHealth;

    public delegate void OnZeroHealth();

    public CreatureHealth(IBarDisplay healthBar, float maxHealth)
    {
        this.healthBar = healthBar;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        Health = maxHealth;
    }

    public void SetZeroHealthCallback(OnZeroHealth callback)
    {
        onZeroHealth = callback;
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
            else if (value <= 0.0001f)
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
