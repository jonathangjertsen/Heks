
public class StatusEffect : IStatusEffect
{
    public StatusEffectType Type { get; set; }
    public float Intensity { get; set; }

    public StatusEffect(StatusEffectType type, float intensity)
    {
        Type = type;
        Intensity = intensity;
    }
}