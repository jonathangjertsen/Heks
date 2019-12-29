using UnityEngine;

public class SpellSpawn : MonoBehaviour, IFlipX
{
    public Spell spell;
    public bool FlipX { get; set; }

    public void Cast(Vector2 initialVelocity, float charge)
    {
        var spell = Instantiate(this.spell, transform.position, transform.rotation);
        spell.Launch(initialVelocity, charge, FlipX);
    }
}
