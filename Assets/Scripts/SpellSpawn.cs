using UnityEngine;

public class SpellSpawn : MonoBehaviour, IFlipX
{
    public Bullet spell;
    private bool flipX;
    public bool FlipX { get => flipX; set => flipX = value; }

    public void Cast(float charge)
    {
        Bullet bullet = Instantiate(spell, transform.position, transform.rotation);
        int flipXAsInt = flipX ? 1 : -1;
        bullet.Launch(
            new Vector2(0.01f * charge * -flipXAsInt, 0.24f * charge),
            200f * charge * flipXAsInt
        );
    }

    private void FixedUpdate()
    {

    }


}
