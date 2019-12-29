using UnityEngine;

public class SpellSpawn : MonoBehaviour, IFlipX
{
    public Bullet spell;
    private bool flipX;
    public bool FlipX { get => flipX; set => flipX = value; }

    public void Cast(Vector2 initialVelocity, float charge)
    {
        Bullet bullet = Instantiate(spell, transform.position, transform.rotation);
        int flipXAsInt = flipX ? 1 : -1;
        bullet.Launch(
            initialVelocity + new Vector2(0.1f * charge * -flipXAsInt, 0.1f * charge),
            100f * charge * -flipXAsInt
        );
    }

    private void FixedUpdate()
    {

    }


}
