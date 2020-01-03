using UnityEngine;

public class SpellStraightBehaviour : SpellBehaviour, IDealsDamage
{
    public float collisionAttack = 100;
    public float CollisionAttack { get => collisionAttack; set => collisionAttack = value; }

    override public void Launch(Vector2 initialVelocity, float charge, bool flipX)
    {
        int flipXAsInt = flipX ? 1 : -1;
        physics.Accelerate(initialVelocity + ((initialVelocity.normalized * 20 + new Vector2(-1, -5) * 10) * charge * flipXAsInt));
    }
}
