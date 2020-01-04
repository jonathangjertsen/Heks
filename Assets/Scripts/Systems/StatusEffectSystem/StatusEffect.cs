using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/StatusEffect", order = 1)]
public class StatusEffect : ScriptableObject, IStatusEffect
{
    [Header("Data")]
    public StatusEffectType type;
    public float intensity;

    [Header("Info")]
    public Sprite sprite;
    [TextArea] public string description;

    public StatusEffectType Type { get => type; set => type = value; }
    public float Intensity { get => intensity; set => intensity = value; }

    public StatusEffect(StatusEffectType type, float intensity)
    {
        Type = type;
        Intensity = intensity;
    }
}