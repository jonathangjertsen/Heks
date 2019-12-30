using System.Collections.Generic;
using System;
using UnityEngine;

public class SpellSpawnBehaviour : MonoBehaviour, IFlipX
{
    private System.Random random;
    private SpellBehaviour activeSpell;
    private List<SpellBehaviour> spells;

    public SpellVizBehaviour viz;
    public BulletBehaviour bullet;
    public StraightBulletBehaviour straightBullet;
    public BulletRingBehaviour bulletRing;

    public bool FlipX { get; set; }

    public void Start()
    {
        spells = new List<SpellBehaviour>() {
            bullet,
            straightBullet,
            bulletRing
        };
        random = new System.Random();

        SetUpNextSpell();
    }

    public void Cast(Vector2 initialVelocity, float charge)
    {
        SpellBehaviour spell = Instantiate(activeSpell, transform.position, transform.rotation);
        spell.Launch(initialVelocity, charge, FlipX);
        SetUpNextSpell();
    }

    private void SetUpNextSpell()
    {
        activeSpell = spells[random.Next(spells.Count)];
        viz.SetSpell(activeSpell);
    }
}
