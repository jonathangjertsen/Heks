using System;
using UnityEngine;

[Serializable]
public class SpellSpawner
{
    private ISpell activeSpell;
    private System.Random random;
    [SerializeField] ISpell[] spells;
    private ISpellInstantiator instantiator;
    private ISpellVisualizer viz;

    public void Init(ISpellInstantiator instantiator, ISpellVisualizer viz, ISpell[] spells)
    {
        this.spells = spells;
        this.instantiator = instantiator;
        this.viz = viz;

        random = new System.Random();

        SetUpNextSpell();
    }

    public void Cast(Vector2 initialVelocity, float charge, bool flipX)
    {
        ISpell spell = instantiator.InstantiateSpell(activeSpell);
        spell.Launch(initialVelocity, charge, flipX);
        SetUpNextSpell();
    }

    private void SetUpNextSpell()
    {
        activeSpell = spells[random.Next(spells.Length)];
        viz.ShowSpell(activeSpell);
    }
}
