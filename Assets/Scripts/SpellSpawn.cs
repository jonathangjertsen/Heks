using System.Collections.Generic;
using System;
using UnityEngine;

public class SpellSpawn : MonoBehaviour, IFlipX
{
    private System.Random random;
    private Spell activeSpell;
    private List<Spell> spells;

    public SpellViz viz;
    public Bullet bullet;
    public StraightBullet straightBullet;
    public BulletRing bulletRing;

    public bool FlipX { get; set; }

    public void Start()
    {
        spells = new List<Spell>() {
            bullet,
            straightBullet,
            bulletRing
        };
        random = new System.Random();

        SetUpNextSpell();
    }

    public void Cast(Vector2 initialVelocity, float charge)
    {
        Spell spell = Instantiate(activeSpell, transform.position, transform.rotation);
        spell.Launch(initialVelocity, charge, FlipX);
        SetUpNextSpell();
    }

    private void SetUpNextSpell()
    {
        activeSpell = spells[random.Next(spells.Count)];
        viz.SetSpell(activeSpell);
    }
}
