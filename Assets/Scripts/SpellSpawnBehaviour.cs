using System.Collections.Generic;
using System;
using UnityEngine;

public interface ISpellCaster : IFlipX
{
    void Cast(Vector2 initialVelocity, float charge);
}

public interface ISpellInstantiator
{
    ISpell InstantiateSpell(ISpell source);
}

public interface ISpellVisualizer
{
    void ShowSpell(ISpell spell);
}


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

public class SpellSpawnBehaviour : MonoBehaviour, IFlipX, ISpellInstantiator, ISpellCaster
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
