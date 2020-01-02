using UnityEngine;

public class SpellSpawnerBehaviour : MonoBehaviour, IFlipX, ISpellInstantiator, ISpellCaster
{
    public SpellVizBehaviour viz;
    public SpellSpawner spellSpawner;
    public SpellBehaviour[] spells;

    public bool FlipX { get; set; }

    public void Start()
    {
        spellSpawner.Init(this, viz, spells);
    }

    public ISpell InstantiateSpell(ISpell spell)
    {
        return Instantiate((SpellBehaviour)spell, transform.position, transform.rotation);
    }

    public void Cast(Vector2 initialVelocity, float charge)
    {
        spellSpawner.Cast(initialVelocity, charge, FlipX);
    }
}
